using Blank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Dish)
                .Include(o => o.Table)
                .ToListAsync();

            var groupedOrders = orders
                .GroupBy(o => o.TableId)
                .Select(group => new
                {
                    Table = group.FirstOrDefault().Table.TName,
                    DishesAndQuantities = group.Select(o => new
                    {

                        DishName = o.Dish.DishName,
                        Quantity = o.Quantity,
                        ToPrice = o.Dish.Price * o.Quantity
                    }).ToList(),
                    TotalPrice = group.Sum(o => o.Dish.Price * o.Quantity),
                    Status = group.FirstOrDefault().OStatus,
                }).ToList();

            return View(groupedOrders); 
        }


        [Authorize(Roles = "Staff")]
        [HttpGet]
        public async Task<IActionResult> CreateOrder()
        {
            ViewBag.ReservedTables = await _context.Tables
                .Where(t => t.TStatus == "Reserved")
                .ToListAsync();

            ViewBag.Dishes = await _context.Dishes.ToListAsync();

            return View();
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(int tableId, int[] DishIds, int[] Quantities)
        {

            System.Diagnostics.Debug.WriteLine($"TableId: {tableId}");
            System.Diagnostics.Debug.WriteLine($"DishIds: {string.Join(", ", DishIds)}");
            System.Diagnostics.Debug.WriteLine($"Quantities: {string.Join(", ", Quantities)}");

            if (DishIds == null || DishIds.Length == 0 || Quantities == null || Quantities.Length == 0)
            {
                TempData["Error"] = "You must select at least one dish and specify a quantity.";
                return RedirectToAction("CreateOrder");
            }

            if (DishIds.Length != Quantities.Length)
            {
                TempData["Error"] = "Dish selection and quantity must match.";
                return RedirectToAction("CreateOrder");
            }

            var table = await _context.Tables.FindAsync(tableId);
            if (table == null || table.TStatus != "Reserved")
            {
                TempData["Error"] = "Selected table is not reserved.";
                return RedirectToAction("CreateOrder");
            }

            decimal totalPrice = 0;

            for (int i = 0; i < DishIds.Length; i++)
            {
                int dishId = DishIds[i];
                int quantity = Quantities[i];

                var dish = await _context.Dishes.FindAsync(dishId);
                if (dish != null)
                {
                    var newOrder = new Order
                    {
                        DishId = dish.DishId,
                        TableId = tableId,
                        Quantity = quantity,
                        OStatus = "In Process"
                    };

                    newOrder.CalculateTotalPrice();
                    totalPrice += newOrder.TotalPrice;

                    _context.Orders.Add(newOrder);
                }
                else
                {
                    TempData["Error"] = $"Dish with ID {dishId} not found.";
                    return RedirectToAction("CreateOrder");
                }
            }

            table.TStatus = "In Service";
            _context.Tables.Update(table);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Order successfully created.";
            return RedirectToAction("Index", "Order");
        }


        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);

            ViewBag.OStatusList = new SelectList(new[]
    {
        "In Process",
        "Serving",
        "Completed",
        "Prepared",
        "Cancelled"
    }, order.OStatus);

            if (order != null)
            {
                order.OStatus = status;
                _context.Orders.Update(order);

                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

    }
}
