using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WAMVC.Data;
using WAMVC.Models;

namespace WAMVC.Data
{
     public static class SeedData
     {
         public static async Task InitializeAsync(IServiceProvider serviceProvider)
         {
             using var scope = serviceProvider.CreateScope();
             var context = scope.ServiceProvider.GetRequiredService<ArtesaniasDBContext>();

         try
         {
            await context.Database.MigrateAsync();
         }
         catch
         {
            context.Database.EnsureCreated();
         }

         var hasher = new PasswordHasher<Usuario>();

         // Create admin user
         if (!await context.Usuarios.AnyAsync(u => u.Email == "admin@test.com"))
         {
            var admin = new Usuario
         {
             Email = "admin@test.com",
             NombreCompleto = "Jose Enrique",
             Rol = "Admin",
             Activo = true
         };
         admin.Password = hasher.HashPassword(admin, "Admin123!");
         context.Usuarios.Add(admin);
         }

         // Create cliente user
         if (!await context.Usuarios.AnyAsync(u => u.Email == "cliente@test.com"))
         {
            var cliente = new Usuario
         {
            Email = "cliente@test.com",
            NombreCompleto = "Maria Perez",
            Rol = "Cliente",
            Activo = true
         };
         cliente.Password = hasher.HashPassword(cliente, "Cliente123!");
         context.Usuarios.Add(cliente);
         }

         // Create personal del local user
         if (!await context.Usuarios.AnyAsync(u => u.Email == "personal@test.com"))
         {
            var personal = new Usuario
         {
            Email = "personal@test.com",
            NombreCompleto = "Manuel Pinto",
            Rol = "Personal",
            Activo = true
         };
         personal.Password = hasher.HashPassword(personal, "Personal123!");
         context.Usuarios.Add(personal);
         }

         await context.SaveChangesAsync();
         }
         }
}
