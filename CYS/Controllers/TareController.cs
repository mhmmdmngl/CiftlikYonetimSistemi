using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

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

		public JsonResult tareBaslat()
		{
			var guid = Guid.NewGuid();
			processCTX ppctx = new processCTX();
			process processx = new process()
			{
				processTypeId = 2,
				requestId = guid.ToString(),
				baslangicZamani = DateTime.Now,
				bitisZamani = DateTime.Now,
				tamamlandi = 0
			};
			ppctx.processEkle(processx);
			processsettingCTX settingCTX = new processsettingCTX();
			processsetting psetting = new processsetting()
			{
				islemModu = 51,
				mevcutIslem = 2,
				mevcutRequest = guid.ToString(),
				oncekiIslem = -1,
				guncellemeZamani = DateTime.Now

			};
			settingCTX.processtypeGuncelle(psetting);
			return Json(guid.ToString());

		
		}

		public JsonResult mevcutTare()
		{
			processsettingCTX settingCTX = new processsettingCTX();
			var mevcutSetting = settingCTX.processsettingTek("select * from processsetting where id = 1", null);
			KantarAyariRepo kaRepo = new KantarAyariRepo();
			var mevcutTare = kaRepo.KantarAyariTek("select * from kantarayari where requestId =@requestId", new { requestId = mevcutSetting.mevcutRequest });
			var deser = JsonConvert.SerializeObject(mevcutTare);
			return Json(deser);
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
