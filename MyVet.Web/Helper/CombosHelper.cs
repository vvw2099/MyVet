using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyVet.Web.Data;

namespace MyVet.Web.Helper
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _dataContext;

        public CombosHelper(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
               
        public IEnumerable<SelectListItem> GetComboPetTypes()
        {
            var list = _dataContext.PetTypes.Select(pt => new SelectListItem
            {
                Text = pt.Name,
                Value = $"{pt.Id}"
            })
                .OrderBy(pt=>pt.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Tipo de Mascota...]",
                Value = "0"
            });

            return list;
        }
        public IEnumerable<SelectListItem> GetComboServiceTypes()
        {
            var list = _dataContext.ServiceTypes.Select(st => new SelectListItem
            {
                Text = st.Name,
                Value = $"{st.Id}"
            })
                .OrderBy(st => st.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Tipo de Servicio]",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboOwners()
        {
            var list = _dataContext.Owners.Select(p => new SelectListItem
            {
                Text = p.user.FullNameWithDocument,
                Value = p.Id.ToString()
            }).OrderBy(p => p.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "Seleccione un  Propietario",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboPets(int ownerId)
        {
            var list = _dataContext.Pets.Where(p => p.Owner.Id == ownerId).Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).OrderBy(p => p.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "Seleccione una mascota",
                Value = "0"
            });

            return list;
        }
    }
}
