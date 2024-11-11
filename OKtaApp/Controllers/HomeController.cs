using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            ViewBag.UserDetail = User.Claims.FirstOrDefault(x => x.Type == "name").Value;
        }
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
