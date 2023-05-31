using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Greedy.Architecture;
using System.Drawing;

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