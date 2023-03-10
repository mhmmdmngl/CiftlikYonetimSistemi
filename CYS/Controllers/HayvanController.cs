using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.ObjectModelRemoting;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using RestSharp;
using System.Collections.Generic;
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

		public JsonResult agirlikDondur(string requestId)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if(user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				AgirlikOlcumCTX actx = new AgirlikOlcumCTX();
				var sonAgirlik = actx.agirlikOlcumTek("select * from agirlikolcum where userId = @userId and requestId = @requestId order by id desc limit 1", new { userId = userObj.id, requestId = requestId });
				if(sonAgirlik != null)
				{
					return Json(new { status = sonAgirlik.agirlikOlcumu, tarih = sonAgirlik.tarih.ToString("dd.MM.yyyy hh:mm:s") });
				}
				return Json(new { status = "0" });
			}
			return Json(new { status = "-1" });

		}

		public JsonResult rfidDondur(string requestId)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				kupeatamaCTX actx = new kupeatamaCTX();
				var sonAgirlik = actx.kupeAtamaTek("select * from kupeatama where userId = @userId and requestId = @requestId order by id desc limit 1", new { userId = userObj.id, requestId = requestId });
				if (sonAgirlik != null)
				{
					HayvanCTX hctx = new HayvanCTX();
					var hayvanVarmi = hctx.hayvanTek("select * from Hayvan where rfidKodu = @rfidKodu and aktif = 1", new { rfidKodu = sonAgirlik.kupeRfid });
					if(hayvanVarmi != null)
					{
						var mevcutHayvan = System.Text.Json.JsonSerializer.Serialize(hayvanVarmi);
						return Json(new { status = "mevcut", message = mevcutHayvan });

					}
					return Json(new { status = sonAgirlik.kupeRfid, tarih = sonAgirlik.tarih.ToString("dd.MM.yyyy hh:mm:s") });
				}
				return Json(new {status = "0"});
			}
			return Json(new { status = "-1" });


		}

		public JsonResult hayvanEkleJson(string rfid, string hayvanAdi, int cinsiyet, int altTurId, string agirlik)
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
				var hayvanVarMi = actx.hayvanTek("select * from Hayvan where rfidKodu = @rfidKodu", new { rfidKodu = rfid });
				if(hayvanVarMi != null)
				{
					hayvanVarMi.cinsiyet = cinsiyetS;
					hayvanVarMi.kupeIsmi = hayvanAdi;
					hayvanVarMi.agirlik = agirlik;
					hayvanVarMi.rfidKodu = rfid;
					hayvanVarMi.userId = userObj.id;
					hayvanVarMi.kategoriId = Convert.ToInt32(altTurId);

					actx.hayvanGuncelle(hayvanVarMi);
					AgirlikHayvanCTX ahctx = new AgirlikHayvanCTX();
					agirlikHayvan ah = new agirlikHayvan()
					{
						hayvanId = hayvanVarMi.id,
						agirlikId = hayvanVarMi.agirlik
					};
					ahctx.agirlikHayvanEkle(ah);
					return Json(new { status = "Success", message = "Mevcut Hayvan Başarıyla Güncellendi" });

				}
				Hayvan hy = new Hayvan()
				{
					cinsiyet = cinsiyetS,
					kupeIsmi = hayvanAdi,
					agirlik = agirlik,
					rfidKodu = rfid,
					userId = userObj.id,
                    kategoriId = Convert.ToInt32(altTurId)
                };
				actx.hayvanEkle(hy);
				var eklenenson = actx.hayvanTek("select * from Hayvan order by id desc limit 1", null);
				AgirlikHayvanCTX ahctx1 = new AgirlikHayvanCTX();
				agirlikHayvan ah1 = new agirlikHayvan()
				{
					hayvanId = eklenenson.id,
					agirlikId = eklenenson.agirlik
				};
				ahctx1.agirlikHayvanEkle(ah1);

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
            var list = hctx.ustKategoriList("select * from ustkategori where isActive = 1", null);
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
			var list = hctx.ustKategoriList("select * from ustkategori where isActive = 1",null);
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
			var ozellikList = hctx.kriterUnsurList("select * from kriterunsur where kriterId = @kriterId and isActive = 1", new { kriterId = Convert.ToString(value) });
			var eleman = System.Text.Json.JsonSerializer.Serialize(ozellikList);
			return Json(eleman);
		}

		public JsonResult hayvaninOzellikleri(int hayvanId)
		{
			KriterUnsurCTX hctx = new KriterUnsurCTX();
			var ozellikList = hctx.kriterUnsurList("select * from hayvankriterunsur where hayvanId = @hayvanId and isActive = 1", new { hayvanId = Convert.ToString(hayvanId) });
			var eleman = System.Text.Json.JsonSerializer.Serialize(ozellikList);
			return Json(eleman);
		}

		public JsonResult HayvanOzellikEkleJson( int kriterId, int unsurId, int hayvanId)
		{
			HayvanKriterUnsurCTX hayvanKriterUnsurCTX = new HayvanKriterUnsurCTX();
			KriterUnsurCTX kctx = new KriterUnsurCTX();

			var varmi = kctx.kriterUnsurTek("SELECT kriterunsur.* FROM kriter, kriterunsur, hayvankriterunsur, hayvan where hayvankriterunsur.kriterUnsurId = kriterunsur.id and kriterunsur.kriterId = kriter.id and kriter.id = @kriterId and hayvankriterunsur.isActive = 1 and hayvan.id = @hayvanId and hayvan.id = hayvankriterunsur.hayvanId", new { hayvanId = hayvanId, kriterId = kriterId });
			if(varmi != null)
			{
				var mevcut = hayvanKriterUnsurCTX.HayvanKriterUnsurTek("select * from hayvankriterunsur where kriterUnsurId = @kriterUnsurId and hayvanId = @hayvanId and isActive = 1", new { hayvanId = hayvanId, kriterUnsurId = varmi.id });
				if(mevcut != null)
				{
					mevcut.kriterUnsurId = unsurId;
					hayvanKriterUnsurCTX.HayvanKriterUnsurGuncelle(mevcut);
					return Json(new { status = "Success", message = "Özellik Güncellendi..." });

				}
				return Json(new { status = "Error", message = "Bir Hata Oluştu..." });

			}

			HayvanKriterUnsur hayvanKriterUnsur = new HayvanKriterUnsur()
			{
				kriterUnsurId = unsurId,
				hayvanId = hayvanId
			};
			hayvanKriterUnsurCTX.HayvanKriterUnsurEkle(hayvanKriterUnsur);
			return Json(new { status = "Success", message = "Özellik Eklendi..." });

		}

		public JsonResult HayvanOzellikSilJson(int id)
		{
			HayvanKriterUnsurCTX hayvanKriterUnsurCTX = new HayvanKriterUnsurCTX();
			var varmi = hayvanKriterUnsurCTX.HayvanKriterUnsurTek("select * from hayvankriterunsur where id = @id and isActive = 1", new { id = id});
			if (varmi != null)
			{
				varmi.isActive = 0;
				hayvanKriterUnsurCTX.HayvanKriterUnsurGuncelle(varmi);
				return Json(new { status = "Success", message = "Özellik Silindi..." });


			}
			return Json(new { status = "Error", message = "Özellik bulunamadı" });


		}

		public JsonResult rfidIstekJson(string requestId)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				var profileObj = JsonConvert.DeserializeObject<Profile>(profile);
				kupeatamaCTX hctx = new kupeatamaCTX();

				var eklenenId = hctx.kupeAtamaTek("select * from kupeatama where requestId = @requestId", new { requestId = requestId });
				if(eklenenId == null)
				{
					KupeAtama kupe = new KupeAtama()
					{
						requestId = requestId,
						userId = userObj.id,
						kupeRfid = ""
					};
					hctx.kupeAtamaEkle(kupe);
					eklenenId = hctx.kupeAtamaTek("select * from kupeatama where requestId = @requestId", new { requestId = requestId });
				}


				var client = new RestClient(profileObj.cihazLink + "/RFIDApi");
				client.Timeout = -1;
				var request = new RestRequest(Method.GET);
				IRestResponse response = client.Execute(request);
				var cevap = response.Content;
				//var gelen = JsonConvert.DeserializeObject<string>(cevap);
				if (cevap == "")
					return Json(new { status = "error", message = "Okuma işlemi gerçekleştirilemedi" });


				eklenenId.kupeRfid = cevap;

				hctx.kupeAtamaGuncelle(eklenenId);
				return Json("");

			}
			return Json("");


		}


		public JsonResult agirlikIstekJson(string requestId)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				var profileObj = JsonConvert.DeserializeObject<Profile>(profile);
				AgirlikOlcumCTX hctx = new AgirlikOlcumCTX();
				var eklenenId = hctx.agirlikOlcumTek("select * from agirlikolcum where requestId = @requestId", new { requestId = requestId });
				if(eklenenId == null)
				{
					AgirlikOlcum agirlik = new AgirlikOlcum()
					{
						requestId = requestId,
						userId = userObj.id,
						agirlikOlcumu = ""
					};
					hctx.agirlikOlcumEkle(agirlik);
					eklenenId = hctx.agirlikOlcumTek("select * from agirlikolcum where requestId = @requestId", new { requestId = requestId });
				}
				
				var client = new RestClient(profileObj.cihazLink+"/AgirlikApi");
				client.Timeout = -1;
				var request = new RestRequest(Method.GET);
				IRestResponse response = client.Execute(request);
				var cevap = response.Content;
				//var gelen = JsonConvert.DeserializeObject<string>(cevap);
				if(cevap == "")
					return Json(new { status = "error", message = "Okuma işlemi gerçekleştirilemedi" });

				eklenenId.agirlikOlcumu = cevap;

				hctx.agirlikOlcumGuncelle(eklenenId);
				return Json("");
			}
			return Json("");

			
		}

		public JsonResult hayvaninOzellikleriJson(int hayvanId)
		{
			HayvanKriterUnsurCTX hkctx = new HayvanKriterUnsurCTX();
			var list = hkctx.HayvanKriterUnsurList("select * from hayvankriterunsur where hayvanId = @hayvanId and isActive = 1", new { hayvanId = hayvanId });
			var jsonObject = JsonConvert.SerializeObject(list);
			return Json(jsonObject);

		}

		public JsonResult son5Hayvan()
		{
			HayvanCTX hctx = new HayvanCTX();
			var son5 = hctx.hayvanList("select * from hayvan where aktif = 1 order by id desc limit 5", null);
			var jsonObject = JsonConvert.SerializeObject(son5);
			return Json(jsonObject);
		}

		public JsonResult tumHayvanlar(string q)
		{
			HayvanCTX hctx = new HayvanCTX();
			var hepsi = hctx.hayvanList("select * from hayvan where aktif = 1 order by id desc", null);
			var jsonObject = JsonConvert.SerializeObject(hepsi);
			return Json(jsonObject);
		}

		public JsonResult ustSoyEkleJson(int ustHayvanId, int hayvanId)
		{
			if (ustHayvanId == hayvanId)
				return Json("");
			UstSoyCTX usctx = new UstSoyCTX();
			var VarMi = usctx.soyagaciTek("select * from soyagaci where ustHayvanId = @ustHayvanId and hayvanId = @hayvanId and isActive = 1", new { ustHayvanId = ustHayvanId, hayvanId = hayvanId });
			if (VarMi != null)
				return Json("");
			soyagaci us = new soyagaci()
			{
				ustHayvanId = ustHayvanId,
				hayvanId = hayvanId
			};
			usctx.soyagaciEkle(us);
			return Json("");
		}

		public class miniTur
		{
			public int id { get; set; }

			public string HayvanTur { get; set; }
			public string AltTur { get; set; }
			public string RFIDKodu { get; set; }
			public string Agirlik { get; set; }
			public string img { get; set; }

		}
		public JsonResult hayvanAkraba(int hayvanId)
		{
			UstSoyCTX usctx = new UstSoyCTX();
			List<soyagaci> soyList = new List<soyagaci>();
			var ebeveyns = usctx.soyagaciList("select * from soyagaci where hayvanId = @hayvanId and isActive = 1", new { hayvanId = hayvanId });
			soyList.AddRange(ebeveyns);
			foreach(var item in ebeveyns)
			{
				soyList.AddRange( usctx.soyagaciList("select * from soyagaci where ustHayvanId = @ustHayvanId and isActive = 1", new {ustHayvanId = item.ustHayvanId}));
			}
			soyList.AddRange(usctx.soyagaciList("select * from soyagaci where ustHayvanId = @ustHayvanId and isActive = 1", new { ustHayvanId = hayvanId }));
			List<miniTur> minTurList = new List<miniTur>();
			int flag = 0;
			foreach(var item in soyList)
			{
				miniTur mt = new miniTur();

				if (flag == 0)
					flag = 1;
	

				mt.id = item.id;
				mt.HayvanTur = item.hayvan.kategori.ustkategori.name;
				mt.AltTur = item.hayvan.kategori.kategoriAdi;
				mt.RFIDKodu = item.hayvan.rfidKodu;
				mt.Agirlik = item.hayvan.agirlik;
				mt.img = "https://cdn.balkan.app/shared/5.jpg";
				minTurList.Add(mt);

			}
			
			var userObj = System.Text.Json.JsonSerializer.Serialize(minTurList);
			var sey = Json(userObj);
			return sey;
		}


		public JsonResult kapiTetikle(int kapiId)
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			if (user != null && profile != null)
			{
				var userObj = JsonConvert.DeserializeObject<User>(user);
				var profileObj = JsonConvert.DeserializeObject<Profile>(profile);

				var client = new RestClient(profileObj.cihazLink + "/Secim?secenek="+ kapiId.ToString());
				client.Timeout = -1;
				var request = new RestRequest(Method.GET);
				IRestResponse response = client.Execute(request);
				var cevap = response.Content;
				//var gelen = JsonConvert.DeserializeObject<string>(cevap);
				if (cevap == "")
					return Json("");

				
				return Json("");
			}
			return Json("");
		}
	}
}
