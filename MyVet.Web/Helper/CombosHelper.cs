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
    }
}
