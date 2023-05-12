

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
            var graph = MakeGraf(state, start);
            List<Node> notVisited = graph.Nodes.ToList();

            var track = new Dictionary<Node, DijkstraData>();//����� �������� ���������� �� ������� ��� ������ �������
            track[graph[start]] = new DijkstraData { Price = 0, Previous = null };//������� ���� � ������� �������

            while (true)
            {
                Node toOpen = null;//����������� �������
                var bestPrice = double.PositiveInfinity;
                foreach (var e in notVisited)//��� ������ ������� � ������������
                {
                    if (track.ContainsKey(e) && track[e].Price < bestPrice)
                    {
                        bestPrice = track[e].Price;
                        toOpen = e;
                    }
                }
                //if (notVisited.Count == 0) yield break;
                if (toOpen == null) yield break;
                if (targets2.Contains(toOpen.NodePoint))
                {
                    yield return ConvertResult(track, toOpen);//����� �� ������������ ����� �����
                    targets2 = targets2.Where(x => x != toOpen.NodePoint);
                }

                foreach (var e in toOpen.IncidentNodes)//��������� �� ���� ������ � ������� ��� ������� �������� ���������
                {
                    var currentPrice = track[toOpen].Price + state.CellCost[e.NodePoint.X, e.NodePoint.Y];//���� ��� �����
                    if (!track.ContainsKey(e) || track[e].Price > currentPrice)//���� ��� ������� �� ������� ������ ������� ������ ��� ��������� ������ ���� ��� ��� ����������� �������
                    {
                        
                        track[e] = new DijkstraData { Previous = toOpen, Price = currentPrice };//������� ����� ���� � ������� �� ������� ������
                        if (targets2.Contains(e.NodePoint))
                        {
                            yield return ConvertResult(track, e);//����� �� ������������ ����� �����
                            targets2=targets2.Where(x => x != e.NodePoint);
                        }
                    }
                }

                notVisited.Remove(toOpen);
            }

            //var result = new List<Point>();
            //while (end != null)
            //{
            //    result.Add(end);
            //    end = track[end].Previous;
            //}
            //result.Reverse();
            //return result;

        }
        PathWithCost ConvertResult(Dictionary<Node, DijkstraData> track, Node node)
        {
            var result = new List<Point>();
            var result2 = new Point[track.Count];
            var end = node;
            while (node != null)
            {
                result.Add(node.NodePoint);
                node = track[node].Previous;
            }
            //result2=result.;
            result.Reverse();
            return new PathWithCost(track[end].Price, result.ToArray());
        }
        Graph MakeGraf(State state, Point start)
        {
            var graph = new Graph();
            var queue = new Queue<Point>();
            //var visited = new HashSet<Point>();//��������� �����������. �� ����� ����� �� ������ ��������
            //visited.Add(start);
            queue.Enqueue(start);
            graph.AddNode(new Node(start));
            while (queue.Count != 0)//� ������� ���������� �������� ����� ������� �� ����� ������������ �� ��������� ����
            {
                var point = queue.Dequeue();
                var v1 = graph[point];
                foreach (var e in PossiblePoint(point, state))
                {
                    var iff = graph.Nodes
                        .Where(x => x.NodePoint == e);
                    if (iff.Select(p => p.NodePoint).Contains(e)
                            && iff.FirstOrDefault().IncidentNodes.Count() > 4) continue;
                    //if (graph[e].NodePoint.IsEmpty) continue;
                    //visited.Add(e);
                    var v2 = new Node(e);
                    if (!iff.Select(p => p.NodePoint).Contains(e))
                    {
                        graph.AddNode(v2);
                        queue.Enqueue(e);
                    }
                    if (!v1.IncidentNodes
                        .Select(p => p.NodePoint)
                        .Contains(e))
                        v1.Connect(v2);
                }
            }
            return graph;
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
        public Node Previous { get; set; }//��� ������ � ��� �������. ����� ���� �������� ������ �.�. ����� ��������� �����
        public int Price { get; set; }//���� �������
    }

    public class Node
    {
        private readonly List<Node> incidentNodes = new List<Node>();//��������� �� ���������
        public readonly Point NodePoint; //readonly ���� ����� ��� ���� ����� � �������� ������ ��������� ���� �� ����������. ���� ������� ������� 1 �� ��� � ������ �������� 1��

        public Node(Point point)//readonly ���� �������� ����� �����������. ������ ����� ����
        {
            NodePoint = point;
        }

        public IEnumerable<Node> IncidentNodes//� ��� ������� ��������� �����. ��������� ���������� ����� ���� �������� ��������� ��� ������������� ����
        {
            get
            {
                foreach (var node in incidentNodes) //��� �������� ����������� ��� node � incidentNode �� �� �������� �� ��������
                    yield return node;
            }
        }

        public void Connect(Node node) //������������� ����� ����� ������
        {
            incidentNodes.Add(node);
            node.incidentNodes.Add(this);//��������� �� ��� ������ �� �������� ��� ������ ����� connect. �.�. ����� �� ������������� ����������������� ����� ����� ���������
        }
    }

    public class Graph
    {
        private List<Node> nodes = new List<Node>();
        //public Graph(int nodesCount)
        //{
        //    nodes = Enumerable.Range(0, nodesCount).Select(z => new Node(z)).ToArray();
        //}
        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public int Length { get { return nodes.Count; } }

        public Node this[Point point] { get { return nodes.Where(z => z.NodePoint == point).FirstOrDefault(); } } //�����c����

        public IEnumerable<Node> Nodes
        {
            get
            {
                foreach (var node in nodes) yield return node;
            }
        }
    }


}