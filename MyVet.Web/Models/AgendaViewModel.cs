using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyVet.Web.Data.Entities;

namespace MyVet.Web.Models
{
    public class AgendaViewModel : Agenda
    {
        [Required(ErrorMessage ="El campo {0} es obligatorio.")]
        [Display(Name ="Propietario")]
        [Range(1,int.MaxValue,ErrorMessage ="Debes seleccionar un propietario.")]
        public int OwnerId { get; set; }

        public IEnumerable<SelectListItem> Owners { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Mascota")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una mascota.")]

        public int PetId { get; set; }

        public IEnumerable<SelectListItem> Pets { get; set; }

        public bool IsMine { get; set; }
        public string Reserved => "Reserved";
    }
}
