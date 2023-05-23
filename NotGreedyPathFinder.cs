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
			//MakePermutations(new int[state.Chests.Count], 0, state, result);
			int bestPrice = int.MaxValue;
			int[] intItem=new int[state.Chests.Count];
			int bestChests=int.MinValue;
			foreach (var item in MakePermutations(new int[state.Chests.Count], 0, state))
            {
				var arrVar = new int[item.Length];
				arrVar = item.Select(x => x ).ToArray();
				result.Add(arrVar);
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
				if (bestPrice < state.InitialEnergy && bestChests == chestArray.Length) break;
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

		static IEnumerable<int[]> MakePermutations(int[] permutation, int countChests,State state)
		{
			//List<int[]> result;
			//if (countChests == permutation.Length)
			//{
			//	var arrVar = new int[permutation.Length]; //интересный момент как удалось отвязать ссылку на permutation при копировании в список result
			//	for (int i = 0; i < permutation.Length; i++)
			//		arrVar[i] = permutation[i];
			//	//result.Add(arrVar);
			//	yield return arrVar;
			//}
			//for (int i = 0; i < permutation.Length; i++)
			//{
			//             for (int j = 0; j < countChests; j++)
			//             {
			//		permutation[countChests] = i;
			//	}
			//	var index = Array.IndexOf(permutation, i, 0, countChests);
			//	if (index != -1)
			//		continue;
			//	permutation[countChests] = i;
			//	MakePermutations(permutation, countChests + 1, state);
			//}
			bool Less<T>(T value_0, T value_1) where T : IComparable
			{
				return value_0.CompareTo(value_1) < 0;
			}		

			var sequence = permutation;
			InitSequence(sequence);
			yield return sequence;
			while (Narayana.NextPermutation(sequence, Less))
			{
				var ttt = sequence;
				yield return sequence;
			} 
			// x < y — критерий сравнения для неубывающей последовательности
		}

		private static void InitSequence(int[] sequence)
		{
			// Заполнение последовательности значениями 1, 2, 3…
			for (var i = sequence.Length-1; i > -1; --i)
				sequence[i] = i;
		}
	}

	public static class Narayana
	{
		/// <summary>
		/// Функция, задающая отношение порядка для значений типа T: < либо >
		/// </summary>
		public delegate bool Predicate2<T>(T value_0, T value_1);

		/// <summary>
		/// Поиск очередной перестановки
		/// </summary>
		public static bool NextPermutation<T>(T[] sequence, Predicate2<T> compare)
		{
			// Этап № 1
			var i = sequence.Length;
			do
			{
				if (i < 2)
					return false; // Перебор закончен
				--i;
			} while (!compare(sequence[i - 1], sequence[i]));
			// Этап № 2
			var j = sequence.Length;
			while (i < j && !compare(sequence[i - 1], sequence[--j])) ;
			_SwapItems(sequence, i - 1, j);
			// Этап № 3
			j = sequence.Length;
			while (i < --j)
				_SwapItems(sequence, i++, j);
			return true;
		}

		/// <summary>
		/// Обмен значениями двух элементов последовательности
		/// </summary>
		private static void _SwapItems<T>(T[] sequence, int index_0, int index_1)
		{
			var item = sequence[index_0];
			sequence[index_0] = sequence[index_1];
			sequence[index_1] = item;
		}
	}
}