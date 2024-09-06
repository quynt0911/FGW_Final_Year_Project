using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blank.Models;

using Newtonsoft.Json;

public class ReportController : Controller
{
    private readonly FinalprojectContext _context;

    public ReportController(FinalprojectContext context)
    {
        _context = context;
    }

    // Trang hiển thị danh sách report
    public async Task<IActionResult> Index()
    {
        var reports = await _context.Reports.ToListAsync();
        return View(reports);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Title,Value,Date")] Report report)
    {
        if (!ModelState.IsValid)
        {
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage); // In lỗi ra console hoặc log
                }
            }
        }
        else if (ModelState.IsValid)
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(report);
    }

    public async Task<IActionResult> Statistic()
    {
        var reports = await _context.Reports.ToListAsync();
        return View(reports);
    }



}


