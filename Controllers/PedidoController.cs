using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WAMVC.Data;
using WAMVC.Models;

namespace WAMVC.Controllers
{
    public class PedidoController : Controller
    {
        private readonly ArtesaniasDBContext _context;

        public PedidoController(ArtesaniasDBContext context)
        {
            _context = context;
        }

        // GET: Pedido
        public async Task<IActionResult> Index()
        {
            var artesaniasDBContext = _context.Pedidos.Include(p => p.Cliente);
            return View(await artesaniasDBContext.ToListAsync());
        }

        // Acción para crear datos de prueba
        public async Task<IActionResult> CrearDatosPrueba()
        {
            try
            {
                // Verificar si ya hay datos
                if (await _context.Clientes.AnyAsync())
                {
                    TempData["InfoMessage"] = "Ya existen datos en la base de datos.";
                    return RedirectToAction("Index");
                }

                // Crear clientes de prueba
                var clientes = new List<ClienteModel>
                {
                    new ClienteModel { Nombre = "Juan Pérez", Email = "juan@email.com", Direccion = "Calle 123, Ciudad" },
                    new ClienteModel { Nombre = "María García", Email = "maria@email.com", Direccion = "Avenida 456, Ciudad" },
                    new ClienteModel { Nombre = "Carlos López", Email = "carlos@email.com", Direccion = "Plaza 789, Ciudad" }
                };

                _context.Clientes.AddRange(clientes);
                await _context.SaveChangesAsync();

                // Crear productos de prueba
                var productos = new List<ProductoModel>
                {
                    new ProductoModel { Nombre = "Artesanía 1", Descripcion = "Producto artesanal hecho a mano", Precio = 25.50m, Stock = 10 },
                    new ProductoModel { Nombre = "Artesanía 2", Descripcion = "Decoración tradicional", Precio = 45.00m, Stock = 5 },
                    new ProductoModel { Nombre = "Artesanía 3", Descripcion = "Utensilio decorativo", Precio = 15.75m, Stock = 20 }
                };

                _context.Productos.AddRange(productos);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Datos de prueba creados correctamente. Ahora puede crear pedidos.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al crear datos de prueba: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Pedido/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedidoModel = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.DetallePedidos)
                    .ThenInclude(dp => dp.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (pedidoModel == null)
            {
                return NotFound();
            }

            return View(pedidoModel);
        }

        // GET: Pedido/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                // Verificar que hay clientes disponibles
                var clientes = await _context.Clientes.ToListAsync();
                if (!clientes.Any())
                {
                    TempData["ErrorMessage"] = "No hay clientes disponibles. Debe crear al menos un cliente antes de crear un pedido.";
                    TempData["ShowCreateTestData"] = true;
                    return RedirectToAction("Index");
                }

                // Usar el nombre del cliente en lugar del ID para mejor usabilidad
                ViewData["IdCliente"] = new SelectList(clientes, "Id", "Nombre");
                
                // Crear un nuevo pedido con valores predeterminados
                var nuevoPedido = new PedidoModel
                {
                    FechaPedido = DateTime.Now,
                    Estado = "Pendiente",
                    MontoTotal = 0
                };
                
                return View(nuevoPedido);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la página de creación: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Pedido/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FechaPedido,IdCliente,Estado,MontoTotal")] PedidoModel pedidoModel)
        {
            try
            {
                // Verificar que el cliente existe
                if (pedidoModel.IdCliente > 0)
                {
                    var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == pedidoModel.IdCliente);
                    if (!clienteExiste)
                    {
                        ModelState.AddModelError("IdCliente", "El cliente seleccionado no existe.");
                    }
                }

                // Log para depurar
                System.Diagnostics.Debug.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
                System.Diagnostics.Debug.WriteLine($"Cliente ID: {pedidoModel.IdCliente}");
                System.Diagnostics.Debug.WriteLine($"Fecha: {pedidoModel.FechaPedido}");
                System.Diagnostics.Debug.WriteLine($"Estado: {pedidoModel.Estado}");
                System.Diagnostics.Debug.WriteLine($"Monto: {pedidoModel.MontoTotal}");

                if (ModelState.IsValid)
                {
                    _context.Add(pedidoModel);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Pedido #{pedidoModel.Id} creado correctamente. Ahora puede agregar productos.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Log errores de validación
                    foreach (var error in ModelState)
                    {
                        System.Diagnostics.Debug.WriteLine($"Campo: {error.Key}");
                        foreach (var errorMsg in error.Value.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"  Error: {errorMsg.ErrorMessage}");
                        }
                    }
                    TempData["ErrorMessage"] = "Hay errores en el formulario. Por favor revise los campos.";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el pedido: " + ex.Message);
                TempData["ErrorMessage"] = "Error al crear el pedido: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
            
            // Si llegamos aquí, algo salió mal - recargar la vista
            var clientes = await _context.Clientes.ToListAsync();
            ViewData["IdCliente"] = new SelectList(clientes, "Id", "Nombre", pedidoModel.IdCliente);
            return View(pedidoModel);
        }

        // GET: Pedido/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedidoModel = await _context.Pedidos.FindAsync(id);
            if (pedidoModel == null)
            {
                return NotFound();
            }
            
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "Id", "Nombre", pedidoModel.IdCliente);
            
            // Lista de estados posibles para el pedido
            ViewData["Estados"] = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "En proceso", Text = "En proceso" },
                new SelectListItem { Value = "Completado", Text = "Completado" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" }
            };
            
            return View(pedidoModel);
        }

        // POST: Pedido/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaPedido,IdCliente,Estado,MontoTotal")] PedidoModel pedidoModel)
        {
            if (id != pedidoModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedidoModel);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Pedido actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoModelExists(pedidoModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "Id", "Nombre", pedidoModel.IdCliente);
            ViewData["Estados"] = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "En proceso", Text = "En proceso" },
                new SelectListItem { Value = "Completado", Text = "Completado" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" }
            };
            
            return View(pedidoModel);
        }

        // GET: Pedido/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedidoModel = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (pedidoModel == null)
            {
                return NotFound();
            }

            return View(pedidoModel);
        }

        // POST: Pedido/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Primero verificar si hay detalles de pedido relacionados
            var tieneDetalles = await _context.DetallePedidos.AnyAsync(dp => dp.IdPedido == id);
            
            if (tieneDetalles)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el pedido porque tiene detalles asociados. Elimine primero los detalles.";
                return RedirectToAction(nameof(Index));
            }
            
            var pedidoModel = await _context.Pedidos.FindAsync(id);
            if (pedidoModel != null)
            {
                _context.Pedidos.Remove(pedidoModel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Pedido eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PedidoModelExists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }
    }
}
