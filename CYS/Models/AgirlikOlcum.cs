namespace CYS.Models
{
	public class AgirlikOlcum
	{
		public int id { get; set; }
		public int userId { get; set; }
		public User user { get; set; }
		public string agirlikOlcumu { get; set; }
		public int aktif { get; set; }
		public DateTime tarih { get; set; }


	}
}
