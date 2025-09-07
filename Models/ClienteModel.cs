using System.ComponentModel.DataAnnotations;

namespace WAMVC.Models
{
    public class ClienteModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        public ICollection<PedidoModel> Pedidos { get; set; } = new List<PedidoModel>(); // Navegación a los pedidos del cliente
    }
}
