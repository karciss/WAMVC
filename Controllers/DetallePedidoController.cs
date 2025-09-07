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
            var artesaniasDBContext = _context.DetallePedidos.Include(d => d.Pedido).Include(d => d.Producto);
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
        public IActionResult Create()
        {
            ViewData["IdPedido"] = new SelectList(_context.Pedidos, "Id", "Id");
            ViewData["IdProducto"] = new SelectList(_context.Productos, "Id", "Id");
            return View();
        }

        // POST: DetallePedido/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdPedido,IdProducto,Cantidad,PrecioUnitario")] DetallePedidoModel detallePedidoModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detallePedidoModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPedido"] = new SelectList(_context.Pedidos, "Id", "Id", detallePedidoModel.IdPedido);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "Id", "Id", detallePedidoModel.IdProducto);
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
            ViewData["IdPedido"] = new SelectList(_context.Pedidos, "Id", "Id", detallePedidoModel.IdPedido);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "Id", "Id", detallePedidoModel.IdProducto);
            return View(detallePedidoModel);
        }

        // POST: DetallePedido/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdPedido,IdProducto,Cantidad,PrecioUnitario")] DetallePedidoModel detallePedidoModel)
        {
            if (id != detallePedidoModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detallePedidoModel);
                    await _context.SaveChangesAsync();
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
            ViewData["IdPedido"] = new SelectList(_context.Pedidos, "Id", "Id", detallePedidoModel.IdPedido);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "Id", "Id", detallePedidoModel.IdProducto);
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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetallePedidoModelExists(int id)
        {
            return _context.DetallePedidos.Any(e => e.Id == id);
        }
    }
}
