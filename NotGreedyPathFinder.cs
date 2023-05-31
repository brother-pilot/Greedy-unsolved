using System.Collections.Generic;
using System.Drawing;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;
using System.Linq;
using System;
 
namespace Greedy
{
    class DataNotGreedy
    {
        public int Price { get; set; }
        public int Chests { get; set; }
        public Point Previous { get; set; }
        public List<Point> ChestList { get; set; }   
    }
    public class NotGreedyPathFinder : IPathFinder
    {
        public List<Point> FindPathToCompleteGoal(State state)
        {
            if (state.Chests.Count == 0 || state.Energy == 0 || state.Chests.Count < state.Goal)
                return new List<Point>();

            //var ttt = paths.GetPathsByDijkstra(state, state.Position, chestArray2);
            //foreach (var item in ttt)
            //{
            //    trackEval[state.Position.ToString() + item.End.ToString()] = item;
            //    if (item.Cost > state.Energy) chestArray2.Remove(item.End);
            //}
            //         var result = new List<int[]>();
            //MakePermutations(new int[chestArray2.Count], 0, state, result);

            //foreach (var item in result)
            //         {
            //    int price = 0;
            //    int chests = 0;
            //    Point start = state.Position;
            //    Point end = chestArray2[0];
            //    for (int i = 0; i < item.Length; i++)
            //             {
            //        end = chestArray2[item[i]];
            //        chests++;
            //        var startEnd = start.ToString() + end.ToString(); 
            //        if (!trackEval.ContainsKey(startEnd))
            //            trackEval[startEnd] = paths.GetPathsByDijkstra(state, start, new List<Point> { end })
            //                .FirstOrDefault();
            //        price += trackEval[startEnd].Cost;
            //        if (price > state.Energy|| chests> chestArray2.Count+1)
            //        {
            //            price -= trackEval[startEnd].Cost;
            //            chests--;
            //            break;
            //        }
            //        start = end;
            //    }
            //    if (chests> bestChests)
            //    {
            //        bestPrice = price;
            //        intItem = item;
            //        bestChests = chests;
            //    }
            //    if (bestPrice == state.InitialEnergy && bestChests == chestArray2.Count) break;
            //}

            //public static IEnumerable<Node> DepthSearch_Correct(Point startNode)

            var paths = new DijkstraPathFinder();
            var trackEval = new Dictionary<string, PathWithCost>();
            var pointEval = new Dictionary<string, DataNotGreedy>();
            var stack = new Stack<Point>();
            stack.Push(state.Position);
            Point previousPoint = state.Position;
            pointEval[state.Position.ToString()+ state.Position.ToString()] = new DataNotGreedy { Price=0,
                Chests=0, ChestList=state.Chests.ToList(), Previous= state.Position};
            int bestPrice = int.MaxValue;
            int[] intItem = new int[state.Chests.Count];
            Point intPoint = state.Position;
            int bestChests = int.MinValue;
            var visited= new HashSet<String>();
            //visited.Add(state.Position);
            while (stack.Count != 0)
            {

                var point = stack.Pop();
                var startPrev= point.ToString() + previousPoint.ToString();
                int price = pointEval[startPrev].Price;
                int chests = pointEval[startPrev].Chests;
                var chestArray2 = pointEval[startPrev].ChestList.ToList(); //благодаря toList отвязываем копирование по ссылке
                chestArray2.Remove(point);
                //yield return point;
                //result.Add(point);
                List<PathWithCost> possible = new List<PathWithCost>();
                if (!visited.Contains(point.ToString()))
                    possible = paths.GetPathsByDijkstra(state, point, chestArray2).ToList();
                else {
                    continue;
                    foreach (var item in chestArray2)
                    {
                        var yy = item.ToString() + point.ToString();
                        var ttt = trackEval[item.ToString() + point.ToString()];
                        possible.Add(ttt);
                    }
                        }
                visited.Add(point.ToString());
                //var chestArray3=possible.
                //foreach (var nextPoint in paths.GetPathsByDijkstra(state, point, chestArray2).Where(n => !visited.Contains(n.End)))
                foreach (var nextPoint in possible.Select(x=>x.End))
                {
                    //visited.Add(nextPoint.End);
                    var chestArray = chestArray2.ToList();
                    price = pointEval[startPrev].Price;
                    chests = pointEval[startPrev].Chests;
                    stack.Push(nextPoint);
                    var startEnd = point.ToString() + nextPoint.ToString();
                    var endStart = nextPoint.ToString() + point.ToString();
                    if (!trackEval.ContainsKey(startEnd))
                    {
                        trackEval[startEnd] = possible.Where(x=>x.End==nextPoint).FirstOrDefault();
                        trackEval[endStart] = possible.Where(x => x.End == nextPoint).FirstOrDefault();
                    }
                    //else break;
                    chests++;
                    price += trackEval[startEnd].Cost;
                    chestArray.Remove(nextPoint);
                    pointEval[nextPoint.ToString()+point.ToString()] = new DataNotGreedy { Price = price, 
                        Chests = chests, ChestList = chestArray.ToList(), Previous = point };
                    if (price > state.Energy || chests > chestArray.Count + 1)
                    {
                        price -= trackEval[startEnd].Cost;
                        chests--;
                        break;
                    }
                    if (chests > bestChests)
                    {
                        bestPrice = price;
                        intPoint = stack.Peek();
                        //intItem = item;
                        bestChests = chests;
                    }

                }
                if (chests > bestChests)
                {
                    bestPrice = price;
                    intPoint = stack.Peek();
                    //intItem = item;
                    bestChests = chests;
                }
                previousPoint = pointEval[startPrev].Previous;
            }

            return CovertResult(intPoint, trackEval, state.Position, state.Chests.ToArray(), bestChests);
        }

        static List<Point> CovertResult(Point intPoint, Dictionary<string, PathWithCost> trackEval,
            Point start, Point[] chestArray, int chests)
        {
            var result = new List<Point>();
            for (int k = 0; k < chests; k++)
            {
                Point end = intPoint;
                var startEnd = start.ToString() + end.ToString();
                foreach (var point in trackEval[startEnd].Path.Skip(1))
                {
                    result.Add(point);
                }
                start = end;
            }
            return result;
        }


    }
}