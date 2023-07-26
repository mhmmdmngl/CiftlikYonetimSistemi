using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CYS.Models
{
	[Route("api/[controller]")]
	[ApiController]
	public class KantarAyari : ControllerBase
	{
		public int Id { get; set; }
		public string RequestId { get; set; }
		public int Tare3 { get; set; }
		public int Tare4 { get; set; }
		public int Tare5 { get; set; }
		public string EsikAgirlik { get; set; }
		public DateTime Tarih { get; set; }
	}
}
