using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyVet.Web.Data;
using MyVet.Web.Helper;
using MyVet.Web.Models;

namespace MyVet.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AgendaController : Controller
    {
        private readonly ICombosHelper _combosHelper;
        private readonly DataContext _context;
        private readonly IAgendaHelper _agendaHelper;

        public AgendaController(ICombosHelper combosHelper, DataContext context,IAgendaHelper agendaHelper)
        {
            _agendaHelper = agendaHelper;
            _context = context;
            _combosHelper = combosHelper;
        }

        
        public IActionResult Index()
        {
            return View( _context.Agendas
                .Include(a => a.Owner)
                .ThenInclude(o => o.user)
                .Include(a => a.Pet)
                .Where(a => a.Date >= DateTime.Today.ToUniversalTime()));
        }

        public async Task<IActionResult> AddDaysAsync()
        {
            await _agendaHelper.AddDaysAsync(7);
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Assing(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agenda = await _context.Agendas
                .FirstOrDefaultAsync(o=> o.Id==id.Value);
            if (agenda == null)
            {
                return NotFound();
            }

            var model = new AgendaViewModel
            {
                Id =agenda.Id,
                Owners = _combosHelper.GetComboOwners(),
                Pets = _combosHelper.GetComboPets(0)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assing(AgendaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var agenda = await _context.Agendas.FindAsync(model.Id);
                if(agenda != null)
                {
                    agenda.IsAvailable = false;
                    agenda.Owner = await _context.Owners.FindAsync(model.OwnerId);
                    agenda.Pet = await _context.Pets.FindAsync(model.PetId);
                    agenda.Remarks = model.Remarks;
                    _context.Agendas.Update(agenda);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            model.Owners = _combosHelper.GetComboOwners();
            model.Pets = _combosHelper.GetComboPets(model.OwnerId);

            return View(model);
        }

        public async Task<JsonResult> GetPetsAsync(int? ownerId)
        {
            var pets = await _context.Pets
                .Where(p => p.Owner.Id == ownerId)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return Json(pets);
        }

        public async Task<IActionResult> Unassign(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agenda = await _context.Agendas
                .Include(a => a.Owner)
                .Include(a => a.Pet)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if(agenda== null)
            {
                return NotFound();
            }

            agenda.IsAvailable = true;
            agenda.Pet = null;
            agenda.Owner = null;
            agenda.Remarks = null;

            _context.Agendas.Update(agenda);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
