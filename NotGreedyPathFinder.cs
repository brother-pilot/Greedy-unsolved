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
        //public Point Previous { get; set; }
        public List<Point> ChestList { get; set; }
        public List<Point> BestTrack { get; set; }
    }
    public class NotGreedyPathFinder : IPathFinder
    {
        static int bestPrice = int.MaxValue;
        static int bestChests = int.MinValue;
        static List<Point> bestTrack = new List<Point>();
        public List<Point> FindPathToCompleteGoal(State state)
        {
            if (state.Chests.Count == 0 || state.Energy == 0 || state.Chests.Count < state.Goal)
                return new List<Point>();
            var trackEval = new Dictionary<(Point, Point), PathWithCost>();
            var pointEval = new Dictionary<(Point, Point), DataNotGreedy>();
            MakePath(state, trackEval, pointEval, new List<Point>(), state.Position, state.Chests.ToList());
            return CovertResult(state, pointEval, trackEval);
        }

        void MakePath(State state, Dictionary<(Point, Point), PathWithCost> trackEval,
            Dictionary<(Point, Point), DataNotGreedy> pointEval, List<Point> track2,
            Point start,
            List<Point> chestList, int price = 0, int chests = 0)
        {
            var paths = new DijkstraPathFinder();
            var chestLocalList = chestList.ToList();
            List<Point> track = track2.ToList(); 
            foreach (var chest in chestList)
            {
                if (!trackEval.ContainsKey((start, chest)))
                {
                    trackEval[(start, chest)] =
                    paths.GetPathsByDijkstra(state, start, new List<Point> { chest }).FirstOrDefault();
                }
                chests++;
                price += trackEval[(start, chest)].Cost;
                track.Add(chest);
                chestLocalList.Remove(chest);
                pointEval[(start, chest)] = new DataNotGreedy
                {
                    Price = price,
                    Chests = chests,
                    ChestList = chestLocalList.ToList(),
                    
                    BestTrack= track.ToList()
            };
                if (price > state.Energy || chests > state.Chests.Count + 1)
                {
                    price -= trackEval[(start, chest)].Cost;
                    chests--;
                    track.Remove(chest);
                    BestChase(price, chests, track);
                    continue;
                    //break;
                }
                BestChase(price, chests, track);
                MakePath(state, trackEval, pointEval, track,chest, chestLocalList, price, chests);
                chestLocalList.Add(chest);
                price -= trackEval[(start, chest)].Cost;
                chests--;
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

        static List<Point> CovertResult(State state,Dictionary<(Point, Point), DataNotGreedy> pointEval,
            Dictionary<(Point, Point), PathWithCost> trackEval)
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