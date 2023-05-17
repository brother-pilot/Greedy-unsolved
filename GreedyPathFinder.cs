using System.Collections.Generic;
using System.Drawing;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;
using System.Linq;

namespace Greedy
{
	public class GreedyPathFinder : IPathFinder
	{
		public List<Point> FindPathToCompleteGoal(State state)
		{
			if (state.Chests.Count == 0 || state.Energy == 0 || state.Chests.Count < state.Goal)
				return new List<Point>();
			var paths = new DijkstraPathFinder();
			var chests = state.Chests;
			var result = new List<Point>();
			for (int i = 0; i < state.Goal && chests.Count != 0; i++)
			{
				Point start;
				if (i == 0)
					start = state.Position;
				else
					start = result.Last();
				var pathToChest = paths.GetPathsByDijkstra(state, start, chests).FirstOrDefault();
				if (pathToChest.Cost > state.InitialEnergy)
					return new List<Point>();
				foreach (var item in pathToChest.Path.Skip(1).ToList())
				{
					result.Add(item);
				}
				chests.Remove(result.Last());
			}
			return result;
		}
	}
}