using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class konumCTX
	{
		public List<konum> konumList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var list = connection.Query<konum>(sorgu, param).ToList();

				return list;
			}
		}

		public konum konumTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Query<konum>(sorgu, param).FirstOrDefault();

				return item;
			}
		}

		public int insert(konum ol)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("insert into konum ( cihazId, att, lang, tarih) values ( @cihazId, @att, @lang, @tarih)", ol);
				return item;
			}
		}

		public int update(olcum hayvan)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("update  olcum set cihazId = @cihazId,att=@att, lang = @lang, tarih = @tarih where id = @id", hayvan);
				return item;
			}
		}
	}
}
