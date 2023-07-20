using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class agirliksuCTX
	{
		public List<agirliksu> agirliksuList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var list = connection.Query<agirliksu>(sorgu, param).ToList();
				HayvanCTX hctx = new HayvanCTX();
				UserCTX uctx = new UserCTX();
				foreach(var item in list)
				{
					item.hayvan = hctx.hayvanTek("select * from hayvan where id = @id", new { id = item.hayvanId }); 
					item.user = uctx.userTek("select * from user where id = @id", new {id = item.userId}); ;
				}
				return list;
			}
		}

		public agirliksu agirliksuTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Query<agirliksu>(sorgu, param).FirstOrDefault();
				HayvanCTX hctx = new HayvanCTX();
				UserCTX uctx = new UserCTX();
				if (item != null)
				{
					item.hayvan = hctx.hayvanTek("select * from hayvan where id = @id", new { id = item.hayvanId });
					item.user = uctx.userTek("select * from user where id = @id", new { id = item.userId }); 
				}
				return item;
			}
		}

		public int agirliksuEkle(agirliksu kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("insert into agirliksu (userId, hayvanId, ilkOlcum, sonOlcum, tarih, requestId) values (@userId, @hayvanId, @ilkOlcum, @sonOlcum, @tarih, @requestId)", kriter);
				return item;
			}
		}

		public int agirliksuGuncelle(agirliksu kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("update agirliksu set userId = userId, hayvanId= hayvanId, ilkOlcum= ilkOlcum, sonOlcum= sonOlcum, tarih= tarih, requestId= requestId where id = @id", kriter);
				return item;
			}
		}
	}
}
