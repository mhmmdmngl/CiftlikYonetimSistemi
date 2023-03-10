using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginApiController : ControllerBase
	{
		[HttpPost]
		public int Post(string username, string password)
		{
			UserCTX userCTX = new UserCTX();
			var userVarMi = userCTX.userTek("select * from user where username = @username and password = @password", new { username, password });
			if(userVarMi != null) { return 1; }
			return 0;
		}
	}
}
