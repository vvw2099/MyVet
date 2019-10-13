using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Models;
using MyVet.Web.Helper;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;

namespace MyVet.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMailHelper _mailhelper;
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration; //no requiere interfaz viene por defecto
        private readonly DataContext _dataContext;
        
        public AccountController(IUserHelper userHelper, IConfiguration configuration,DataContext dataContext,
            IMailHelper mailHelper)
        {
            _dataContext = dataContext;
            _mailhelper = mailHelper;
            _userHelper = userHelper;
            _configuration = configuration; // exta inyeccion sirve para validar token
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await  _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Usuario o Contraseña Invalido");
                model.Password = string.Empty;
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if(user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password
                        );

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMonths(4),
                            signingCredentials: credentials
                            );

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(string.Empty, results);
                    }
                }
            }
            return BadRequest();
        }
        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await AddUserAsync(model);
                if(user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya fue registrado");
                    return View(model);
                }

                var owner = new Owner
                {
                    Pets = new List<Pet>(),
                    user = user,    
                };

                _dataContext.Owners.Add(owner);
                await _dataContext.SaveChangesAsync();

                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail","Account",new {
                    userid =user.Id,
                    token = myToken
                },protocol:HttpContext.Request.Scheme);

                _mailhelper.SendMail(model.Username, "Email confirmation",
                    $"<table style = 'max-width: 600px; padding: 10px; margin:0 auto; border-collapse: collapse;'>" +
                    $"  <tr>" +
                    $"    <td style = 'background-color: #34495e; text-align: center; padding: 0'>" +
                    $"       <a href = 'https://www.facebook.com/NuskeCIV/' >" +
                    $"         <img width = '20%' style = 'display:block; margin: 1.5% 3%' src= 'https://veterinarianuske.com/wp-content/uploads/2016/10/line_separator.png'>" +
                    $"       </a>" +
                    $"  </td>" +
                    $"  </tr>" +
                    $"  <tr>" +
                    $"  <td style = 'padding: 0'>" +
                    $"     <img style = 'padding: 0; display: block' src = 'https://veterinarianuske.com/wp-content/uploads/2018/07/logo-nnske-blanck.jpg' width = '100%'>" +
                    $"  </td>" +
                    $"</tr>" +
                    $"<tr>" +
                    $" <td style = 'background-color: #ecf0f1'>" +
                    $"      <div style = 'color: #34495e; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>" +
                    $"            <h1 style = 'color: #e67e22; margin: 0 0 7px' > Hola </h1>" +
                    $"                    <p style = 'margin: 2px; font-size: 15px'>" +
                    $"                      El mejor Hospital Veterinario Especializado de la Ciudad de Morelia enfocado a brindar servicios médicos y quirúrgicos<br>" +
                    $"                      aplicando las técnicas más actuales y equipo de vanguardia para diagnósticos precisos y tratamientos oportunos..<br>" +
                    $"                      Entre los servicios tenemos:</p>" +
                    $"      <ul style = 'font-size: 15px;  margin: 10px 0'>" +
                    $"        <li> Urgencias.</li>" +
                    $"        <li> Medicina Interna.</li>" +
                    $"        <li> Imagenologia.</li>" +
                    $"        <li> Pruebas de laboratorio y gabinete.</li>" +
                    $"        <li> Estetica canina.</li>" +
                    $"      </ul>" +
                    $"  <div style = 'width: 100%;margin:20px 0; display: inline-block;text-align: center'>" +
                    $"    <img style = 'padding: 0; width: 200px; margin: 5px' src = 'https://veterinarianuske.com/wp-content/uploads/2018/07/tarjetas.png'>" +
                    $"  </div>" +
                    $"  <div style = 'width: 100%; text-align: center'>" +
                    $"    <h2 style = 'color: #e67e22; margin: 0 0 7px' >Confirmación de Correo </h2>" +
                    $"    To allow the user,plase click in this link:</ br ></ br > " +
                    $"    <a style ='text-decoration: none; border-radius: 5px; padding: 11px 23px; color: white; background-color: #3498db' href = \"{tokenLink}\">Confirmar Correo</a>" +
                    $"    <p style = 'color: #b3b3b3; font-size: 12px; text-align: center;margin: 30px 0 0' > Nuskë Clinica Integral Veterinaria 2019 </p>" +
                    $"  </div>" +
                    $" </td>" +
                    $"</tr>" +
                    $"</table>");

                ViewBag.Message = "Las intrucciones de su usuario han sido enviadas a su email.";
                return View(model);
            }
            return View(model);
        }

        private async Task<User> AddUserAsync(AddUserViewModel model)
        {
            var user = new User
            {
                Address = model.Address,
                Document = model.Document,
                Email = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Username
            };

            var result = await _userHelper.AddUserAsync(user, model.Password);
            if(result != IdentityResult.Success)
            {
                return null;
            }

            var newUser = await _userHelper.GetUserByEmailAsync(model.Username);
            await _userHelper.AddUserToRoleAsync(newUser, "Customer");

            return newUser;
        }

        public async Task<IActionResult> ChangeUser()
        {
            var owner = await _dataContext.Owners
                .Include(o => o.user)
                .FirstOrDefaultAsync(o => o.user.UserName.ToLower() == (User.Identity.Name.ToLower()));

            if (owner == null)
            {
                return NotFound();
            }

            var view = new EditUserViewModel
            {
                Address = owner.user.Address,
                Document = owner.user.Document,
                FirstName = owner.user.FirstName,
                Id = owner.Id,
                LastName = owner.user.LastName,
                PhoneNumber = owner.user.PhoneNumber
            };
            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var owner = await _dataContext.Owners
                    .Include(o => o.user)
                    .FirstOrDefaultAsync(o => o.Id == model.Id);

                owner.user.Document = model.Document;
                owner.user.FirstName = model.FirstName;
                owner.user.LastName = model.LastName;
                owner.user.Address = model.Address;
                owner.user.PhoneNumber = model.PhoneNumber;

                await _userHelper.UpdateUserAsync(owner.user);
                return RedirectToAction("Index","Home");
            }

            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                if(user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No se encontro el usuario");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId)||string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ComfirmEmailAsync(user, token);
            if(!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();  
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty,"Este correo no corresponde a su usuario");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new {token = myToken}, protocol:HttpContext.Request.Scheme
                    );

                _mailhelper.SendMail(model.Email,"Veterinaria Zulu - Cambio de Password",
                    $"<h1>Cambio de Contraseña</h1>"+
                    $"Para cambiar la contraseña haga click en este link:</br></br>"+
                    $"<a href = \"{link}\">Cambiar Contraseña</a>");

                ViewBag.Message = "Las instrucciones  para cambiar su contraseña se han enviado a su correo.";
                return View();
            }
            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if(user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Constraseña cambiada exitosamente.";
                    return View(model);
                }

                ViewBag.Message = "Error al cambiar la contraseña.";
                return View(model);
            }
            ViewBag.Message = "No se encontró el usuario.";
            return View(model);
        }
    }
}
