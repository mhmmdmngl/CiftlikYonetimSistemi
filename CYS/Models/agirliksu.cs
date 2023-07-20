namespace CYS.Models
{
	public class agirliksu
	{
        public int id { get; set; }
		public int userId { get; set; }
		public User user { set; get; }
		public Hayvan hayvan { set; get; }
		public int hayvanId { get; set; }
        public double ilkOlcum { get; set; }
		public double sonOlcum { get; set; }
		public DateTime tarih { set; get; }
        public string requestId { get; set; }

    }
}
