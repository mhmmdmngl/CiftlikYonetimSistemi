using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers.WebApis
{
    [Route("api/[controller]")]
    [ApiController]
    public class OlcumController : ControllerBase
    {
		[HttpGet]
		public string Get()
		{
			olcumSessionCTX olcumSession = new olcumSessionCTX();
			Guid sessionId = Guid.NewGuid();
			olcumSession os = new olcumSession()
			{
				sessionGuid = sessionId.ToString(),
				tarih = DateTime.Now
			};
			olcumSession.insert(os);
			return sessionId.ToString();

		}
	}
}
