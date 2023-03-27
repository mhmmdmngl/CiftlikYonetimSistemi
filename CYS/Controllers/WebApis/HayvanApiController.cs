using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class HayvanApiController : ControllerBase
	{
		[HttpGet]
		public IEnumerable<Hayvan> Get(int userId)
		{
			HayvanCTX hctx = new HayvanCTX();
			return hctx.hayvanList("select * from Hayvan where userId = @userId", new {userId = userId});
		}

	}
}
