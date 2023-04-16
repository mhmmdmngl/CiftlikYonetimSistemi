﻿using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class HayvanApiController : ControllerBase
	{
		public class postCevap
		{
			public int isSynced { set; get; }
			public int insertUpdate { set; get; }
			public string message { set; get; }
		}
		[HttpGet]
		public IEnumerable<Hayvan> Get(int userId)
		{
			HayvanCTX hctx = new HayvanCTX();
			return hctx.hayvanList("select * from Hayvan where userId = @userId", new {userId = userId});
		}

		[HttpPost]
		public postCevap hayvanEkle(string hayvan, string requestId)
		{
			postCevap pc = new postCevap();
			var hayvanNesne = JsonConvert.DeserializeObject<Hayvan>(hayvan);
			HayvanCTX hctx = new HayvanCTX();
			AgirlikHayvanCTX ahctx = new AgirlikHayvanCTX();
			kupehayvanCTX kaCTX = new kupehayvanCTX();
			var hayvanVarMi = hctx.hayvanTek("select * from Hayvan where rfidKodu = @rfidKodu and aktif = 1", new { rfidKodu = hayvanNesne.rfidKodu });
			if(hayvanVarMi != null)
			{
				if(hayvanVarMi.requestId == requestId)
				{
					pc.isSynced = 0;
					pc.insertUpdate = 0;
					pc.message = "Bu güncelleme zaten mevcut";
					return pc;
				}
				var guncelle = hctx.hayvanGuncelle(hayvanNesne);
				if(guncelle == 1)
				{
					pc.isSynced = 1;
					pc.insertUpdate = 2;
					pc.message = "Güncelleme İşlemi tamamlandı";

					agirlikHayvan ah = new agirlikHayvan()
					{
						hayvanId = hayvanVarMi.id,
						agirlikId = hayvanNesne.agirlik,
						requestId = requestId,
						tarih = DateTime.Now

					};
					ahctx.agirlikHayvanEkle(ah);
					return pc;

				}
				pc.isSynced = -1;
				pc.insertUpdate = -1;
				pc.message = "Bilinmeyen hata oluştu";
				return pc;

			}
			else
			{
				var eklendiMi = hctx.hayvanEkle(hayvanNesne);
				hayvanVarMi = hctx.hayvanTek("select * from hayvan where requestId = @requestId and aktif = 1", new { requestId = requestId });
				if (eklendiMi == 1)
				{
					pc.isSynced = 1;
					pc.insertUpdate = 1;
					pc.message = "Ekleme İşlemi tamamlandı";

					agirlikHayvan ah = new agirlikHayvan()
					{
						hayvanId = hayvanVarMi.id,
						agirlikId = hayvanNesne.agirlik,
						requestId = requestId,
						tarih = DateTime.Now

					};
					ahctx.agirlikHayvanEkle(ah);

					kupehayvan kh = new kupehayvan()
					{
						hayvanId = hayvanVarMi.id,
						kupeId = hayvanVarMi.rfidKodu,
						requestId = requestId,
						tarih = DateTime.Now
					};
					kaCTX.kupehayvanEkle(kh);
					return pc;
				}
				pc.isSynced = -1;
				pc.insertUpdate = -1;
				pc.message = "Bilinmeyen hata oluştu";
				return pc;
			}

		}

	}
}