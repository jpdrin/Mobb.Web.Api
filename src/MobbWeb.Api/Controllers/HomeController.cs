using Microsoft.AspNetCore.Mvc;

namespace MobbWeb.Api.Controllers
{
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}
