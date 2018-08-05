using System;
using System.Collections.Generic;
using System.IO;
using RecipePlanner;

namespace RecipePlanner
{
	class Driver
	{
		public static void Main()
		{
			string inputString = "";
			Console.WriteLine("Read from file?");
			if(Console.ReadLine().ToLower() == "y")
			{
				Console.WriteLine("Input file");
				string[] inputFile = File.ReadAllLines(Console.ReadLine());
				for(int i = 0; i < inputFile.Length / 5; i++)
				{
					RecipeGraph.AddRecipe(new string[5]{inputFile[(5*i)],inputFile[(5*i)+1],inputFile[(5*i)+2],inputFile[(5*i)+3],inputFile[(5*i)+4]});
				}
			}
			for(;;)
			{
				string name;
				string machine;
				double time;
				Dictionary<string, double> reagents = new Dictionary<string, double>();
				Dictionary<string, double> products = new Dictionary<string, double>();
				Console.WriteLine("Add a new recipe?");
				if(Console.ReadLine().ToLower() == "n")
				{
					break;
				}
				Console.WriteLine("Recipe Name");
				name = Console.ReadLine();
				Console.WriteLine("Machine");
				machine = Console.ReadLine();
				Console.WriteLine("Time");
				time = double.Parse(Console.ReadLine());
				for(;;)
				{
					Console.WriteLine("Add reagent?");
					if(Console.ReadLine().ToLower() == "n")
					{
						break;
					}
					Console.WriteLine("Input reagent: \"resource,amount\"");
					inputString = Console.ReadLine();
					reagents.Add(inputString.Split(',')[0], int.Parse(inputString.Split(',')[1]));
				}
				for(;;)
				{
					Console.WriteLine("Add product?");
					if(Console.ReadLine().ToLower() == "n")
					{
						break;
					}
					Console.WriteLine("Input product: \"resource,amount\"");
					inputString = Console.ReadLine();
					products.Add(inputString.Split(',')[0], double.Parse(inputString.Split(',')[1]));
				}
				RecipeGraph.AddRecipe(name, machine, time, reagents, products);
			}
			Console.WriteLine(RecipeGraph.recipes.Count + " recipe(s) processed");
			Console.WriteLine(RecipeGraph.resources.Count + " resource(s) processed");
			foreach(Recipe r in RecipeGraph.recipes)
			{
				Console.WriteLine(r);
			}
			Console.WriteLine("\nBuilding adjacence tables.");
			RecipeGraph.BuildAdjacence();
			List<int> raw = RecipeGraph.GetRawMaterials();
			Console.WriteLine("Raw Materials ({0}):", raw.Count);
			bool first = true;
			foreach(int res in raw)
			{
				if(!first)
				{
					Console.Write(',');
				}
				first = false;
				Console.Write(RecipeGraph.resources[res]);
			}
			List<int> end = RecipeGraph.GetEndProducts();
			Console.WriteLine("\n\nEnd Products ({0}):", end.Count);
			first = true;
			foreach(int res in end)
			{
				if(!first)
				{
					Console.Write(',');
				}
				first = false;
				Console.Write(RecipeGraph.resources[res]);
			}
			
			for(;;)
			{
				Console.WriteLine("\nSelect a resource to inspect:");
				for(int i = 0; i < RecipeGraph.resources.Count; i++)
				{
					Console.WriteLine(i + ": " + RecipeGraph.resources[i]);
				}
				int selection = int.Parse(Console.ReadLine());
				
				Console.WriteLine("Input desired item rate");
				double rate = double.Parse(Console.ReadLine());
				
				Console.WriteLine();
				Analytics.rQueue.Enqueue(new KeyValuePair<int, double>(selection, rate));
				Analytics.Run();
				Console.WriteLine("{0} {1}/s requires:", rate, RecipeGraph.resources[selection]);
				Console.WriteLine(RecipeGraph.DictionaryPrint(Analytics.rawCounts));
				
				Console.WriteLine("Steps:");
				foreach(KeyValuePair<int, double> step in Analytics.steps)
				{
					Console.WriteLine("{0} in {1} ({2})", RecipeGraph.recipes[step.Key].name, RecipeGraph.recipes[step.Key].machine, step.Value);
				}
				
				Console.WriteLine("\nInspect another resource?");
				if(Console.ReadLine().ToLower() == "n")
				{
					break;
				}
			}
		}
	}
}
