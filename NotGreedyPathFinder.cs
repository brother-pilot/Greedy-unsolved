using System.Collections.Generic;
using System.Drawing;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;
using System.Linq;
using System;

namespace Greedy
{
	public class NotGreedyPathFinder : IPathFinder
	{
		public List<Point> FindPathToCompleteGoal(State state)
		{	
			if (state.Chests.Count == 0 || state.Energy == 0 || state.Chests.Count < state.Goal)
				return new List<Point>();
			var paths = new DijkstraPathFinder();
			var chestArray = state.Chests.ToArray();
			var trackEval = new Dictionary<string, PathWithCost>();
			var result = new List<int[]>();
			MakePermutations(new int[state.Chests.Count], 0, state, result);
			int bestPrice = int.MaxValue;
			int[] intItem=new int[state.Chests.Count];
			int bestChests=int.MinValue;
			foreach (var item in result)
            {
				int price = 0;
				int chests = 0;
				Point start = state.Position;
				Point end = chestArray[0];
				for (int i = 0; i < item.Length; i++)
                {
					end = chestArray[item[i]];
					chests++;
					var startEnd = start.ToString() + end.ToString(); 
					if (!trackEval.ContainsKey(startEnd))
						trackEval[startEnd] = paths.GetPathsByDijkstra(state, start, new List<Point> { end })
							.FirstOrDefault();
					price += trackEval[startEnd].Cost;
					int y;
					if (chests == 7)
						y = 0;
					if (price > state.Energy|| chests> chestArray.Length+1)
					{
						price -= trackEval[startEnd].Cost;
						chests--;
						break;
					}
					start = end;
				}
				if (chests> bestChests)
				{
					bestPrice = price;
					intItem = item;
					bestChests = chests;
				}
				if (bestPrice == state.InitialEnergy && bestChests == chestArray.Length) break;
			}
			return CovertResult(intItem, trackEval, state.Position, chestArray, bestChests);
		}

		static List<Point> CovertResult(int[] intItem, Dictionary<string,PathWithCost> trackEval, 
			Point start, Point[] chestArray, int chests)
        {
			var result = new List<Point>();
			for (int k = 0; k < chests; k++)
			{
				Point end = chestArray[intItem[k]];
				var startEnd = start.ToString() + end.ToString();
				foreach (var point in trackEval[startEnd].Path.Skip(1))
                {
					result.Add(point);
				}
				start = end;
			}
			return result;
		}

		static void MakePermutations(int[] permutation, int countChests,State state, List<int[]> result)
		{
			if (countChests == permutation.Length)
			{
				var arrVar = new int[permutation.Length]; //интересный момент как удалось отвязать ссылку на permutation при копировании в список result
				for (int i = 0; i < permutation.Length; i++)
					arrVar[i] = permutation[i];
				result.Add(arrVar);
				return;
			}
			for (int i = 0; i < permutation.Length; i++)
			{
				var index = Array.IndexOf(permutation, i, 0, countChests);
				if (index != -1)
					continue;
				permutation[countChests] = i;
				MakePermutations(permutation, countChests + 1, state, result);
			}
		}
	}
}