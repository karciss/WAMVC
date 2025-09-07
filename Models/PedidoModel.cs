using System.ComponentModel.DataAnnotations;

namespace WAMVC.Models
{
    public class PedidoModel
    {
        public int Id { get; set; }

        [Display(Name = "Fecha del pedido")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "La fecha del pedido es obligatoria")]
        public DateTime FechaPedido { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Display(Name = "Estado del pedido")]
        public string Estado { get; set; } = "Pendiente";

        [Display(Name = "Total")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "El monto total debe ser mayor o igual a 0")]
        public decimal MontoTotal { get; set; } = 0;

        public ClienteModel? Cliente { get; set; } //un pedido pertenece a un cliente
        public ICollection<DetallePedidoModel> DetallePedidos { get; set; } = new List<DetallePedidoModel>(); //un pedido tiene muchos detalles de pedido
    }
}
