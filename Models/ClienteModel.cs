using System.ComponentModel.DataAnnotations;

namespace WAMVC.Models
{
    public class ClienteModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los {1} caracteres")]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        [StringLength(150, ErrorMessage = "El email no puede exceder los {1} caracteres")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede exceder los {1} caracteres")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        public ICollection<PedidoModel> Pedidos { get; set; } = new List<PedidoModel>(); // Navegación a los pedidos del cliente
    }
}
