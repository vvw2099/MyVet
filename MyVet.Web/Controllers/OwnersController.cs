﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Helper;
using MyVet.Web.Models;

namespace MyVet.Web.Controllers
{
    [Authorize(Roles ="Admin")]
    public class OwnersController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;

        public OwnersController(DataContext context, IUserHelper userHelper,ICombosHelper combosHelper,
            IConverterHelper converterHelper, IImageHelper imageHelper,IMailHelper mailHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
        }

        // GET: Owners
        public IActionResult Index()
        {
            return View( _context.Owners.Include(o =>o.user).Include(o=>o.Pets));
        }

        // GET: Owners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .Include(o => o.user)
                .Include(o => o.Pets)
                .ThenInclude(p => p.PetType)
                .Include(o => o.Pets)
                .ThenInclude(p => p.Histories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // GET: Owners/Create
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

                var response = await _userHelper.AddUserAsync(user, model.Password);

                if (response.Succeeded)
                {
                    var userInDB = await _userHelper.GetUserByEmailAsync(model.Username);
                    await _userHelper.AddUserToRoleAsync(userInDB, "Customer");

                    var owner = new Owner
                    {
                        Agendas = new List<Agenda>(),
                        Pets = new List<Pet>(),
                        user = userInDB
                    };

                    _context.Owners.Add(owner);

                    try
                    {
                        await _context.SaveChangesAsync();

                        var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                        var tokenLink = Url.Action("ConfirmEmail","Account",new { 
                            userId = user.Id,
                            token = myToken
                        },protocol: HttpContext.Request.Scheme);

                        _mailHelper.SendMail(model.Username,"Email confirmation",$"<h1>Confirmación de Correo</h1>"+
                            $"To allow the user, " +
                            $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirmar Correo</a>");

                        return RedirectToAction(nameof(Index));
                    }
                    catch(Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.ToString());
                        return View(model);
                    }
                    
                }
                ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
            }
            return View(model);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .Include(o => o.user)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (owner == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Address = owner.user.Address,
                Document = owner.user.Document,
                FirstName = owner.user.FirstName,
                Id = owner.Id,
                LastName = owner.user.LastName,
                PhoneNumber = owner.user.PhoneNumber
            };
            return View(model);
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

            var owner = await _context.Owners
                .Include(u => u.user)
                .Include(o => o.Pets)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }
            if(owner.Pets.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "La propietario no se puede eliminar porque tiene Mascotas");
                return RedirectToAction(nameof(Index));
            }

            await _userHelper.DeleteUserAsync(owner.user.Email);

            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
                            
        }

            

        private bool OwnerExists(int id)
        {
            return _context.Owners.Any(e => e.Id == id);
        }
        public async Task<IActionResult> AddPet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var owner = await _context.Owners.FindAsync(id.Value);
            if(owner == null)
            {
                return NotFound();
            }

            var model = new PetViewModel
            {
                Born = DateTime.Today,
                OwnerId = owner.Id,
                PetTypes = _combosHelper.GetComboPetTypes()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddPet(PetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;
                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile);
                }
                else
                {

                }
                
                var pet = await _converterHelper.ToPetAsync(model, path, true);
                _context.Pets.Add(pet);
                await _context.SaveChangesAsync();
                return RedirectToAction($"Details/{model.OwnerId}");
                    
            }
            model.PetTypes = _combosHelper.GetComboPetTypes();
            return View(model);
        }
        public async Task<IActionResult> EditPet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.Owner)
                .Include(p => p.PetType)
                .FirstOrDefaultAsync(p => p.Id == id);
            if(pet == null)
            {
                return NotFound();
            }

            return View(_converterHelper.ToPetViewModel(pet));
        }

        [HttpPost]
        public async Task<IActionResult> EditPet(PetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = model.ImageUrl;

                if(model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile);
                }

                var pet = await _converterHelper.ToPetAsync(model, path,false);
                _context.Pets.Update(pet);
                await _context.SaveChangesAsync();
                return RedirectToAction($"Details/{model.OwnerId}");
            }

            model.PetTypes = _combosHelper.GetComboPetTypes();
            return View(model);
        }
        public async Task<IActionResult> DetailsPet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pet = await _context.Pets
                .Include(p => p.Owner)
                .ThenInclude(o => o.user)
                .Include(p => p.Histories)
                .ThenInclude(h => h.ServiceType)
                .FirstOrDefaultAsync(o => o.Id == id.Value);

            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        public async Task<IActionResult> AddHistory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets.FindAsync(id.Value);

            if(pet == null)
            {
                return NotFound();
            }

            var model = new HistoryViewModel
            {
                Date = DateTime.Now,
                PetId = pet.Id,
                ServiceTypes = _combosHelper.GetComboServiceTypes()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddHistory(HistoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var history = await _converterHelper.ToHistoryAsync(model, true);
                _context.Histories.Add(history);
                await _context.SaveChangesAsync();

                return RedirectToAction($"{nameof(DetailsPet)}/{model.PetId}");
            }
            model.ServiceTypes = _combosHelper.GetComboServiceTypes();

            return View(model);
        }

        public async Task<IActionResult> EditHistory(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var history = await _context.Histories
                .Include(h => h.Pet)
                .Include(h => h.ServiceType)
                .FirstOrDefaultAsync(p => p.Id == id.Value);

            if(history == null)
            {
                return NotFound();
            }
            return View(_converterHelper.ToHistoryViewModel(history));
        }

        [HttpPost]
        public async Task<IActionResult> EditHistory(HistoryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var history = await _converterHelper.ToHistoryAsync(model, false);
                _context.Histories.Update(history);
                await _context.SaveChangesAsync();

                return RedirectToAction($"{nameof(DetailsPet)}/{model.PetId}");
            }
            model.ServiceTypes = _combosHelper.GetComboServiceTypes();

            return View(model);
        }

        public async Task<IActionResult> DeleteHistory(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var history = await _context.Histories
                .Include(h => h.Pet)
                .FirstOrDefaultAsync(h => h.Id == id.Value);

            if(history == null)
            {
                return NotFound();
            }

            _context.Histories.Remove(history);
            await _context.SaveChangesAsync();

            return RedirectToAction($"{nameof(DetailsPet)}/{history.Pet.Id}");
        }
        public async Task<IActionResult> DeletePet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.Owner)
                .Include(h => h.Histories)
                .FirstOrDefaultAsync(h => h.Id == id.Value);

            if (pet == null)
            {
                return NotFound();
            }

            if(pet.Histories.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "La mascota no se puede eliminar porque tiene Historial");
                return RedirectToAction($"{nameof(Details)}/{pet.Owner.Id}");
            }
            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return RedirectToAction($"{nameof(Details)}/{pet.Owner.Id}");
        }
    }
}
