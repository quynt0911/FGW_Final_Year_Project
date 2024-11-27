using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blank.Models;
using Microsoft.AspNetCore.Identity;

namespace Blank.Controllers
{
    public class TaskController : Controller
    {
        private readonly FinalprojectContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TaskController(FinalprojectContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Task
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                // Admin thấy tất cả các task
                var tasks = await _context.Tasks.ToListAsync();

                // Lấy danh sách Staff
                var staffList = await _userManager.GetUsersInRoleAsync("Staff");
                ViewBag.StaffList = staffList.Select(s => new { UserId = s.Id, UserName = s.UserName });

                return View(tasks);
            }
            else if (roles.Contains("Staff"))
            {
                // Staff chỉ thấy các task được giao cho họ
                var tasks = await _context.Tasks
                    .Where(t => t.AssignedTo == user.Id)
                    .ToListAsync();
                return View(tasks);
            }

            return Unauthorized();
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Task/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,TaskName,Description,TaskStatus")] Models.Task task)
        {
            if (ModelState.IsValid)
            {
                task.TaskStatus = "Pending"; // Task mặc định ở trạng thái Pending khi tạo mới
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // GET: Task/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,TaskName,Description,TaskStatus,AssignedTo")] Models.Task task)
        {
            if (id != task.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.TaskId))
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
            return View(task);
        }

        // GET: Task/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }

        [HttpPost]
        public async Task<IActionResult> AssignTask(int taskId, string userId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound();
            }

            task.AssignedTo = userId;
            task.TaskStatus = string.IsNullOrEmpty(userId) ? "Pending" : "Assigned";
            _context.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // POST: Start Task (for Staff)
        [HttpPost]
        public async Task<IActionResult> StartTask(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null || task.TaskStatus != "Assigned")
            {
                return BadRequest("Task must be assigned before starting.");
            }

            task.TaskStatus = "In Process"; // Đổi trạng thái thành In Process
            _context.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Complete Task (for Staff)
        [HttpPost]
        public async Task<IActionResult> CompleteTask(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null || task.TaskStatus != "In Process")
            {
                return BadRequest("Task must be in process to complete.");
            }

            task.TaskStatus = "Complete"; // Đổi trạng thái thành Complete
            _context.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
