using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class HayvanCTX
	{
		public List<Hayvan> hayvanList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var list = connection.Query<Hayvan>(sorgu, param).ToList();
				UserCTX uctx = new UserCTX();
				foreach (var item in list)
				{
					item.user = uctx.userTek("select * from user where id = @id", new { id = item.userId });
				}
				return list;
			}
		}

		public Hayvan hayvanTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Query<Hayvan>(sorgu, param).FirstOrDefault();
				UserCTX uctx = new UserCTX();
				if(item != null)
					item.user = uctx.userTek("select * from user where id = @id", new { id = item.userId });
				return item;
			}
		}

		public int hayvanEkle(Hayvan hayvan)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("insert into hayvan (rfidKodu,kupeIsmi,cinsiyet,agirlik, userId) values (@rfidKodu,@kupeIsmi,@cinsiyet,@agirlik, @userId)", hayvan);
				return item;
			}
		}

		public int hayvanGuncelle(Hayvan hayvan)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("update hayvan set rfidKodu = rfidKodu,kupeIsmi = @kupeIsmi,cinsiyet = @cinsiyet,agirlik = @agirlik, userId= @userId, aktif = @aktif where id = @id", hayvan);
				return item;
			}
		}
	}
}
