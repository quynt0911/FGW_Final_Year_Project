using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blank.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.Threading.Tasks;

namespace Blank.Controllers
{
    public class OrderController : Controller
    {
        private readonly FinalprojectContext _context;

        public OrderController(FinalprojectContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Dish) 
                .Include(o => o.Table) 
                .ToListAsync();

            return View(orders);
        }

        // GET: Order/Create
        public IActionResult Create(int dishId)
        {
            ViewData["DishId"] = dishId;
            ViewData["TableList"] = new SelectList(_context.Tables, "TableId", "TableName");
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DishId,TableId,OStatus")] Order order, int quantity)
        {
            if (ModelState.IsValid)
            {
                order.OStatus = "Pending"; 
                _context.Add(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["TableList"] = new SelectList(_context.Tables, "TableId", "TableName", order.TableId);
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            ViewData["TableList"] = new SelectList(_context.Tables, "TableId", "TableName", order.TableId);
            return View(order);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,DishId,TableId,OStatus")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Order updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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

            ViewData["TableList"] = new SelectList(_context.Tables, "TableId", "TableName", order.TableId);
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Dish)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
