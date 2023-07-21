using CYS.Repos;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace CYS.Controllers
{
	public class BuyukBasIslemController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public JsonResult iptalEt()
		{
			processsettingCTX processsettingCTX = new processsettingCTX();
			var mevcut = processsettingCTX.processsettingTek("select * from processsetting where id = 1", null);
			mevcut.islemModu = 121;
			processsettingCTX.processtypeGuncelle(mevcut);
			return Json("1");
		}
	}
}
