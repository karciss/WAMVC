namespace WAMVC.Models
{
    public class ClienteModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public ICollection<PedidoModel> Pedidos { get; set; } // Navegación a los pedidos del cliente
    }
}
