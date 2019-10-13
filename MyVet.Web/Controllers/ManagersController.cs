using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Helper;
using MyVet.Web.Models;

namespace MyVet.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagersController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        public ManagersController(DataContext context, IUserHelper userHelper, IMailHelper mailHelper)
        {
            _context = context;
            _mailHelper = mailHelper;
            _userHelper = userHelper;
        }

        // GET: Managers
        public IActionResult Index()
        {
            return View( _context.Managers.Include(m => m.user));
        }

        // GET: Managers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manager = await _context.Managers
                .Include(o=>o.user)
                .FirstOrDefaultAsync(m => m.Id == id.Value);
            if (manager == null)
            {
                return NotFound();
            }

            return View(manager);
        }

        // GET: Managers/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await AddUser(model);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya esta registrado");
                    return View(model);
                }

                var manager = new Manager { user = user };

                _context.Managers.Add(manager);
                await _context.SaveChangesAsync();

                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(model.Username, "Confirmacion de Correo",
                    $"<h1>Confirmación de correo</h1>" +
                    $"Para crear el usuario, " +
                    $"haga click en este enlace:</br></br><a href = \"{tokenLink}\">Confirmar correo</a>");

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        private async Task<User> AddUser(AddUserViewModel model)
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

            var result = await _userHelper.AddUserAsync(user,model.Password);
            if(result != IdentityResult.Success)
            {
                return null;
            }

            var newUser = await _userHelper.GetUserByEmailAsync(model.Username);
            await _userHelper.AddUserToRoleAsync(newUser,"Admin");
            return newUser;
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manager = await _context.Managers
                .Include(m => m.user)
                .FirstOrDefaultAsync(m => m.Id == id.Value);
            if (manager == null)
            {
                return NotFound();
            }

            var view = new EditUserViewModel
            {
                Address = manager.user.Address,
                Document = manager.user.Document,
                FirstName = manager.user.FirstName,
                Id = manager.Id,
                LastName = manager.user.LastName,
                PhoneNumber = manager.user.PhoneNumber
            };

            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                var owner = await _context.Owners
                    .Include(o => o.user)
                    .FirstOrDefaultAsync(o => o.Id == model.Id);

                owner.user.Document = model.Document;
                owner.user.FirstName = model.FirstName;
                owner.user.LastName = model.LastName;
                owner.user.Address = model.Address;
                owner.user.PhoneNumber = model.PhoneNumber;

                await _userHelper.UpdateUserAsync(owner.user);
                                
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manager = await _context.Managers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manager == null)
            {
                return NotFound();
            }

            _context.Managers.Remove(manager);
            await _context.SaveChangesAsync();
            await _userHelper.DeleteUserAsync(manager.user.Email);

            return RedirectToAction(nameof(Index));
        }

        

        private bool ManagerExists(int id)
        {
            return _context.Managers.Any(e => e.Id == id);
        }
    }
}
