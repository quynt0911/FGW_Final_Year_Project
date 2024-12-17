using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blank.Models;
using Microsoft.AspNetCore.Authorization;

namespace Blank.Controllers
{
    public class IngredientController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FinalprojectContext _context;

        public IngredientController(FinalprojectContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }



        [Authorize(Roles = "Chef, Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ingredients.ToListAsync());
        }

        // GET: Ingredient/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(m => m.IngId == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        [Authorize(Roles = "Chef")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                if (ingredient.PhotoFile != null)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ingredient.PhotoFile.FileName);
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "ingredients");
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ingredient.PhotoFile.CopyToAsync(fileStream);
                    }

                    ingredient.PhotoUrl = $"/images/ingredients/{uniqueFileName}";
                }


                _context.Add(ingredient);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ingredient created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(ingredient);
        }



        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return View(ingredient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ingredient ingredient)
        {
            if (id != ingredient.IngId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ingredient.PhotoFile != null)
                    {
                        if (!string.IsNullOrEmpty(ingredient.PhotoUrl))
                        {
                            string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, ingredient.PhotoUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ingredient.PhotoFile.FileName);
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "ingredients");
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ingredient.PhotoFile.CopyToAsync(fileStream);
                        }

                        ingredient.PhotoUrl = $"/images/ingredients/{uniqueFileName}";
                    }


                    _context.Update(ingredient);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ingredient updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngredientExists(ingredient.IngId))
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
            return View(ingredient);
        }

        private bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(e => e.IngId == id);
        }




        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .FirstOrDefaultAsync(m => m.IngId == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        [Authorize(Roles = "Chef")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
