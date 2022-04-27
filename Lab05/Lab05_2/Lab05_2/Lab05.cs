using System.Linq;

namespace ASD
{
    using ASD.Graphs;
    using System;
    using System.Collections.Generic;

    public class Lab06 : System.MarshalByRefObject
    {
        /// <summary>
        /// Część I: wyznaczenie najszerszej ścieżki grafu.
        /// </summary>
        /// <param name="G">informacja o przejazdach między punktami; wagi krawędzi są całkowite i nieujemne i oznaczają szerokość trasy między dwoma punktami</param>
        /// <param name="start">informacja o wierzchołku początkowym</param>
        /// <param name="end">informacja o wierzchołku końcowym</param>
        /// <returns>najszersza ścieżka między wierzchołkiem początkowym a końcowym lub pusta lista, jeśli taka ścieżka nie istnieje</returns>
        public List<int> WidePath(DiGraph<int> G, int start, int end)
        {
            int[] width = new int[G.VertexCount];
            int[] prev = new int[G.VertexCount];
            SafePriorityQueue<int, int> queue = new SafePriorityQueue<int, int>(new Comparison());

            width[start] = int.MaxValue;

            for (int i = 0; i < width.Length; i++)
            {
                queue.Insert(i, width[i]);
                prev[i] = -1;
            }

            while (queue.Count != 0)
            {
                var u = queue.Extract();
                foreach (var v in G.OutNeighbors(u))
                {
                    if (width[v] < Math.Min(width[u], G.GetEdgeWeight(u, v)))
                    {
                        width[v] = Math.Min(width[u], G.GetEdgeWeight(u, v));
                        queue.UpdatePriority(v, width[v]);
                        prev[v] = u;
                    }
                }
            }

            if (prev[end] == -1)
                return new List<int>();

            int current = end;
            List<int> list = new List<int>();
            while (current != start)
            {
                list.Add(current);
                current = prev[current];
            }
            list.Add(start);
            list.Reverse();

            return list;

        }

        /// <summary>
        /// Część II: wyznaczenie najszerszej epidemicznej ścieżki.
        /// </summary>
        /// <param name="G">informacja o przejazdach między punktami; wagi krawędzi są całkowite i nieujemne i oznaczają szerokość trasy między dwoma punktami</param>
        /// <param name="start">informacja o wierzchołku początkowym</param>
        /// <param name="end">informacja o wierzchołku końcowym</param>
        /// <param name="weights">wagi wierzchołków odpowiadające czasom oczekiwania na bramkach wjzadowych do poszczególnych miejsc. Wagi są nieujemne i całkowite</param>
        /// <param name="maxWeight">maksymalna waga krawędzi w grafie</param>
        /// <returns>ścieżka dla której różnica między jej najwęższą krawędzią a sumą wag wierzchołków przez które przechodzi jest maksymalna.</returns>
        public List<int> WeightedWidePath(DiGraph<int> G, int start, int end, int[] weights, int maxWeight)
        {
            int[] path = new int[0];
            int curMax = int.MinValue;

            List<int> widths = new List<int>();
            foreach (var e in G.DFS().SearchAll())
                widths.Add(e.weight);
            widths.Sort();
            widths.Reverse();

            DiGraph<int> subgraph = new DiGraph<int>(G.VertexCount);

            foreach (var curWidth in widths.Distinct())
            {
                foreach (var e in G.BFS().SearchAll())
                {
                    if (e.weight == curWidth)
                        subgraph.AddEdge(e.From, e.To, weights[e.To]);
                }
                var pathInfo = Paths.Dijkstra(subgraph, start);
                if (pathInfo.Reachable(start, end))
                {
                    if (curWidth - pathInfo.GetDistance(start, end) > curMax)
                    {
                        curMax = curWidth - pathInfo.GetDistance(start, end);
                        path = pathInfo.GetPath(start, end);
                    }
                }
            }
            return path.ToList();
        }
    }

    public class Comparison : Comparer<int>
    {
        public override int Compare(int x, int y)
        {
            return y - x;
        }
    }
}