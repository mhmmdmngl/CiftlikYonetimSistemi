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
			if(userVarMi != null) { return userVarMi.id; }
			return 0;
		}

		[HttpPut]
		public int Update(int userId, string link)
		{
			UserCTX userCTX = new UserCTX();
			var userVarMi = userCTX.userTek("select * from user where id = @id ", new { id = userId});
			if (userVarMi != null) { 
				ProfileCTX pctx = new ProfileCTX();
				var mevcutProfil = pctx.profilTek("select * from profile where userId = @userId", new {userId = userId});
				mevcutProfil.cihazLink= link;
				pctx.profilGuncelle(mevcutProfil);
			}
			return 0;
		}
	}
}
