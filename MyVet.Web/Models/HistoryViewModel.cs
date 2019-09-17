using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyVet.Web.Data.Entities;

namespace MyVet.Web.Models
{
    public class HistoryViewModel : History
    {
        public int PetId { get; set; }

        [Display(Name ="Tipo de Servicio")]
        [Required(ErrorMessage ="El campo {0} es obligatorio.")]
        [Range(1,int.MaxValue, ErrorMessage ="Debe seleccionar un tipo de servicio")]
        public int ServiceTypeId { get; set; }

        public IEnumerable<SelectListItem> ServiceTypes { get; set; }
    }
}
