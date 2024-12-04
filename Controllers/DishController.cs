using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blank.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Blank.Controllers
{
    public class DishController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FinalprojectContext _context;

        public DishController(FinalprojectContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Dish
        public async Task<IActionResult> Index()
        {
            return View(await _context.Dishes.ToListAsync());
        }

        // GET: Dish/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes.FirstOrDefaultAsync(m => m.DishId == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // GET: Dish/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DishId,DishName,DDescription,Price,PhotoFile")] Dish dish)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (dish.PhotoFile != null)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(dish.PhotoFile.FileName);
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "dishes");
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await dish.PhotoFile.CopyToAsync(fileStream);
                        }

                        dish.PhotoUrl = $"/images/dishes/{uniqueFileName}";
                    }

                    _context.Add(dish);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Dish created successfully."; 
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                }
            }
            return View(dish);
        }

        // GET: Dish/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NotFound();
            }
            return View(dish);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DishId,DishName,DDescription,Price,PhotoFile,PhotoUrl")] Dish dish)
        {
            if (id != dish.DishId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (dish.PhotoFile != null)
                    {
                        if (!string.IsNullOrEmpty(dish.PhotoUrl))
                        {
                            string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, dish.PhotoUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(dish.PhotoFile.FileName);
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "dishes");
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await dish.PhotoFile.CopyToAsync(fileStream);
                        }

                        dish.PhotoUrl = $"/images/dishes/{uniqueFileName}";
                    }

                    _context.Update(dish);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Dish updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.DishId))
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
            return View(dish);
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.DishId == id);
        }


        // GET: Dish/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Invalid dish ID.";
                return RedirectToAction(nameof(Index));
            }

            var dish = await _context.Dishes.FirstOrDefaultAsync(m => m.DishId == id);
            if (dish == null)
            {
                TempData["ErrorMessage"] = "Dish not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(dish);
        }

        // POST: Dish/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var dish = await _context.Dishes.FindAsync(id);
                if (dish != null)
                {
                    if (!string.IsNullOrEmpty(dish.PhotoUrl))
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, dish.PhotoUrl.TrimStart('/'));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }

                    _context.Dishes.Remove(dish);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Dish deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Dish not found.";
                }
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Unable to delete this dish at this time. Please check the related orders.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the dish.";
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Menu(string sortOrder, string searchString)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParam"] = sortOrder == "Price" ? "price_desc" : "Price";

            var dishes = from d in _context.Dishes select d;

            if (!String.IsNullOrEmpty(searchString))
            {
                dishes = dishes.Where(d => d.DishName.Contains(searchString) || d.DDescription.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    dishes = dishes.OrderByDescending(d => d.DishName);
                    break;
                case "Price":
                    dishes = dishes.OrderBy(d => d.Price);
                    break;
                case "price_desc":
                    dishes = dishes.OrderByDescending(d => d.Price);
                    break;
                default:
                    dishes = dishes.OrderBy(d => d.DishName);
                    break;
            }

            return View(await dishes.AsNoTracking().ToListAsync());
        }



        public async Task<IActionResult> AddToOrder(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes.FirstOrDefaultAsync(m => m.DishId == id);
            if (dish == null)
            {
                return NotFound();
            }

            ViewData["TableList"] = new SelectList(_context.Tables, "TableId", "TableName");
            return View(dish);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToOrder(int id, int quantity, int tableId)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NotFound();
            }

            var order = new Order
            {
                DishId = id,
                Quantity = quantity,
                TableId = tableId,
                TotalPrice = dish.Price * quantity
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Dish added to order successfully.";
            return RedirectToAction(nameof(Menu));
        }

    }
}
