using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class ProfileCTX
	{
		public List<Profile> profilList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var list = connection.Query<Profile>(sorgu, param).ToList();
				return list;
			}
		}

		public Profile profilTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Query<Profile> (sorgu, param).FirstOrDefault();
				return item;
			}
		}

		public int profilEkle(Profile profil)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("insert into profile (userId,companyName,companyDescription,companyId, address,phoneNumber,cellPhoneNumber,logo) values (@userId,@companyName,@companyDescription,@companyId, @address,@phoneNumber,@cellPhoneNumber,@logo)", profil);
				return item;
			}
		}

		public int profilGuncelle(Profile profil)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("update user set userId = @userId,companyName = @companyName,companyDescription = @companyDescription,companyId = @companyId, address = @address,phoneNumber = @phoneNumber,cellPhoneNumber = @cellPhoneNumber,logo = @logo where id = @id", profil);
				return item;
			}
		}
	}
}
