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
            // Check if the user is logged in
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "You must be logged in to make a reservation.";
                return Unauthorized("User is not logged in.");
            }

            // Load restaurants and available tables
            ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
            ViewBag.Tables = await _context.Tables.Where(t => t.TStatus == "Available").ToListAsync();

            // Create a reservation model with pre-set CustomerId and CustomerName
            var reservation = new Reservation
            {
                CustomerId = userId,
                CustomerName = User.Identity.Name // Optional: Pre-fill with user's name
            };

            return View(reservation);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeReservation([Bind("RestaurantId,DateTime,TableId,Request,CustomerName")] Reservation reservation)
        {
            // Ensure the user is logged in
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "You must be logged in to make a reservation.";
                return Unauthorized("User is not logged in.");
            }

            // Assign CustomerId and default values
            reservation.CustomerId = userId;
            reservation.RStatus = "Pending";

            // Validate the selected table
            if (!reservation.TableId.HasValue)
            {
                TempData["Error"] = "No table selected. Please select a table.";
                ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
                return View(reservation);
            }

            var selectedTable = await _context.Tables.FindAsync(reservation.TableId.Value);
            if (selectedTable == null || selectedTable.TStatus != "Available")
            {
                TempData["Error"] = "Selected table is not available. Please choose another table.";
                ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
                return View(reservation);
            }

            // Update table status
            selectedTable.TStatus = "Reserved";

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                _context.Add(reservation);
                _context.Tables.Update(selectedTable);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Reservation successfully made.";
                return RedirectToAction("Index", "Reservation");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while saving the reservation: {ex.Message}";
                Console.WriteLine($"Error while saving reservation: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }

            ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
            return View(reservation);
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
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(reservation);
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
            // Lấy lịch sử đặt bàn của người dùng hiện tại
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

            ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
            ViewBag.Tables = await _context.Tables.Where(t => t.TStatus == "Available" || t.TableId == reservation.TableId).ToListAsync();

            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReservation([Bind("ReserId, RestaurantId, DateTime, TableId, Request, CustomerId")] Reservation updatedReservation)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors and try again.";
                ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
                ViewBag.Tables = await _context.Tables.Where(t => t.TStatus == "Available" || t.TableId == updatedReservation.TableId).ToListAsync();
                return View(updatedReservation);
            }

            var existingReservation = await _context.Reservations.FindAsync(updatedReservation.ReserId);
            if (existingReservation == null)
            {
                TempData["Error"] = "Reservation not found.";
                return RedirectToAction("History");
            }

            var selectedTable = await _context.Tables.FindAsync(updatedReservation.TableId);
            if (selectedTable == null || (selectedTable.TStatus != "Available" && selectedTable.TableId != existingReservation.TableId))
            {
                TempData["Error"] = "Selected table is not available. Please choose another table.";
                return View(updatedReservation);
            }

            try
            {
                // Lưu vị trí cũ
                existingReservation.RStatus = "Pending";
                existingReservation.RestaurantId = updatedReservation.RestaurantId;
                existingReservation.DateTime = updatedReservation.DateTime;
                existingReservation.Request = updatedReservation.Request;

                if (existingReservation.TableId != updatedReservation.TableId)
                {
                    var previousTable = await _context.Tables.FindAsync(existingReservation.TableId);
                    if (previousTable != null)
                    {
                        previousTable.TStatus = "Available"; // Bàn cũ trở lại trạng thái Available
                        _context.Tables.Update(previousTable);
                    }

                    selectedTable.TStatus = "Reserved";
                }

                existingReservation.TableId = updatedReservation.TableId;
                _context.Reservations.Update(existingReservation);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Reservation successfully updated. Waiting for admin approval.";
                return RedirectToAction("History");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating reservation: {ex.Message}";
                return View(updatedReservation);
            }
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
