using Blank.Models;
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
                    Status = group.FirstOrDefault().OStatus,  // Using the status of the first order in the group
                }).ToList();

            return View(groupedOrders);  // Pass the grouped data to the view
        }


        // GET: Create Order
        [HttpGet]
        public async Task<IActionResult> CreateOrder()
        {
            // Get all tables with the status "Reserved"
            ViewBag.ReservedTables = await _context.Tables
                .Where(t => t.TStatus == "Reserved")
                .ToListAsync();

            // Get all available dishes
            ViewBag.Dishes = await _context.Dishes.ToListAsync();

            return View();
        }

        // // POST: Create Order
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

            // Check if table exists and is reserved
            var table = await _context.Tables.FindAsync(tableId);
            if (table == null || table.TStatus != "Reserved")
            {
                TempData["Error"] = "Selected table is not reserved.";
                return RedirectToAction("CreateOrder");
            }

            decimal totalPrice = 0;

            // Loop through the selected dishes and quantities
            for (int i = 0; i < DishIds.Length; i++)
            {
                int dishId = DishIds[i];
                int quantity = Quantities[i];

                // Fetch the dish by its ID
                var dish = await _context.Dishes.FindAsync(dishId);
                if (dish != null)
                {
                    // Create a new order entry
                    var newOrder = new Order
                    {
                        DishId = dish.DishId,
                        TableId = tableId,
                        Quantity = quantity,
                        OStatus = "In Process"
                    };

                    // Calculate the total price for this dish
                    newOrder.CalculateTotalPrice();
                    totalPrice += newOrder.TotalPrice;

                    // Add the new order to the database
                    _context.Orders.Add(newOrder);
                }
                else
                {
                    TempData["Error"] = $"Dish with ID {dishId} not found.";
                    return RedirectToAction("CreateOrder");
                }
            }

            // Set the table status to "In Service"
            table.TStatus = "In Service";
            _context.Tables.Update(table);

            // Commit the transaction to the database
            await _context.SaveChangesAsync();

            TempData["Success"] = "Order successfully created.";
            return RedirectToAction("Index", "Order");
        }


        // POST: ChangeStatus
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int orderId, string status)
        {
            // Lấy order từ database
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
                // Cập nhật trạng thái
                order.OStatus = status;
                _context.Orders.Update(order);

                // Lưu thay đổi
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

    }
}
