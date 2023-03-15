using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class AgirlikOlcumCTX
	{
		public List<AgirlikOlcum> agirlikOlcumList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var list = connection.Query<AgirlikOlcum>(sorgu, param).ToList();
				UserCTX uctx = new UserCTX();
				foreach (var item in list)
				{
					item.user = uctx.userTek("select * from user where id = @id", new { id = item.userId});
				}
				return list;
			}
		}

		public AgirlikOlcum agirlikOlcumTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Query<AgirlikOlcum>(sorgu, param).FirstOrDefault();
				UserCTX uctx = new UserCTX();
				if (item != null)
					item.user = uctx.userTek("select * from user where id = @id", new { id = item.userId });
				return item;
			}
		}

		public int agirlikOlcumEkle(AgirlikOlcum hayvan)
		{


			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("insert into agirlikolcum (userId,agirlikOlcumu, requestId) values (@userId,@agirlikOlcumu, @requestId)", hayvan);
				return item;
			}
		}

		public int agirlikOlcumGuncelle(AgirlikOlcum hayvan)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("update agirlikolcum set userId = @userId,agirlikOlcumu=@agirlikOlcumu, aktif = @aktif, requestId = @requestId where id = @id", hayvan);
				return item;
			}
		}
	}
}
