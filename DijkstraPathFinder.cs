using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Greedy.Architecture;
using System.Drawing;
/*
 * На этот раз в лабиринт с сокровищами попал Жадина! Лабиринт очень старый и все свободные клетки успели покрыться зарослями. Для каждой клетки лабиринта известна её трудность — количество сил, которые потратит Жадина на прохождение зарослей этой клетки.

После каждого шага силы Жадины уменьшаются на трудность клетки, в которую он шагнул. Когда силы заканчиваются, Жадина останавливается.

Посмотреть на лабиринты можно запустив проект

Выбрать лабиринт можно в меню States слева сверху.
Цифры и яркость на клетках — это трудность.
Жадина хочет узнать, какое минимальное количество сил ему нужно потратить чтобы дойти от текущей клетки до какого-то из сундуков. Причём, он ещё не решил, какой из сундуков он хочет посетить, так что Жадина хочет узнать кратчайшее расстояние до каждого из сундуков.

Помогите Жадине найти кратчайшие пути до каждого из сундуков!

Для того, чтобы сдать задачу, в файле DijkstraPathFinder.cs реализуйте метод GetPathsByDijkstra. Он должен возвращать пути до сундуков в виде IEnumerable в порядке увеличения трудности пути до них. При этом вычислять пути он должен лениво, то есть не вычислять пути до далёких сундуков, пока их не запросили из IEnumerable.

Описание лабиринта передаётся в метод в объекте типа State:

в поле CellCost находится двумерный массив трудностей всех клеток (0 означает стену).
есть методы, которые помогут проверить, что какая-то клетка является стеной или находится внутри лабиринта.
Все тесты в классе DijkstraPathFinder_Should должны завершиться успехом
*/

//использование алгоритма Дейкстры
namespace Greedy
{
    public class DijkstraPathFinder
    {
        public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start,
            IEnumerable<Point> targets)
        {
            HashSet<Point> notOpened = new HashSet<Point>();//будем их добавлять лениво
            notOpened.Add(start);
            HashSet<Point> visitedNodes = new HashSet<Point>();
            var track = new Dictionary<Point, DijkstraData>();//будет хранится информация об оценках для каждой вершины
            track[start] = new DijkstraData { Cost = 0, Previous = null };//заносим инфу о нулевой вершине

            while (true)
            {
                Point? toOpen = null;//открываемая вершина. делаем типом nullable чтобы можно было null
                var bestPrice = double.PositiveInfinity;
                foreach (var e in notOpened)//для каждой вершины в непосещенных
                {
                    if (track.ContainsKey(e) && track[e].Cost < bestPrice)
                    {
                        bestPrice = track[e].Cost;
                        toOpen = e;
                    }
                }
                if (toOpen == null || targets.Count() == 0) yield break;
                if (targets.Contains(toOpen.Value))
                {
                    yield return ConvertResult(track, (Point)toOpen);//выход из бесконечного цикла здесь
                }

                foreach (var e in PossiblePoint(toOpen.Value, state))//пройдемся по всем ребрам у которых эта вершина является начальной
                {
                    var currentPrice = track[toOpen.Value].Cost + state.CellCost[e.X, e.Y];//плюс вес ребра
                    if (!track.ContainsKey(e) || track[e].Cost > currentPrice)//если для вершины не найдена вообще никакая оценка или найденная оценка хуже чем для посчитанной вершины
                    {
                        track[e] = new DijkstraData { Previous = toOpen, Cost = currentPrice };//запишем новую цену и вершину из которой пришли//запишем новую цену и вершину из которой пришли
                    }
                    if (!visitedNodes.Contains(e)) notOpened.Add(e);
                }
                notOpened.Remove(toOpen.Value);//или (Point)toOpen
                visitedNodes.Add(toOpen.Value);
            }
        }

        PathWithCost ConvertResult(Dictionary<Point, DijkstraData> track, Point node)
        {
            var result = new List<Point>();
            var end = node;
            while (true)
            {
                result.Add(node);
                if (track[node].Previous == null) break;
                node = track[node].Previous.Value;
            }
            result.Reverse();
            return new PathWithCost(track[end].Cost, result.ToArray());
        }

        public List<Point> PossiblePoint(Point point, State state)
        {
            var direction = new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
            var result = new List<Point>();
            for (int j = 0; j <= 3; j++)
            {
                var newPoint = new Point
                { X = point.X + direction[j, 0], Y = point.Y + +direction[j, 1] };
                if (state.InsideMap(newPoint) && !state.IsWallAt(newPoint))
                {
                    result.Add(newPoint);
                }
            }
            return result;
        }
        public IEnumerable<Point> GetIncidentNodes(Point point, State state)
        {
            return new Point[]
            {
                new Point(point.X, point.Y+1),
                new Point(point.X, point.Y-1),
                new Point(point.X+1, point.Y),
                new Point(point.X-1, point.Y)
            }.Where(p => state.InsideMap(p) && !state.IsWallAt(p));
        }
    }

    class DijkstraData
    {
        public Point? Previous { get; set; }//как пришли в эту вершину. Можно было обойтись полями т.к. чисто служебный класс
        public int Cost { get; set; }//цена вершины
    }

}