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
				KategoriCTX kctx = new KategoriCTX();
				HayvanKriterUnsurCTX hkuCTX = new HayvanKriterUnsurCTX();
				foreach (var item in list)
				{
					item.user = uctx.userTek("select * from user where id = @id", new { id = item.userId });
					item.kategori = kctx.KategoriTek("select * from kategori where id = @id", new {id = item.kategoriId});
					item.ozellikler = hkuCTX.HayvanKriterUnsurList("select * from hayvankriterunsur where hayvanId = @hayvanId and isActive = 1", new { hayvanId = item.id });
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
				HayvanKriterUnsurCTX hkuCTX = new HayvanKriterUnsurCTX();

				KategoriCTX kctx = new KategoriCTX();

                if (item != null)
				{
                    item.user = uctx.userTek("select * from user where id = @id", new { id = item.userId });
                    item.kategori = kctx.KategoriTek("select * from kategori where id = @id", new { id = item.kategoriId });
					item.ozellikler = hkuCTX.HayvanKriterUnsurList("select * from hayvankriterunsur where hayvanId = @hayvanId and isActive = 1", new { hayvanId = item.id });

				}
				return item;
			}
		}

		public int hayvanEkle(Hayvan hayvan)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("insert into hayvan (rfidKodu,kupeIsmi,cinsiyet,agirlik, userId, kategoriId, requestId) values (@rfidKodu,@kupeIsmi,@cinsiyet,@agirlik, @userId, @kategoriId, @requestId)", hayvan);
				return item;
			}
		}

		public int hayvanGuncelle(Hayvan hayvan)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("update hayvan set rfidKodu = rfidKodu,kupeIsmi = @kupeIsmi,cinsiyet = @cinsiyet,agirlik = @agirlik, userId= @userId, kategoriId = @kategoriId, aktif = @aktif, requestId = @requestId where id = @id", hayvan);
				return item;
			}
		}
	}
}
