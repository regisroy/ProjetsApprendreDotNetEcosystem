using System;
using SQLite;

namespace SqliteAlone
{
	public class Stock
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Symbol { get; set; }
	}

	public class Valuation
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[Indexed]
		public int StockId { get; set; }

		public DateTime Time { get; set; }
		public decimal Price { get; set; }
	}
	
	[Table("People")]
	public class Personne 
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
	
		[Column("Name")]
		public string LastName { get; set; }
	
		[MaxLength(50)]
		public string FirstName { get; set; }
	
		[Indexed]
		public int RoleId { get; set; }
	}

	public class Role // Je ne spécifie pas de nom, la table prendra donc le nom de la classe
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
	
		public string Name { get; set; }
	
		[Ignore] // J'ai choisi de ne pas stocker ce champ en BDD
		public bool IsUsed { get; set; }
	}
}