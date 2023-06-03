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
        //глобальные перменные для хранения найденного самого оптимального пути
        static int bestPrice = int.MaxValue;
        static int bestChests = int.MinValue;
        static List<Point> bestTrack = new List<Point>();
        public List<Point> FindPathToCompleteGoal(State state)
        {
            if (state.Chests.Count == 0 || state.Energy == 0 || state.Chests.Count < state.Goal)
                return new List<Point>();
            var trackEval = new Dictionary<(Point, Point), PathWithCost>();
            //запускаем полный перебор по сундукам с поиском в глубину 
            MakePath(state, trackEval, new List<Point>(), state.Position, state.Chests.ToList());
            return CovertResult(state, trackEval);
        }

        void MakePath(State state, Dictionary<(Point, Point), PathWithCost> trackEval,
            List<Point> track2, Point start, List<Point> chestList, int price = 0, int chests = 0)
        {
            var paths = new DijkstraPathFinder();
            var chestLocalList = chestList.ToList();
            List<Point> track = track2.ToList(); 
            foreach (var chest in chestList)
            {
                //уменьшаем число вычислений сохраняя найденне переходы от сундука к сундуку
                if (!trackEval.ContainsKey((start, chest)))
                {
                    trackEval[(start, chest)] =
                    paths.GetPathsByDijkstra(state, start, new List<Point> { chest }).FirstOrDefault();
                }
                chests++;
                price += trackEval[(start, chest)].Cost;
                track.Add(chest);
                chestLocalList.Remove(chest);
                //проверяем на отсечение получаемый вариант
                if (price > state.Energy || chests > state.Chests.Count + 1)
                {
                    price -= trackEval[(start, chest)].Cost;
                    chests--;
                    track.Remove(chest);
                    BestChase(price, chests, track);
                    continue;
                }
                BestChase(price, chests, track);
                MakePath(state, trackEval, track,chest, chestLocalList, price, chests);
                chestLocalList.Add(chest);
                //возращаем все в исходное состояние в этом цикле перебора сундука чтобы цена
                //и кол-во сундуков были те же
                price -= trackEval[(start, chest)].Cost;
                chests--;
                //возращаем все в исходное состояние в этом цикле перебора сундука чтобы не было циклов
                track.Remove(chest);
            }
        }

        static void BestChase(int price, int chests, List<Point> track)
        {
            if (chests > bestChests)
            {
                bestPrice = price;
                bestTrack=track.ToList();
                bestChests = chests;
            }
        }

        static List<Point> CovertResult(State state, Dictionary<(Point, Point), PathWithCost> trackEval)
        {
            var result = new List<Point>();
            var start = state.Position;
            foreach (var end in bestTrack)
            {
                foreach (var point in trackEval[(start,end)].Path.Skip(1))
                {
                    result.Add(point);
                }
                start = end;
            }
            return result;
        }
    }
}