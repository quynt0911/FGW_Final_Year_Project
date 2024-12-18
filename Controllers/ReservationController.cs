using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blank.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Task = System.Threading.Tasks.Task;

namespace Blank.Controllers
{
    public class ReservationController : Controller
    {
        private readonly FinalprojectContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReservationController(FinalprojectContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string status = "Pending")
        {
            var reservations = await _context.Reservations
                .Include(r => r.Table)
                .Include(r => r.Restaurant)
                .Where(r => r.RStatus == status)
                .ToListAsync();

            return View(reservations);
        }

        [HttpGet]
        public async Task<IActionResult> MakeReservation()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "You must be logged in to make a reservation.";
                return Unauthorized("User is not logged in.");
            }

            ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
            ViewBag.Tables = await _context.Tables.Where(t => t.TStatus == "Available").ToListAsync();
            ViewData["CustomerId"] = userId;
            DateTime currentDateTime = DateTime.Now;
            currentDateTime = new DateTime(
                currentDateTime.Year,
                currentDateTime.Month,
                currentDateTime.Day,
                currentDateTime.Hour,
                currentDateTime.Minute,
                0
            );

            var reservation = new Reservation
            {
                CustomerId = userId,
                CustomerName = User.Identity.Name,
                DateTime = currentDateTime
            };

            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeReservation([Bind("RestaurantId,DateTime,TableId,Request,CustomerName")] Reservation reservation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "You must be logged in to make a reservation.";
                return Unauthorized("User is not logged in.");
            }
            ModelState.Remove("CustomerId");
            reservation.CustomerId = userId;
            reservation.RStatus = "Pending";

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid input. Please check your form fields.";
                await LoadViewDataAsync();
                return View(reservation);
            }

            if (!reservation.TableId.HasValue)
            {
                TempData["Error"] = "No table selected. Please select a table.";
                await LoadViewDataAsync();
                return View(reservation);
            }

            var selectedTable = await _context.Tables.FindAsync(reservation.TableId.Value);
            if (selectedTable == null || selectedTable.TStatus != "Available")
            {
                TempData["Error"] = "Selected table is not available. Please choose another table.";
                await LoadViewDataAsync();
                return View(reservation);
            }

            selectedTable.TStatus = "Reserved";

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                _context.Add(reservation);
                _context.Tables.Update(selectedTable);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Reservation successfully made.";
                return RedirectToAction("History", "Reservation");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while saving the reservation: {ex.Message}";
                Console.WriteLine($"Error while saving reservation: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            await LoadViewDataAsync();
            return View(reservation);
        }

        private async Task LoadViewDataAsync()
        {
            ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
            ViewBag.Tables = await _context.Tables.Where(t => t.TStatus == "Available").ToListAsync();
        }

        private async Task LoadViewBags()
        {
            ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
            ViewBag.Tables = await _context.Tables.Where(t => t.TStatus == "Available").ToListAsync();
        }

        public IActionResult ReservationConfirmation()
        {
            return View();
        }

        [HttpGet("Reservation/DetailsApi/{id}")]
        public async Task<IActionResult> DetailsApi(int id)
        {
            try
            {
                var table = await _context.Tables.FindAsync(id);
                if (table == null)
                {
                    return NotFound(new { success = false, message = "Table not found" });
                }

                return Json(new
                {
                    tName = table.TName,
                    location = table.Location,
                    tStatus = table.TStatus
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.RStatus = "Approved";
            _context.Update(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RejectReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.ReserId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            reservation.RStatus = "Rejected";

            if (reservation.Table != null)
            {
                reservation.Table.TStatus = "Available";
                _context.Tables.Update(reservation.Table);
            }

            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Details(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Restaurant)
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.ReserId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }


        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "You must be logged in to view your reservation history.";
                return Unauthorized();
            }

            var reservations = await _context.Reservations
                .Where(r => r.CustomerId == userId)
                .Include(r => r.Table)
                .Include(r => r.Restaurant)
                .ToListAsync();

            return View(reservations);
        }

        [HttpGet]
        public async Task<IActionResult> EditReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Table)
                .Include(r => r.Restaurant)
                .FirstOrDefaultAsync(r => r.ReserId == id);

            if (reservation == null)
            {
                TempData["Error"] = "Reservation not found.";
                return RedirectToAction("History");
            }

            ViewBag.TableName = reservation.Table?.TName ?? "N/A";
            ViewBag.RestaurantName = reservation.Restaurant?.ResName ?? "N/A";

            DateTime currentDateTime = DateTime.Now;
            currentDateTime = new DateTime(
                currentDateTime.Year,
                currentDateTime.Month,
                currentDateTime.Day,
                currentDateTime.Hour,
                currentDateTime.Minute,
                0
            );

            var editModel = new Reservation
            {
                ReserId = reservation.ReserId,
                DateTime = currentDateTime,
                Request = reservation.Request,
                CustomerId = reservation.CustomerId,
                CustomerName = reservation.CustomerName,
                TableId = reservation.TableId,
                RestaurantId = reservation.RestaurantId
            };

            return View(editModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReservation([Bind("ReserId, DateTime, Request")] Reservation updatedReservation)
        {
            if (updatedReservation.ReserId == 0)
            {
                TempData["Error"] = "Reservation ID is missing.";
                return RedirectToAction("History");
            }

            var existingReservation = await _context.Reservations.FindAsync(updatedReservation.ReserId);
            if (existingReservation == null)
            {
                TempData["Error"] = "Reservation not found.";
                return RedirectToAction("History");
            }

            existingReservation.DateTime = updatedReservation.DateTime;
            existingReservation.Request = updatedReservation.Request;
            existingReservation.RStatus = "Pending";

            try
            {
                _context.Update(existingReservation);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reservation updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating reservation: {ex.Message}";
            }

            return RedirectToAction("History");
        }




        // [HttpGet]
        // public async Task<IActionResult> Reser()
        // {
        //     // Check if the user is logged in
        //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     if (string.IsNullOrEmpty(userId))
        //     {
        //         TempData["Error"] = "You must be logged in to make a reservation.";
        //         return Unauthorized("User is not logged in.");
        //     }

        //     // Load restaurants and available tables
        //     ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
        //     ViewBag.Tables = await _context.Tables.Where(t => t.TStatus == "Available").ToListAsync();

        //     // Create a reservation model with pre-set CustomerId and CustomerName
        //     var reservation = new Reservation
        //     {
        //         CustomerId = userId,
        //         CustomerName = User.Identity.Name // Optional: You can pre-fill with the user's name
        //     };

        //     return View(reservation);
        // }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Reser([Bind("RestaurantId,DateTime,TableId,Request,CustomerName,CustomerId")] Reservation reservation)
        // {
        //     // Validate the ModelState
        //     if (!ModelState.IsValid)
        //     {
        //         TempData["Error"] = "Please correct the errors:\n" + string.Join("\n", ModelState.Values
        //             .SelectMany(v => v.Errors)
        //             .Select(e => e.ErrorMessage));
        //         ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
        //         ViewBag.Tables = await _context.Tables.Where(t => t.TStatus == "Available").ToListAsync();
        //         return View(reservation);
        //     }

        //     // Ensure the user is logged in
        //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     if (string.IsNullOrEmpty(userId))
        //     {
        //         TempData["Error"] = "You must be logged in to make a reservation.";
        //         return Unauthorized("User is not logged in.");
        //     }

        //     // Ensure the CustomerId is set from the logged-in user
        //     reservation.CustomerId = userId;

        //     // Validate the selected table
        //     if (!reservation.TableId.HasValue)
        //     {
        //         TempData["Error"] = "No table selected. Please select a table.";
        //         ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
        //         ViewBag.Tables = await _context.Tables.Where(t => t.TStatus == "Available").ToListAsync();
        //         return View(reservation);
        //     }

        //     var selectedTable = await _context.Tables.FindAsync(reservation.TableId.Value);
        //     if (selectedTable == null || selectedTable.TStatus != "Available")
        //     {
        //         TempData["Error"] = "Selected table is not available. Please choose another table.";
        //         ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
        //         ViewBag.Tables = await _context.Tables.Where(t => t.TStatus == "Available").ToListAsync();
        //         return View(reservation);
        //     }

        //     // Set reservation details
        //     reservation.RStatus = "Pending";

        //     // Update table status
        //     selectedTable.TStatus = "Reserved";

        //     try
        //     {
        //         using var transaction = await _context.Database.BeginTransactionAsync();
        //         _context.Add(reservation);
        //         _context.Tables.Update(selectedTable);
        //         await _context.SaveChangesAsync();
        //         await transaction.CommitAsync();

        //         TempData["Success"] = "Reservation successfully made.";
        //         return RedirectToAction("Index", "Reservation");
        //     }
        //     catch (Exception ex)
        //     {
        //         TempData["Error"] = "An error occurred while saving the reservation. Please try again.";
        //         Console.WriteLine($"Error while saving reservation: {ex.Message}");
        //     }

        //     ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
        //     ViewBag.Tables = await _context.Tables.Where(t => t.TStatus == "Available").ToListAsync();
        //     return View(reservation);
        // }




    }


}
