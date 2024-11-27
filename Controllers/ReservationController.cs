using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blank.Models;

namespace Blank.Controllers
{
    public class ReservationController : Controller
    {
        private readonly FinalprojectContext _context;

        public ReservationController(FinalprojectContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string status = "Pending")
        {
            var reservations = await _context.Reservations
                .Include(r => r.Restaurant)
                .Include(r => r.Table)
                .Where(r => r.RStatus == status)
                .ToListAsync();

            ViewBag.CurrentStatus = status;
            return View(reservations);
        }

        public async Task<IActionResult> MakeReservation()
        {
            ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
            ViewBag.Tables = await _context.Tables.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeReservation([Bind("RestaurantId,DateTime,TableId,TableOption,Request")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                if (reservation.TableOption == "QuickPick")
                {
                    reservation.TableId = null;
                }

                reservation.RStatus = "Pending";
                _context.Add(reservation);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.Restaurants = await _context.Restaurants.ToListAsync();
            ViewBag.Tables = await _context.Tables.ToListAsync();
            return View(reservation);
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
    }



}
