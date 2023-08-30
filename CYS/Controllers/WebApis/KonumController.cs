using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class KonumController : ControllerBase
	{
		[HttpGet]
		public string Get(string att, string lang, int cihazId)
		{
			konumCTX kctx = new konumCTX();
			konum knm = new konum()
			{
				cihazId = cihazId,
				att = att,
				lang = lang
			};
			kctx.insert(knm);
			return "1";
		}
	}
}
