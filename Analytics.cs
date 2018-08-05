using System;
using System.Collections.Generic;
using System.Diagnostics;
using RecipePlanner;

namespace RecipePlanner
{
	class Analytics
	{
		public static Queue<KeyValuePair<int, double>> rQueue = new Queue<KeyValuePair<int, double>>();	//Resource ID, quantity
		public static Dictionary<int, double> rawCounts;
		public static List<KeyValuePair<int, double>> steps;
		
		public static void Run()
		{
			KeyValuePair<int, double> currentResource;
			bool made;
			int timeout = 2;
			Stopwatch timer = new Stopwatch();
			rawCounts = new Dictionary<int,double>();
			List<Recipe> matches;
			int matchNumber;
			steps = new List<KeyValuePair<int,double>>();
			
			timer.Start();
			while(rQueue.Count != 0 )// && timer.ElapsedMilliseconds < timeout)
			{
				currentResource = rQueue.Dequeue();
				made = false;
				matches = new List<Recipe>();
				foreach(Recipe rec in RecipeGraph.recipes)
				{
					if(rec.products.ContainsKey(currentResource.Key))
					{
						made = true;
						matches.Add(rec);
					}
				}
				if(!made)
				{
					AddRaw(currentResource);
				}else
				{
					if(matches.Count == 1)
					{
						matchNumber = 0;
					}else
					{
						//choose one
						Console.WriteLine("Multiple recipes produce {0} (need {1}).\nChoose a recipe from the following or -1 for raw.", RecipeGraph.resources[currentResource.Key], currentResource.Value);
						for(int i = 0; i < matches.Count; i++)
						{
							Console.Write(i + ": ");
							Console.WriteLine(matches[i]);
							Console.WriteLine();
						}
						matchNumber = int.Parse(Console.ReadLine());
						timer.Restart();
						Console.WriteLine();
					}
					if(matchNumber == -1)
					{
						AddRaw(currentResource);
						continue;
					}
					foreach(KeyValuePair<int, double> res in matches[matchNumber].reagents)
					{
						rQueue.Enqueue(new KeyValuePair<int,double>(res.Key, res.Value * currentResource.Value / matches[matchNumber].products[currentResource.Key]));
					}
					steps.Add(new KeyValuePair<int, double>(
						matches[matchNumber].id,
						matches[matchNumber].time * currentResource.Value / matches[matchNumber].products[currentResource.Key]));
				}
			}
			if(timer.ElapsedMilliseconds >= timeout)
			{
				Console.WriteLine("Loop detected");
				foreach(KeyValuePair<int, double> loopMat in rQueue)
				{
					AddRaw(loopMat);
				}
			}
			timer.Stop();
		}
		
		static void AddRaw(KeyValuePair<int, double> raw)
		{
			if(rawCounts.ContainsKey(raw.Key))
			{
				rawCounts[raw.Key] += raw.Value;
				return;
			}
			rawCounts.Add(raw.Key, raw.Value);
		}
	}
}
