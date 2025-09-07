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
    public class DetallePedidoController : Controller
    {
        private readonly ArtesaniasDBContext _context;

        public DetallePedidoController(ArtesaniasDBContext context)
        {
            _context = context;
        }

        // GET: DetallePedido
        public async Task<IActionResult> Index()
        {
            var artesaniasDBContext = _context.DetallePedidos
                .Include(d => d.Pedido)
                .Include(d => d.Producto);
            return View(await artesaniasDBContext.ToListAsync());
        }

        // GET: DetallePedido/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedidoModel = await _context.DetallePedidos
                .Include(d => d.Pedido)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detallePedidoModel == null)
            {
                return NotFound();
            }

            return View(detallePedidoModel);
        }

        // GET: DetallePedido/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                // Verificar que hay pedidos disponibles
                var pedidos = await _context.Pedidos.ToListAsync();
                if (!pedidos.Any())
                {
                    TempData["ErrorMessage"] = "No hay pedidos disponibles. Debe crear al menos un pedido antes de crear un detalle.";
                    return RedirectToAction("Index");
                }

                // Verificar que hay productos disponibles
                var productos = await _context.Productos.ToListAsync();
                if (!productos.Any())
                {
                    TempData["ErrorMessage"] = "No hay productos disponibles. Debe crear al menos un producto antes de crear un detalle.";
                    return RedirectToAction("Index");
                }

                // Crear SelectList para pedidos con información más descriptiva
                ViewData["IdPedido"] = new SelectList(
                    pedidos.Select(p => new { p.Id, Display = $"Pedido #{p.Id} - {p.FechaPedido:dd/MM/yyyy}" }),
                    "Id", "Display"
                );

                // Crear SelectList para productos
                ViewData["IdProducto"] = new SelectList(productos, "Id", "Nombre");

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la página de creación: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: DetallePedido/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPedido,IdProducto,Cantidad")] DetallePedidoModel detallePedidoModel)
        {
            try
            {
                // Remover validaciones de las propiedades de navegación que no se van a enlazar
                ModelState.Remove("Pedido");
                ModelState.Remove("Producto");
                ModelState.Remove("PrecioUnitario"); // Lo asignaremos manualmente

                // Log para depurar
                System.Diagnostics.Debug.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
                System.Diagnostics.Debug.WriteLine($"Pedido ID: {detallePedidoModel.IdPedido}");
                System.Diagnostics.Debug.WriteLine($"Producto ID: {detallePedidoModel.IdProducto}");
                System.Diagnostics.Debug.WriteLine($"Cantidad: {detallePedidoModel.Cantidad}");

                // Verificaciones adicionales
                if (detallePedidoModel.IdPedido > 0)
                {
                    var pedidoExiste = await _context.Pedidos.AnyAsync(p => p.Id == detallePedidoModel.IdPedido);
                    if (!pedidoExiste)
                    {
                        ModelState.AddModelError("IdPedido", "El pedido seleccionado no existe.");
                    }
                }
                else
                {
                    ModelState.AddModelError("IdPedido", "Debe seleccionar un pedido.");
                }

                if (detallePedidoModel.IdProducto > 0)
                {
                    var productoExiste = await _context.Productos.AnyAsync(p => p.Id == detallePedidoModel.IdProducto);
                    if (!productoExiste)
                    {
                        ModelState.AddModelError("IdProducto", "El producto seleccionado no existe.");
                    }
                }
                else
                {
                    ModelState.AddModelError("IdProducto", "Debe seleccionar un producto.");
                }

                if (detallePedidoModel.Cantidad < 1 || detallePedidoModel.Cantidad > 100)
                {
                    ModelState.AddModelError("Cantidad", "La cantidad debe estar entre 1 y 100.");
                }

                // Asignar el precio unitario desde el producto
                if (detallePedidoModel.IdProducto > 0)
                {
                    var producto = await _context.Productos.FindAsync(detallePedidoModel.IdProducto);
                    if (producto != null)
                    {
                        detallePedidoModel.PrecioUnitario = producto.Precio;
                        System.Diagnostics.Debug.WriteLine($"Precio asignado: {detallePedidoModel.PrecioUnitario}");
                    }
                    else
                    {
                        ModelState.AddModelError("IdProducto", "No se pudo obtener el precio del producto.");
                    }
                }

                if (ModelState.IsValid)
                {
                    _context.Add(detallePedidoModel);
                    await _context.SaveChangesAsync();
                    
                    var producto = await _context.Productos.FindAsync(detallePedidoModel.IdProducto);
                    TempData["SuccessMessage"] = $"Detalle de pedido creado correctamente. Pedido #{detallePedidoModel.IdPedido}, Producto: {producto?.Nombre}, Cantidad: {detallePedidoModel.Cantidad}, Precio: ${detallePedidoModel.PrecioUnitario}";
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
                ModelState.AddModelError("", "Error al crear el detalle de pedido: " + ex.Message);
                TempData["ErrorMessage"] = "Error al crear el detalle de pedido: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }

            // Si llegamos aquí, algo salió mal - recargar la vista
            var pedidos = await _context.Pedidos.ToListAsync();
            var productos = await _context.Productos.ToListAsync();
            
            ViewData["IdPedido"] = new SelectList(
                pedidos.Select(p => new { p.Id, Display = $"Pedido #{p.Id} - {p.FechaPedido:dd/MM/yyyy}" }),
                "Id", "Display", detallePedidoModel.IdPedido
            );
            ViewData["IdProducto"] = new SelectList(productos, "Id", "Nombre", detallePedidoModel.IdProducto);
            
            return View(detallePedidoModel);
        }

        // GET: DetallePedido/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedidoModel = await _context.DetallePedidos.FindAsync(id);
            if (detallePedidoModel == null)
            {
                return NotFound();
            }
            
            var pedidos = await _context.Pedidos.ToListAsync();
            var productos = await _context.Productos.ToListAsync();
            
            ViewData["IdPedido"] = new SelectList(
                pedidos.Select(p => new { p.Id, Display = $"Pedido #{p.Id} - {p.FechaPedido:dd/MM/yyyy}" }),
                "Id", "Display", detallePedidoModel.IdPedido
            );
            ViewData["IdProducto"] = new SelectList(productos, "Id", "Nombre", detallePedidoModel.IdProducto);
            
            return View(detallePedidoModel);
        }

        // POST: DetallePedido/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdPedido,IdProducto,Cantidad,PrecioUnitario")] DetallePedidoModel detallePedidoModel)
        {
            if (id != detallePedidoModel.Id)
            {
                return NotFound();
            }

            // Remover validaciones de las propiedades de navegación
            ModelState.Remove("Pedido");
            ModelState.Remove("Producto");

            if (ModelState.IsValid)
            {
                try
                {
                    // Si el producto cambió, actualizar el precio unitario
                    var detalleOriginal = await _context.DetallePedidos.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
                    if (detalleOriginal != null && detalleOriginal.IdProducto != detallePedidoModel.IdProducto)
                    {
                        var producto = await _context.Productos.FindAsync(detallePedidoModel.IdProducto);
                        if (producto != null)
                        {
                            detallePedidoModel.PrecioUnitario = producto.Precio;
                        }
                    }
                    
                    _context.Update(detallePedidoModel);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Detalle de pedido actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetallePedidoModelExists(detallePedidoModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            var pedidos = await _context.Pedidos.ToListAsync();
            var productos = await _context.Productos.ToListAsync();
            
            ViewData["IdPedido"] = new SelectList(
                pedidos.Select(p => new { p.Id, Display = $"Pedido #{p.Id} - {p.FechaPedido:dd/MM/yyyy}" }),
                "Id", "Display", detallePedidoModel.IdPedido
            );
            ViewData["IdProducto"] = new SelectList(productos, "Id", "Nombre", detallePedidoModel.IdProducto);
            
            return View(detallePedidoModel);
        }

        // GET: DetallePedido/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedidoModel = await _context.DetallePedidos
                .Include(d => d.Pedido)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detallePedidoModel == null)
            {
                return NotFound();
            }

            return View(detallePedidoModel);
        }

        // POST: DetallePedido/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detallePedidoModel = await _context.DetallePedidos.FindAsync(id);
            if (detallePedidoModel != null)
            {
                _context.DetallePedidos.Remove(detallePedidoModel);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Detalle de pedido eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DetallePedidoModelExists(int id)
        {
            return _context.DetallePedidos.Any(e => e.Id == id);
        }
    }
}
