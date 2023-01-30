using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class UserCTX
	{
		public List<User> userList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var list = connection.Query<User>(sorgu, param).ToList();
				return list;
			}
		}

		public User userTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Query<User>(sorgu, param).FirstOrDefault();
				return item;
			}
		}

		public int userEkle(Profile profil)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("insert into user (userName, password) values (@userName, @password)", profil);
				return item;
			}
		}

		public int userGuncelle(Profile profil)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("update user set userName = @userName, password = @password where id = @id", profil);
				return item;
			}
		}
	}
}
