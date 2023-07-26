using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class TareApiController : ControllerBase
	{
		[HttpPost]
		public string Get(int islemModu)
		{
			processsettingCTX settingCTX = new processsettingCTX();
			var mevcutRequest = settingCTX.processsettingTek("select * from processsetting where id = 1", null);
			if (islemModu == 51)
			{

				KantarAyariRepo kaCTX = new KantarAyariRepo();
				KantarAyari ka = new KantarAyari()
				{
					RequestId = mevcutRequest.mevcutRequest,
					Tare3 = 1,
					Tarih = DateTime.Now
				};
				kaCTX.KantarAyariEkle(ka);
			}
			else if (islemModu == 53)
			{
				var mevcutps = settingCTX.processsettingTek("select * from processsetting where id = 1", null);

				KantarAyariRepo kaCTX = new KantarAyariRepo();
				var mevcutKantar = kaCTX.KantarAyariTek("select * from kantarayari where requestId = @requestId", new { requestId = mevcutps.mevcutRequest });
				if (mevcutKantar != null)
				{
					mevcutKantar.Tare5 = 1;
					kaCTX.KantarAyariGuncelle(mevcutKantar);
				}
			}

			else if (islemModu == 52)
			{
				var mevcutps = settingCTX.processsettingTek("select * from processsetting where id = 1", null);

				KantarAyariRepo kaCTX = new KantarAyariRepo();
				var mevcutKantar = kaCTX.KantarAyariTek("select * from kantarayari where requestId = @requestId", new { requestId = mevcutps.mevcutRequest });
				if (mevcutKantar != null)
				{
					mevcutKantar.Tare4 = 1;
					kaCTX.KantarAyariGuncelle(mevcutKantar);
				}
				
			}
			else if(islemModu == 99)
			{

			}


			return "1";
		}
	}
}
