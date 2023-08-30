﻿using CYS.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace CYS.Repos
{
	public class olcumCTX
	{
		public List<olcum> olcumList(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var list = connection.Query<olcum>(sorgu, param).ToList();
				
				return list;
			}
		}

		public olcum olcumTek(string sorgu, object param)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Query<olcum>(sorgu, param).FirstOrDefault();

				return item;
			}
		}

		public int insert(olcum ol)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("insert into olcum (olcumSessionId, adet, sonGuncelleme) values (@olcumSessionId, @adet, @sonGuncelleme)", ol);
				return item;
			}
		}

		public int update(olcum hayvan)
		{
			using (var connection = new MySqlConnection("Server=localhost;Database=cys;User Id=root;Password=Muhamm3d!1;"))
			{
				var item = connection.Execute("update  olcum set olcumSessionId = @olcumSessionId,adet=@adet, sonGuncelleme = @sonGuncelleme where id = @id", hayvan);
				return item;
			}
		}
	}
}