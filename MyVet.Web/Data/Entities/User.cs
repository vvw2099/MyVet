using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyVet.Web.Data.Entities
{
    public class User:IdentityUser
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(20, ErrorMessage = "El {0} campo no puede mas de {1} caracteres.")]
        [Display(Name = "Documento")]
        public string Document { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El {0} campo no puede mas de {1} caracteres.")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El {0} campo no puede mas de {1} caracteres.")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }
                
        [MaxLength(100, ErrorMessage = "El {0} campo no puede mas de {1} caracteres.")]
        [Display(Name = "Direccion")]
        public string Address { get; set; }
        [Display(Name = "Propietario")]
        public string FullName => $"{FirstName} {LastName}";
        [Display(Name = "Propietario")]
        public string FullNameWithDocument => $"{FirstName} {LastName} - {Document}";

    }
}
