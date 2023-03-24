using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class AgirlikApiController : ControllerBase
	{
		// GET: api/<RFIDApiController>
		[HttpGet]
		public IEnumerable<string> Get()
		{
			return new string[] { "123", "value2" };
		}

		// GET api/<RFIDApiController>/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
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
