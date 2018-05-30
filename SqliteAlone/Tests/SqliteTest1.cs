using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NFluent;
using NUnit.Framework;
using SQLite;

namespace SqliteAlone.Tests
{
	[TestFixture]
	public class SqliteTest1
	{
		private const string SQLITE_DB_FILE_DIR = @"c:\tmp\sqlite";
		private static readonly string SQLITE_DB_FILE = Path.Combine(SQLITE_DB_FILE_DIR, "sqlite");
		private SQLiteConnection _connection;

		[Test]
		public void ReallySimpleTestFromDocSqliteNet_Stock()
		{
			_connection.CreateTable<Stock>();
			_connection.CreateTable<Valuation>();

			//AddStock
			AddStock(_connection, "€");

			List<Stock> roles = _connection.Table<Stock>().ToList();
			Check.That(roles).CountIs(1);
		}

		[Test]
		public void Sqlite_simple_Personne()
		{
			_connection.CreateTable<Role>();
			_connection.CreateTable<Personne>();
			// Utilisation de l'API asynchrone
//			SQLiteAsyncConnection connAsync = new SQLiteAsyncConnection("myDb.db3");
//			connAsync.CreateTableAsync<Role>();
//			connAsync.CreateTableAsync<Personne>();

			AddRoleAndPersonne(_connection);
		}

		[TearDown]
		public void TearDown()
		{
			_connection.Dispose();
			_connection.Close();
			try
			{
				File.Delete(SQLITE_DB_FILE);
			}
			finally
			{
				GC.Collect();
			}
		}

		[SetUp]
		public void SetUp()
		{
			Directory.CreateDirectory(SQLITE_DB_FILE_DIR);
			_connection = new SQLiteConnection(SQLITE_DB_FILE);
		}

		public void AddRoleAndPersonne(SQLiteConnection conn)
		{
			Role r1 = new Role {Name = "Administrator"};
			Role r2 = new Role {Name = "Visitor"};

			var res1 = conn.Insert(r1);
			var res2 = conn.Insert(r2);

			Personne p1 = new Personne {LastName = "Pierre", FirstName = "Paul", RoleId = r1.Id};
			conn.Insert(p1);

			// Des méthodes similaires existent pour les opérations Update et Delete

			List<Role> roles = conn.Table<Role>().Where(x => x.Name == "Administrator").ToList();
			Check.That(roles).CountIs(1);

			IEnumerable<Personne> personnes = conn.Query<Personne>("SELECT * FROM People WHERE RoleId = ?", r1.Id);
			Check.That(personnes).CountIs(1);
		}

		public static void AddStock(SQLiteConnection db, string symbol)
		{
			var Id = db.Insert(new Stock
			{
				Symbol = symbol
			});
			Console.WriteLine("{0}", Id);
		}

		public static IEnumerable<Valuation> QueryValuations(SQLiteConnection db, Stock stock)
		{
			return db.Query<Valuation>("select * from Valuation where StockId = ?", stock.Id);
		}
	}
}