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
    public class TableController : Controller
    {
        private readonly FinalprojectContext _context;

        public TableController(FinalprojectContext context)
        {
            _context = context;
        }

        // GET: Table
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tables.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> ViewT()
        {
            var tables = await _context.Tables.ToListAsync();
            return View(tables);
        }

        [HttpGet]
        public async Task<IActionResult> ViewTU()
        {
            var tables = await _context.Tables.ToListAsync();
            return View(tables);
        }


        [HttpGet("Table/DetailsApi/{id}")]
        public async Task<IActionResult> DetailsApi(int id)
        {
            try
            {
                var table = await _context.Tables.FindAsync(id);
                if (table == null)
                {
                    Console.WriteLine($"Table with ID {id} not found.");
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
                Console.WriteLine($"Error fetching table details: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Table/UpdateAllPositions")]
        public async Task<IActionResult> UpdateAllPositions([FromBody] List<TablePositionUpdate> updates)
        {
            try
            {
                if (updates == null || updates.Count == 0)
                {
                    return BadRequest(new { success = false, message = "No updates provided" });
                }

                foreach (var update in updates)
                {
                    Console.WriteLine($"Updating TableId: {update.TableId}, Location: {update.Location}");
                    var table = await _context.Tables.FindAsync(Convert.ToInt32(update.TableId));
                    if (table != null)
                    {
                        table.Location = update.Location;
                    }
                    else
                    {
                        Console.WriteLine($"Table with ID {update.TableId} not found.");
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "All table positions updated successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating positions: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        public class TablePositionUpdate
        {
            public string TableId { get; set; }
            public string Location { get; set; }
        }




        // GET: Table/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Tables
                .FirstOrDefaultAsync(m => m.TableId == id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // GET: Table/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Table/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TableId,TName,Location,TStatus")] Table table)
        {
            if (ModelState.IsValid)
            {
                _context.Add(table);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(table);
        }

        // GET: Table/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Tables.FindAsync(id);
            if (table == null)
            {
                return NotFound();
            }
            return View(table);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TableId,TName,Location,TStatus")] Table table)
        {
            if (id != table.TableId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(table);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableExists(table.TableId))
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
            return View(table);
        }

        // GET: Table/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Tables
                .FirstOrDefaultAsync(m => m.TableId == id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // POST: Table/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table != null)
            {
                _context.Tables.Remove(table);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TableExists(int id)
        {
            return _context.Tables.Any(e => e.TableId == id);
        }

        [HttpPost("UpdatePosition")]
        public async Task<IActionResult> UpdatePosition(int tableId, string location)
        {
            if (string.IsNullOrWhiteSpace(location) || !location.Contains(","))
            {
                return BadRequest(new { success = false, message = "Invalid location format" });
            }

            var table = await _context.Tables.FindAsync(tableId);
            if (table == null)
            {
                return NotFound(new { success = false, message = "Table not found" });
            }

            table.Location = location;
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Table position updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
