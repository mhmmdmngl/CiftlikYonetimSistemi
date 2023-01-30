using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace CYS.Controllers
{
	public class HayvanController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult HayvanListesi()
		{
			if (sessionKontrol() == false)
				return RedirectToAction("GirisYap", "Login");
			HayvanCTX hctx = new HayvanCTX();
			var user = HttpContext.Session.GetString("user");
			var userObj = JsonConvert.DeserializeObject<Hayvan>(user);
			var hayvanListesi = hctx.hayvanList("select * from hayvan where userId = @userId and aktif = 1 order by id desc", new { userId = userObj.id });
			return View(hayvanListesi);
		}

		public IActionResult HayvanEkle()
		{
			return View();
		}

		public IActionResult HayvanSil(int hayvanId)
		{
			HayvanCTX hctx = new HayvanCTX();
			var hayvan = hctx.hayvanTek("select * from hayvan where id = @id", new { id = hayvanId });
			if(hayvan != null)
			{
				hayvan.aktif = 0;
				hctx.hayvanGuncelle(hayvan);
			}
			return RedirectToAction("HayvanListesi");
		}

		public IActionResult HayvanDuzenle(int hayvanId)
		{
			ViewBag.HayvanId = hayvanId;
			HayvanCTX hctx = new HayvanCTX();
			var hayvan = hctx.hayvanTek("select * from hayvan where id = @id", new { id = hayvanId });
			return View(hayvan);
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

		public JsonResult agirlikDondur()
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if(user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				AgirlikOlcumCTX actx = new AgirlikOlcumCTX();
				var sonAgirlik = actx.agirlikOlcumTek("select * from agirlikOlcum where userId = @userId order by id desc limit 1", new { userId = userObj.id });
				if(sonAgirlik != null)
				{
					return Json(new { status = sonAgirlik.agirlikOlcumu, tarih = sonAgirlik.tarih.ToString("dd.MM.yyyy hh:mm:s") });
				}
				return Json(new { status = "0" });
			}
			return Json(new { status = "-1" });

		}

		public JsonResult rfidDondur()
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				kupeatamaCTX actx = new kupeatamaCTX();
				var sonAgirlik = actx.kupeAtamaTek("select * from kupeatama where userId = @userId order by id desc limit 1", new { userId = userObj.id });
				if (sonAgirlik != null)
				{
					return Json(new { status = sonAgirlik.kupeRfid, tarih = sonAgirlik.tarih.ToString("dd.MM.yyyy hh:mm:s") });
				}
				return Json(new {status = "0"});
			}
			return Json(new { status = "-1" });


		}

		public JsonResult hayvanEkleJson(string rfid, string hayvanAdi, int cinsiyet)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				string cinsiyetS = "";
				switch(cinsiyet)
				{
					case 0:
						cinsiyetS = "Erkek";
						break;
					case 1:
						cinsiyetS = "Dişi";
						break;
					case 2:
						cinsiyetS = "Bilinmeyen";
						break;
				}
				var userObj = JsonConvert.DeserializeObject<User>(user);
				HayvanCTX actx = new HayvanCTX();
				Hayvan hy = new Hayvan()
				{
					cinsiyet = cinsiyetS,
					kupeIsmi = hayvanAdi,
					agirlik = "Belli Değil",
					rfidKodu = rfid,
					userId = userObj.id
				};
				actx.hayvanEkle(hy);
				return Json(new { status = "Success", message = "Hayvan Başarıyla Eklendi" });

			}
			return Json(new { status = "Error", message = "Bir Hata Oluştu" });
		}

		public JsonResult HayvanAgirlikGuncelleJson(string id, string agirlik)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				HayvanCTX actx = new HayvanCTX();
				var ilgiliHayvan = actx.hayvanTek("select * from hayvan where id = @id", new { id = id });
				if(ilgiliHayvan != null)
				{

					ilgiliHayvan.agirlik = agirlik;
					actx.hayvanGuncelle(ilgiliHayvan);
					return Json(new { status = "Success", message = "Hayvan Başarıyla Güncellendi" });

				}
			}
			return Json(new { status = "Error", message = "Bir Hata Oluştu" });

		}

		public JsonResult HayvanDuzenleJson(string hayvanId, string rfid, string hayvanAdi, string agirlik, int cinsiyet)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				string cinsiyetS = "";
				switch (cinsiyet)
				{
					case 0:
						cinsiyetS = "Erkek";
						break;
					case 1:
						cinsiyetS = "Dişi";
						break;
					case 2:
						cinsiyetS = "Bilinmeyen";
						break;
				}
				HayvanCTX hctx = new HayvanCTX();
				var hayvan = hctx.hayvanTek("select * from hayvan where id = @id", new { id = hayvanId });
				if(hayvan != null)
				{
					hayvan.rfidKodu = rfid;
					hayvan.agirlik = agirlik;
					hayvan.cinsiyet = cinsiyetS;
					hayvan.kupeIsmi = hayvanAdi;
					hctx.hayvanEkle(hayvan);
					return Json(new { status = "Success", message = "Hayvan Başarıyla Güncellendi" });
				}
				

			}
			return Json(new { status = "Error", message = "Bir Hata Oluştu" });

		}

	}
}
