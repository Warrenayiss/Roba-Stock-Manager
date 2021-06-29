using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roba_Stock_Manager.Classes
{
	public class Product
	{
		public string Name { get; set; }
		public int Quantity { get; set; }
		public int UnitPrice { get; set; }
		public int TotalPrice { get; set; }


		public Product(string name, int quantity)
		{
			Name = name;
			Quantity = quantity;
		}
	}
}
