using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers
{
	public class TareController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult KantarAyari()
		{
			if (sessionKontrol() == false)
				return RedirectToAction("GirisYap", "Login");
			return View();
		}

		bool sessionKontrol()
		{

			try
			{

				var user = HttpContext.Session.GetString("user");
				var profile = HttpContext.Session.GetString("profile");

				if (user == null)
					return false;
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

	}
}
