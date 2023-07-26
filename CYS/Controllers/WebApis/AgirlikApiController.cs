using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class AgirlikApiController : ControllerBase
	{
		// GET: api/<RFIDApiController>
		[HttpGet]
		public string Get(float agirlik)
		{
			processsettingCTX processsetting = new processsettingCTX();
			var mevcutps = processsetting.processsettingTek("select * from processsetting where id = 1", null);
			AgirlikOlcumCTX agirlikCTX = new AgirlikOlcumCTX();
			AgirlikOlcum kam = new AgirlikOlcum()
			{
				agirlikOlcumu = agirlik.ToString(),
				userId = 1,
				tarih = DateTime.Now,
				requestId = mevcutps.mevcutRequest
			};
			agirlikCTX.agirlikOlcumEkle(kam);
			return "1";

		}


		// POST api/<RFIDApiController>
		[HttpPost]
		public void Post(float agirlik, int processId)
		{
			AgirlikOlcumCTX agirlikCTX = new AgirlikOlcumCTX();
			AgirlikOlcum kam = new AgirlikOlcum()
			{
				agirlikOlcumu = agirlik.ToString(),
				userId = 1,
				tarih = DateTime.Now
			};
			agirlikCTX.agirlikOlcumEkle(kam);
		}

		// PUT api/<RFIDApiController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<RFIDApiController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
