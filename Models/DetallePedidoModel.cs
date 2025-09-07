namespace WAMVC.Models
{
    public class DetallePedidoModel
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public PedidoModel Pedido { get; set; } // un pedido puede tener muchos detalles de pedido
        public ProductoModel Producto { get; set; } // un producto puede estar en muchos detalles de pedido
    }
}
