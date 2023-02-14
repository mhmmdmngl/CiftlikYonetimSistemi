using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class KriterUnsurCTX
	{
		public List<KriterUnsur> kriterUnsurList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var list = connection.Query<KriterUnsur>(sorgu, param).ToList();
				KriterCTX kctx = new KriterCTX();
				foreach(var item in list)
					item.kriter = kctx.kriterTek("select * from Kriter where id = @id", new { id = item.kriterId });

				return list;
			}
		}

		public KriterUnsur kriterUnsurTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Query<KriterUnsur>(sorgu, param).FirstOrDefault();
				KriterCTX kctx = new KriterCTX();
				if (item != null)
					item.kriter = kctx.kriterTek("select * from Kriter where id = @id", new { id = item.kriterId });

				return item;
			}
		}

		public int KriterUnsurEkle(KriterUnsur kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("insert into KriterUnsur (kriterId, unsurAdi) values (@kriterId, @unsurAdi)", kriter);
				return item;
			}
		}

		//public int kriterGuncelle(Kriter kriter)
		//{
		//	using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
		//	{
		//		var item = connection.Execute("update Kriter set kriterAdi = @kriterAdi,isActive = @isActive where id = @id", kriter);
		//		return item;
		//	}
		//}
	}
}
