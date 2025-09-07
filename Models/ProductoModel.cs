using System.ComponentModel.DataAnnotations;

namespace WAMVC.Models
{
    public class ProductoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre del producto")]
        public string Nombre { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, 99999, ErrorMessage = "El precio debe estar entre 0.01 y 99,999")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, 1000, ErrorMessage = "El stock debe estar entre 0 y 1000")]
        [Display(Name = "Stock disponible")]
        public int Stock { get; set; }

        public ICollection<DetallePedidoModel> DetallePedidos { get; set; } = new List<DetallePedidoModel>(); // un producto puede estar en muchos detalles de pedido
    }
}
