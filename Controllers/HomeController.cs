using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Blank.Models;

namespace Blank.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewBag.PageTitle = "Homepage";
        return View();
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
