using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class HayvanKriterUnsurCTX
	{
		public List<HayvanKriterUnsur> HayvanKriterUnsurList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var list = connection.Query<HayvanKriterUnsur>(sorgu, param).ToList();
				KriterUnsurCTX kctx = new KriterUnsurCTX();
				HayvanCTX hctx = new HayvanCTX();
				foreach (var item in list)
				{
					item.kriterunsur = kctx.kriterUnsurTek("select * from KriterUnsur where id = @id", new { id = item.kriterUnsurId });
					//item.hayvan = hctx.hayvanTek("select * from Hayvan where id = @id", new { id = item.hayvanId });

				}

				return list;
			}
		}

		public HayvanKriterUnsur HayvanKriterUnsurTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Query<HayvanKriterUnsur>(sorgu, param).FirstOrDefault();
				KriterUnsurCTX kctx = new KriterUnsurCTX();
				HayvanCTX hctx = new HayvanCTX();
				if (item != null)
				{
					item.kriterunsur = kctx.kriterUnsurTek("select * from KriterUnsur where id = @id", new { id = item.kriterUnsurId });
					//item.hayvan = hctx.hayvanTek("select * from Hayvan where id = @id", new { id = item.hayvanId });
				}
				return item;
			}
		}

		public int HayvanKriterUnsurEkle(HayvanKriterUnsur kriter)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("insert into hayvanKriterUnsur (hayvanId, kriterUnsurId) values (@hayvanId, @kriterUnsurId)", kriter);
				return item;
			}
		}

		public int HayvanKriterUnsurGuncelle(HayvanKriterUnsur hayvan)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("update HayvanKriterUnsur set hayvanId = @hayvanId, kriterUnsurId = @kriterUnsurId, isActive = @isActive where id = @id", hayvan);
				return item;
			}
		}
	}
}
