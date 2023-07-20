using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class BuyukBasAgirlikController : ControllerBase
	{
		[HttpGet]
		public int Post(string eid, string baslangicAgirligi, string sonAgirlik)
		{
			processsettingCTX processsettingCTX = new processsettingCTX();
			var ps = processsettingCTX.processsettingTek("select * from processsetting where id =1", null);
			agirliksuCTX agirliksuCTX = new agirliksuCTX();
			var mevcutguiddeVarMi = agirliksuCTX.agirliksuTek("select * from agirliksu where requestId = @requestId", ps.mevcutRequest);
			Hayvan ilgiliHayvan = null;
			if (eid != "" && baslangicAgirligi == "0" && sonAgirlik == "0")
			{
				if (mevcutguiddeVarMi == null)
				{
					HayvanCTX hctx = new HayvanCTX();
					ilgiliHayvan = hctx.hayvanTek("select * from hayvan where rfidKodu = @rfidKodu", new { rfidKodu = eid });
					if (ilgiliHayvan == null)
					{
						var toplamHayvanSayisi = hctx.hayvanList("select id from Hayvan where aktif = 1", null);
						ilgiliHayvan = new Hayvan()
						{
							cinsiyet = "Bilinmiyor",
							kategoriId = -1,
							kupeIsmi = "TRK-" + DateTime.Now.ToString("dd.MM.yyyy") + "-" + (toplamHayvanSayisi.Count + 1).ToString(),
							rfidKodu = eid,
							agirlik = "0",
							sonGuncelleme = DateTime.Now,
							requestId = ps.mevcutRequest
						};
						hctx.hayvanEkle(ilgiliHayvan);
						ilgiliHayvan = hctx.hayvanTek("select * from Hayvan where rfidKodu = @eid", new { eid = eid });
					}
					mevcutguiddeVarMi = new agirliksu()
					{
						hayvanId = ilgiliHayvan.id,
						requestId = ps.mevcutRequest,
						ilkOlcum = "-1",
						sonOlcum = "-1",
						userId = 1,
						tarih = DateTime.Now

					};
					agirliksuCTX.agirliksuEkle(mevcutguiddeVarMi);
					return 1;
				}
			}
			else if (eid != "" && baslangicAgirligi != "0")
			{
				var varMi = agirliksuCTX.agirliksuTek("select * from agirliksu where requestId = @requestId", new { requestId = ps.mevcutRequest });
				if (varMi != null)
				{
					varMi.ilkOlcum = baslangicAgirligi;
					agirliksuCTX.agirliksuGuncelle(varMi);
					return 1;
				}
			}
			else if (eid != "" && sonAgirlik != "0")
			{
				var varMi = agirliksuCTX.agirliksuTek("select * from agirliksu where requestId = @requestId", new { requestId = ps.mevcutRequest });
				if (varMi != null)
				{
					varMi.sonOlcum = sonAgirlik;
					agirliksuCTX.agirliksuGuncelle(varMi);

					ps.mevcutRequest = "";
					ps.islemModu = 0;
					processsettingCTX.processtypeGuncelle(ps);
					return 1;
				}
			}
			return 0;
		}
	}
}
