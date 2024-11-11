using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Profile()
    {
        return View(User);
    }
    public IActionResult Error()
    {
        return View();
    }
}
