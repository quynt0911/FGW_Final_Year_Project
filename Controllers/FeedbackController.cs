using Blank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;

namespace Blank.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly FinalprojectContext _context;

        public FeedbackController(FinalprojectContext context)
        {
            _context = context;
        }

        // GET: Feedback/Create
        public async Task<IActionResult> Create()
        {
            // Lấy danh sách các nhà hàng từ cơ sở dữ liệu
            ViewBag.Restaurants = await _context.Restaurants.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Feedback feedback, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "feedbacks");
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        feedback.ImagePath = "/images/feedbacks/" + uniqueFileName;
                    }

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _context.Users.FindAsync(userId);

                    feedback.CustomerId = userId;
                    feedback.CustomerName = user?.UserName; 

                    _context.Add(feedback);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Feedback submitted successfully.";
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "An error occurred: " + ex.Message;
                }
            }

            return View(feedback);
        }

        public async Task<IActionResult> Index()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Restaurant)
                .ToListAsync();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                foreach (var feedback in feedbacks)
                {
                    if (feedback.CustomerId == userId)
                    {
                        feedback.CustomerName = user?.UserName ?? "Unknown";
                    }
                }
            }

            return View(feedbacks);
        }
    }
}
