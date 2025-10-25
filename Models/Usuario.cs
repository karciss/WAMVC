using System.ComponentModel.DataAnnotations;

namespace WAMVC.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }  = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = "Usuario"; // Usuario o Administrador

        public bool Activo { get; set; } = true;
    }
}
