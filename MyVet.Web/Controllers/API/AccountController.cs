using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyVet.Common.Models;
using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Helper;

namespace MyVet.Web.Controllers.API
{
    [Route("api/[Controller]")]
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public AccountController(DataContext dataContext, IUserHelper userHelper, IMailHelper mailHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "Bad Request"
                });
            }

            var user = await _userHelper.GetUserByEmailAsync(request.Email);
            if (user != null)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "Este correo ya está registrado"
                });
            }

            user = new User
            {
                Address = request.Address,
                Document = request.Document,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.Phone,
                UserName = request.Email
            };

            var result = await _userHelper.AddUserAsync(user, request.Password);
            if (result != IdentityResult.Success)
            {
                return BadRequest(result.Errors.FirstOrDefault().Description);
            }

            var userNew = await _userHelper.GetUserByEmailAsync(request.Email);
            await _userHelper.AddUserToRoleAsync(userNew, "Customer");
            _dataContext.Owners.Add(new Owner { user = userNew });
            await _dataContext.SaveChangesAsync();

            var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            var tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);

            _mailHelper.SendMail(request.Email, "Confirmación de Correo", $"<h1>Veterinaria Zulu - Confirmación de Correo</h1>" +
               $"Para activar su usuario, " +
               $"por favor haga click en este link:</br></br><a href = \"{tokenLink}\">Confirmar</a>");

            return Ok(new Response<object>
            {
                IsSuccess = true,
                Message = "Una confirmación de fue enviada a su correo. Por favor confirme su usuario e ingrese a la App."
            });


        }

        [HttpPost]
        [Route("RecoverPassword")]
        public async Task<IActionResult> RecoverPassword([FromBody] EmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "Bad request"
                });
            }

            var user = await _userHelper.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "Este correo no tiene asignado ningun usuario."
                });
            }

            var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            var link = Url.Action("ResetPassword", "Account", new { token = myToken }, protocol: HttpContext.Request.Scheme);
            _mailHelper.SendMail(request.Email, "Nueva Contraseña", $"<h1>Recuperar Contraseña</h1>" +
                $"Para cambiar su contraseña haga click en este link:</br></br>" +
                $"<a href = \"{link}\">Recuperar</a>");

            return Ok(new Response<object>
            {
                IsSuccess = true,
                Message = "Un Correo con intrucciones para cambiar la clave se ha enviado a su correo."
            });
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutUser([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userEntity = await _userHelper.GetUserByEmailAsync(request.Email);
            if (userEntity == null)
            {
                return BadRequest("User not found.");
            }

            userEntity.FirstName = request.FirstName;
            userEntity.LastName = request.LastName;
            userEntity.Address = request.Address;
            userEntity.PhoneNumber = request.Phone;
            userEntity.Document = request.Phone;

            var respose = await _userHelper.UpdateUserAsync(userEntity);
            if (!respose.Succeeded)
            {
                return BadRequest(respose.Errors.FirstOrDefault().Description);
            }

            var updatedUser = await _userHelper.GetUserByEmailAsync(request.Email);
            return Ok(updatedUser);
        }

        [HttpPost]
        [Route("ChangePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "Bad request"
                });
            }

            var user = await _userHelper.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = "Este correo no esta asignado a ningun usuario."
                });
            }

            var result = await _userHelper.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new Response<object>
                {
                    IsSuccess = false,
                    Message = result.Errors.FirstOrDefault().Description
                });
            }

            return Ok(new Response<object>
            {
                IsSuccess = true,
                Message = "La contraseña fue cambiada con exito!"
            });
        }
    }
}
