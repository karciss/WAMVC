using System.ComponentModel.DataAnnotations;

namespace WAMVC.Models
{
    public class DetallePedidoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un pedido")]
        [Display(Name = "Pedido")]
        public int IdPedido { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un producto")]
        [Display(Name = "Producto")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, 100, ErrorMessage = "La cantidad debe estar entre 1 y 100")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "Precio unitario")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal PrecioUnitario { get; set; }

        
        public PedidoModel? Pedido { get; set; } // un pedido puede tener muchos detalles de pedido
        public ProductoModel? Producto { get; set; } // un producto puede estar en muchos detalles de pedido
    }
}
