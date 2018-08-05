using System;
using System.Collections.Generic;
using System.Linq;
using RecipePlanner;

namespace RecipePlanner
{
	class RecipeGraph
	{
		public static List<string> resources = new List<string>();
		public static List<Recipe> recipes = new List<Recipe>();
		public static List<List<int>> adjacenceTo;	//index "leads to" each of its elements
		public static List<List<int>> adjacenceFr;	//index is "lead to" by each of its elements
		
		public static void BuildAdjacence()
		{
			adjacenceTo = new List<List<int>>();
			adjacenceFr = new List<List<int>>();
			for(int i = 0; i < recipes.Count; i++)
			{
				adjacenceTo.Add(new List<int>());
				adjacenceFr.Add(new List<int>());
			}
			
			List<int> fr;
			List<int> to;
			for(int i = 0; i < resources.Count; i++)
			{
				fr = new List<int>();
				to = new List<int>();
				for(int j = 0; j < recipes.Count; j++)
				{
					if(recipes[j].products.ContainsKey(i))
					{
						fr.Add(j);
					}
					if(recipes[j].reagents.ContainsKey(i))
					{
						to.Add(j);
					}
				}
				for(int j = 0; j < fr.Count; j++)
				{
					adjacenceTo[fr[j]].AddRange(to);
				}
				for(int j = 0; j < to.Count; j++)
				{
					adjacenceFr[to[j]].AddRange(fr);
				}
			}
		}
		
		public static void AddRecipe(string name, string machine, double time, Dictionary<string, double> reagents, Dictionary<string, double> products)
		{
			Dictionary<int,double>[] io = new Dictionary<int,double>[2];
			io[0] = new Dictionary<int, double>();
			io[1] = new Dictionary<int, double>();
			
			foreach(KeyValuePair<string, double> reagent in reagents)
			{
				io[0].Add(ResourceNameToID(reagent.Key), reagent.Value);
			}
			foreach(KeyValuePair<string, double> product in products)
			{
				io[1].Add(ResourceNameToID(product.Key), product.Value);
			}
			
			recipes.Add(new Recipe(recipes.Count, name, machine, time, io[0], io[1]));
		}
		
		public static void AddRecipe(string[] lines)
		{
			AddRecipe(lines[0], lines[1], double.Parse(lines[2]), LineToDictionary(lines[3]), LineToDictionary(lines[4]));
		}
		
		static Dictionary<string, double> LineToDictionary(string line)
		{
			Dictionary<string, double> output = new Dictionary<string, double>();
			
			try
			{
				foreach(string element in line.Split(';'))
				{
					output.Add(element.Split(',')[0], double.Parse(element.Split(',')[1]));
				}
			}
			catch(IndexOutOfRangeException i)
			{
				Console.WriteLine(line);
				throw i;
			}
			return output;
		}
		
		static int ResourceNameToID(string name)
		{
			Console.WriteLine("Resolving resource " + name);
			for(int i = 0; i < resources.Count; i++)
			{
				if(resources[i] == name)
				{
					Console.WriteLine("Resource found, ID " + i);
					return i;
				}
			}
			Console.WriteLine("Resource not found; adding resource ID " + resources.Count);
			resources.Add(name);
			return resources.Count - 1;
		}
		
		public static List<int> GetEndProducts()
		{
			List<int> endProducts = new List<int>();
			bool used;
			
			for(int i = 0; i < resources.Count; i++)
			{
				used = false;
				foreach(Recipe r in recipes)
				{
					if(r.reagents.ContainsKey(i))
					{
						used = true;
					}
				}
				if(!used)
				{
					endProducts.Add(i);
				}
			}
			return endProducts;
		}
		
		public static List<int> GetRawMaterials()
		{
			List<int> rawMaterials = new List<int>();
			bool made;
			
			for(int i = 0; i < resources.Count; i++)
			{
				made = false;
				foreach(Recipe r in recipes)
				{
					if(r.products.ContainsKey(i))
					{
						made = true;
					}
				}
				if(!made)
				{
					rawMaterials.Add(i);
				}
			}
			return rawMaterials;
		}
		
		public static string DictionaryPrint(Dictionary<int, double> input)
		{
			string output = "";
			bool first = true;
			
			foreach(KeyValuePair<int, double> element in input)
			{
				if(!first)
				{
					output += ";";
				}
				output += RecipeGraph.resources[element.Key] + "," + element.Value;
				first = false;
			}
			return output;
		}
	}
}
