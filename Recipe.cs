using System;
using System.Collections.Generic;
using RecipePlanner;

namespace RecipePlanner
{
	struct Recipe
	{
		public int id;
		public string name;
		public double time;
		public string machine;
		public Dictionary<int, double> reagents;	//Resource ID, quantity
		public Dictionary<int, double> products;
		
		public Recipe(int id, string name, string machine, double time, Dictionary<int, double> reagents, Dictionary<int, double> products)
		{
			this.id = id;
			this.name = name;
			this.machine = machine;
			this.time = time;
			this.reagents = reagents;
			this.products = products;
		}
		
		public override string ToString()
			=> name + "\n" + machine + "\n" + time + "\n" + RecipeGraph.DictionaryPrint(reagents) + "\n" + RecipeGraph.DictionaryPrint(products);
	}
}
