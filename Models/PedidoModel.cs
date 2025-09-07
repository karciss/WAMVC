namespace WAMVC.Models
{
    public class PedidoModel
    {
        public int Id { get; set; }
        public DateTime FechaPedido { get; set; }
        public int IdCliente { get; set; }
        public string Estado { get; set; }
        public decimal MontoTotal { get; set; }
        public ClienteModel Cliente { get; set; } //un pedido pertenece a un cliente
        public ICollection<DetallePedidoModel> DetallePedidos { get; set; } //un pedido tiene muchos detalles de pedido
    }
}
