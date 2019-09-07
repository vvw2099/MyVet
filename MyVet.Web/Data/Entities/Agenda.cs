using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MyVet.Web.Data.Entities
{
    public class Agenda
    {
        public int Id { get; set; }
        [Display(Name = "Fecha")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd H:mm tt}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "El  campo {0} es obligatorio.")]
        public DateTime Date { get; set; }
        public string Remarks { get; set; }
        [Display(Name ="Disponible")]
        public bool IsAvailable { get; set; }
        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd H:mm tt}")]
        public DateTime DateLocal => Date.ToLocalTime();

        public Owner Owner { get; set; }
        public Pet Pet { get; set; }
    }
}
