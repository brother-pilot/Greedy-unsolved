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
            var targets2 = targets;
            HashSet<Point> notVisited = new HashSet<Point>();
            for (int i = 0; i < state.CellCost.GetLength(0); i++)
            {
                for (int j = 0; j < state.CellCost.GetLength(1); j++)
                {
                    if (!state.IsWallAt(new Point(i, j))) notVisited.Add(new Point(i, j));
                }
            }
            var track = new Dictionary<Point, DijkstraData>();//будет хранится информация об оценках для каждой вершины
            track[start] = new DijkstraData { Cost = 0, Previous = null };//заносим инфу о нулевой вершине

            while (true)
            {
                Point? toOpen = null;//открываемая вершина
                var bestPrice = double.PositiveInfinity;
                foreach (var e in notVisited)//для каждой вершины в непосещенных
                {
                    if (track.ContainsKey(e) && track[e].Cost < bestPrice)
                    {
                        bestPrice = track[e].Cost;
                        toOpen = e;
                    }
                }
                if (toOpen == null) yield break;
                if (targets2.Contains((Point)toOpen))
                {
                    yield return ConvertResult(track, (Point)toOpen);//выход из бесконечного цикла здесь
                    targets2 = targets2.Where(x => x != toOpen);
                }

                foreach (var e in PossiblePoint((Point)toOpen, state))//пройдемся по всем ребрам у которых эта вершина является начальной
                {
                    var currentPrice = track[(Point)toOpen].Cost + state.CellCost[e.X, e.Y];//плюс вес ребра
                    if (!track.ContainsKey(e) || track[e].Cost > currentPrice)//если для вершины не найдена вообще никакая оценка или найденная оценка хуже чем для посчитанной вершины
                    {
                        track[e] = new DijkstraData { Previous = toOpen, Cost = currentPrice };//запишем новую цену и вершину из которой пришли//запишем новую цену и вершину из которой пришли
                        if (targets2.Contains(e))
                        {
                            yield return ConvertResult(track, e);//выход из бесконечного цикла здесь
                            targets2 = targets2.Where(x => x != e);
                        }
                    }
                }
                notVisited.Remove((Point)toOpen);
            }
        }

        PathWithCost ConvertResult(Dictionary<Point, DijkstraData> track, Point node)
        {
            var result = new List<Point>();
            var end = node;
            while (true)
            {
                result.Add(node);
                if (track[node].Previous==null) break;
                node = (Point)track[node].Previous;
            }
            result.Reverse();
            return new PathWithCost(track[end].Cost, result.ToArray());
        }

        List<Point> PossiblePoint(Point point, State state)
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
    }

    class DijkstraData
    {
        public Point? Previous { get; set; }//как пришли в эту вершину. Можно было обойтись полями т.к. чисто служебный класс
        public int Cost { get; set; }//цена вершины
    }


}