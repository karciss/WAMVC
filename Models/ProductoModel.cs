namespace WAMVC.Models
{
    public class ProductoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        public ICollection<DetallePedidoModel> DetallePedidos { get; set; } // un producto puede estar en muchos detalles de pedido
    }
}
