using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
			
			ViewBag.list = hayvanTurListe();
			ViewBag.kriterler = kriterListe();
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

		public JsonResult hayvanEkleJson(string rfid, string hayvanAdi, int cinsiyet, int altTurId)
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
					userId = userObj.id,
                    kategoriId = Convert.ToInt32(altTurId)
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
					hctx.hayvanGuncelle(hayvan);
					return Json(new { status = "Success", message = "Hayvan Başarıyla Güncellendi" });
				}
				

			}
			return Json(new { status = "Error", message = "Bir Hata Oluştu" });

		}

		public class hayvanTuru
		{
			public int Id;
			public string Text;
		}

		public List<SelectListItem> hayvanTurListe()
		{
            List<hayvanTuru> ustKategoriList = new List<hayvanTuru>();
			List<SelectListItem> itList = new List<SelectListItem>();
            ustKategoriCTX hctx = new ustKategoriCTX();
            var list = hctx.ustKategoriList("select * from cys.ustKategori where isActive = 1", null);
            foreach (var item in list)
			{
				SelectListItem it = new SelectListItem() { Value = item.id.ToString(), Text = item.name };
				itList.Add(it);

            }

            return itList;
        }

		public List<SelectListItem> kriterListe()
		{
			KriterCTX kritCTX = new KriterCTX();
			List<Kriter> ustKategoriList = new List<Kriter>();
			List<SelectListItem> itList = new List<SelectListItem>();
			ustKategoriCTX hctx = new ustKategoriCTX();
			var kriterListesi = kritCTX.kriterList("select * from kriter", null);
			foreach (var item in kriterListesi)
			{
				SelectListItem it = new SelectListItem() { Value = item.id.ToString(), Text = item.kriterAdi };
				itList.Add(it);

			}

			return itList;
		}

		public JsonResult HayvanListesiJson(string q)
		{
            List<hayvanTuru> ustKategoriList = new List<hayvanTuru>();
			// Add parts to the list.
			ustKategoriCTX hctx = new ustKategoriCTX();
			var list = hctx.ustKategoriList("select * from cys.ustKategori where isActive = 1",null);
			foreach(var item in list)
			{
                ustKategoriList.Add(new hayvanTuru() { Id = item.id, Text = item.name });

            }

			if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
			{
				ustKategoriList = ustKategoriList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
			}
			return Json(new { items = ustKategoriList });
        }

        public JsonResult hayvanAltTurJson(string value)
        {
            KategoriCTX hctx = new KategoriCTX();
            var ustKategoriList = hctx.KategoriList("select * from kategori where ustKategoriId = @ustKategoriId and isActive = 1", new {ustKategoriId = Convert.ToString(value)});
            var eleman = System.Text.Json.JsonSerializer.Serialize(ustKategoriList);
            return Json(eleman);
        }

		public JsonResult hayvanOzellikGetirJson(string value)
		{
			KriterUnsurCTX hctx = new KriterUnsurCTX();
			var ozellikList = hctx.kriterUnsurList("select * from KriterUnsur where kriterId = @kriterId and isActive = 1", new { kriterId = Convert.ToString(value) });
			var eleman = System.Text.Json.JsonSerializer.Serialize(ozellikList);
			return Json(eleman);
		}

		public JsonResult HayvanOzellikEkleJson( int kriterId, int unsurId, int hayvanId)
		{
			HayvanKriterUnsurCTX hayvanKriterUnsurCTX = new HayvanKriterUnsurCTX();
			HayvanKriterUnsur hayvanKriterUnsur = new HayvanKriterUnsur()
			{
				kriterUnsurId = unsurId,
				hayvanId = hayvanId
			};
			hayvanKriterUnsurCTX.HayvanKriterUnsurEkle(hayvanKriterUnsur);
			return Json(new { status = "Success", message = "Özellik Eklendi..." });

		}

	}
}
