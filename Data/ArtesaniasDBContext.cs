using Microsoft.EntityFrameworkCore;
using WAMVC.Models;
namespace WAMVC.Data
{
    public class ArtesaniasDBContext : DbContext
    {
        public ArtesaniasDBContext(DbContextOptions<ArtesaniasDBContext> options) : base(options)
        {
        }
        public DbSet<ProductoModel> Productos { get; set; }
        public DbSet<ClienteModel> Clientes { get; set; }
        public DbSet<PedidoModel> Pedidos { get; set; }
        public DbSet<DetallePedidoModel> DetallePedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //un pedido pertence a un cliente y un cliente puede tener muchos pedidos
            modelBuilder.Entity<PedidoModel>()
                .HasOne(p => p.Cliente) //un pedido pertenece a un cliente
                .WithMany(c => c.Pedidos) //un cliente puede tener muchos pedidos
                .HasForeignKey(p => p.IdCliente); //clave foranea en la tabla pedidos

            //un pedido tiene muchos detalles de pedido 
            modelBuilder.Entity<PedidoModel>()
                .HasMany(p => p.DetallePedidos) //un pedido tiene muchos detalles de pedido
                .WithOne(dp => dp.Pedido) //un detalle de pedido pertenece a un pedido
                .HasForeignKey(dp => dp.IdPedido); //clave foranea en la tabla detalles de pedido

            //un detallePedido tiene un producto y un producto puede estar en muchos detalles de pedido
            modelBuilder.Entity<DetallePedidoModel>()
                .HasOne(dp => dp.Producto) //un detalle de pedido tiene un producto
                .WithMany(p => p.DetallePedidos) //un producto puede estar en muchos detalles de pedido
                .HasForeignKey(dp => dp.IdProducto); //clave foranea en la tabla detalles de pedido
                
            // Configurar precisión para campos decimales
            modelBuilder.Entity<ProductoModel>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<DetallePedidoModel>()
                .Property(d => d.PrecioUnitario)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<PedidoModel>()
                .Property(p => p.MontoTotal)
                .HasColumnType("decimal(18,2)");
        }
    }
}
