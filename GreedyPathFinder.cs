using System.Collections.Generic;
using System.Drawing;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;
using System.Linq;

/*
 * Чтобы жизнь удалась, Жадине нужно собрать N сундуков. При этом, Жадина отказывается идти к сундуку, если есть другой сундук, путь до которого потребует меньше сил: он не только жадина, но и лентяй!

Помогите Жадине собрать N сундуков!

После выполнения этой задачи, Жадина начнёт передвигаться по лабиринту после запуска приложения.

Для того, чтобы сдать задачу, в файле GreedyPathFinder.cs реализуйте метод FindPathToCompleteGoal. Он должен возвращать путь передвижения Жадины. Путь не должен содержать исходную позицию — ту из которой Жадина начинает движение. Если подходящего пути не существует, метод должен возвращать пустой список.

Текущее состояние уровня передается в метод в объекте типа State.

Используйте класс DijkstraPathFinder, реализованный в предыдущей задаче. Его не нужно включать в отправляемый на проверку файл, считайте, что этот класс уже есть в проекте. При проверке этой задачи будет использоваться авторская реализация DijkstraPathFinder, а не ваша.

Гарантируется, что если рассмотреть множество всех сундуков и добавить в него исходную позицию, то в нём не существует тройки A, B, C, такой, что от А добраться до B так же трудно, как и от A до C. Другими словами, у Жадины всегда есть только один вариант дальнейших действий.

Тесты в классах GreedyPathFinder_Should и GreedyTimeLimit_Tests должны завершаться успехом.
*/


//Использование жадной стратегии для решения задачи Коммивояжера

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