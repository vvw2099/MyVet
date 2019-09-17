using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyVet.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyVet.Web.Models
{
    public class PetViewModel:Pet
    {
        public int OwnerId { get; set; }

        [Required(ErrorMessage ="El campo {0} es obligatorio.")]
        [Display(Name ="Tipo de Mascota")]
        [Range(1,int.MaxValue, ErrorMessage = "Debes seleccionar un tipo de mascota")]
        public int PetTypeId { get; set; }

        [Display(Name ="Foto de la Mascota")]
        public IFormFile ImageFile { get; set; }
        public IEnumerable<SelectListItem> PetTypes { get; set; }
    }
}
