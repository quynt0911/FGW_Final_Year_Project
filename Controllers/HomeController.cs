using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blank.Models;

namespace Blank.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
     private readonly FinalprojectContext _context;

    public HomeController(ILogger<HomeController> logger, FinalprojectContext context)
    {
        _logger = logger;
         _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.PageTitle = "Homepage";

        var tables = await _context.Tables.ToListAsync();
        foreach (var table in tables)
        {
            if (string.IsNullOrWhiteSpace(table.Location) || !table.Location.Contains(","))
            {
                table.Location = "0,0"; 
            }
        }

        return View(tables);
    }

    public IActionResult About()
    {
        ViewBag.PageTitle = "About Us";
        return View();
    }

    public IActionResult Inx()
    {
        ViewBag.PageTitle = "Inx";
        return View();
    }

    public IActionResult Dash()
    {
        ViewBag.PageTitle = "Dash";
        return View();
    }

    public IActionResult Privacy()
    {
        ViewBag.PageTitle = "Privacy";
        return View();
    }

    public IActionResult Contact()
    {
        ViewBag.PageTitle = "Contact Us";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
