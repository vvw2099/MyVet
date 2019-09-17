using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MyVet.Web.Models
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Display(Name ="Documento")]
        [MaxLength(20,ErrorMessage ="El campo {0} no puede tener mas de {1} caracteres")]
        [Required(ErrorMessage ="El campo {0} es obligatorio")]
        public string Document { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [Display(Name = "Apellido")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string LastName { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [Display(Name = "Dirección")]
        public string Address { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; }
    }
}
