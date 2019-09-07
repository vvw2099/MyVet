using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MyVet.Web.Data.Entities;

namespace MyVet.Web.Data.Entities
{
    public class PetType
    {
        public int Id { get; set; }

        [Display(Name ="Tipo de Mascota")]
        [MaxLength(50,ErrorMessage = "El campo {0} es obligatorio")]
        [Required(ErrorMessage = "El {0} campo no puede mas de {1} caracteres.")]
        public string Name { get; set; }

        public ICollection<Pet> Pets { get; set; }
    }
}
