using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MyVet.Web.Data.Entities
{
    public class Pet
    {
        public int Id { get; set; }

        [Display(Name = "Nombre de la Mascota")]
        [MaxLength(50, ErrorMessage = "El {0} campo no puede mas de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; }

        [Display(Name = "Foto de tu mascota")]
        public string ImageUrl { get; set; }

        [MaxLength(50, ErrorMessage = "El {0} campo no puede mas de {1} caracteres.")]
        public string Race { get; set; }

        [Display(Name = "Fecha de nacimiento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString ="{0:yyyy/MM/dd}",ApplyFormatInEditMode =true)]
        public DateTime Born { get; set; }

        public string Remarks { get; set; }

        //reemplazar la url correcta  por la imagen
        public string ImageFullPath => string.IsNullOrEmpty(ImageUrl) ? null : $"https://TBD.azurewebsites.net{ImageUrl.Substring(1)}";

        [Display(Name = "Fecha de nacimiento")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime BornLocal => Born.ToLocalTime();

        public PetType PetType { get; set; }

        public Owner Owner { get; set; }

        public ICollection<History> Histories { get; set; }
        public ICollection<Agenda> Agendas { get; set; }

    }
}
