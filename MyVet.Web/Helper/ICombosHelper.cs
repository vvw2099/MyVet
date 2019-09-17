using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyVet.Web.Helper
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetComboPetTypes();
        IEnumerable<SelectListItem> GetComboServiceTypes();
    }
}
