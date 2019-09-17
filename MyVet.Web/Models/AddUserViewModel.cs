using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MyVet.Web.Models
{
    public class AddUserViewModel : EditUserViewModel
    {
        [Display(Name ="Correo Electrónico")]
        [Required(ErrorMessage ="El campo {0} es obligatorio.")]
        [MaxLength(100,ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [EmailAddress]
        public string Username { get; set; }

        

        [Display(Name ="Contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirmación Contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }

    }
}
