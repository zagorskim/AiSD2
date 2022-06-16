using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;
using System.Linq;
namespace ASD
{
    public class Lab15 : System.MarshalByRefObject
    {
        /// <summary>
        /// Etap 1: Rozmiar najliczniejszej krainy w zadanym grafie (2.5p)
        /// 
        /// Przez krainę rozumiemy maksymalny zbiór wierzchołków, z których
        /// każde dwa należą do jakiegoś cyklu (równoważnie: najliczniejszy
        /// zbiór wierzchołków G indukujący podgraf 2-spójny wierzchołkowo).
        /// 
        /// Uwaga: Z powyższej definicji wynika, że zbiór pusty jest krainą, 
        /// a zbiór jednoelementowy nie.
        /// </summary>
        /// <param name="G">Graf prosty</param>
        /// <returns>Rozmiar największej bańki</returns>
        /// 
        public int MaxProvinceSize(Graph G)
        {
            int t = 0;
            int[] disc = new int[G.VertexCount];
            int[] low = new int[G.VertexCount];
            int[] parent = new int[G.VertexCount];
            Stack<Edge> stack = new Stack<Edge>();
            List<List<Edge>> list = new List<List<Edge>>();
            for(int i = 0; i < G.VertexCount; i++)
            {
                disc[i] = -1;
                low[i] = -1;
                parent[i] = -1;
            }
            for(int i = 0; i < G.VertexCount; i++)
            {
                if (disc[i] == -1)
                    Rec(G, list, i, t, stack, disc, low, parent);
                if (stack.Count() > 0)
                {
                    var temp = new List<Edge>();
                    while (stack.Count > 0)
                    {
                        temp.Add(stack.Pop());
                    }
                    list.Add(temp);
                }
            }
            int max = 0;
            foreach (var l in list)
            {
                var temp = l.Select(x => x.To).Distinct().ToList();
                if (max < temp.Count())
                    max = temp.Count();
            }
            return max > 1 ? max : 0;
        }

        /// <summary>
        /// Etap 2: Wierzchołek znajdujący się w największej liczbie krain (2.5p)
        /// 
        /// Funcja zwraca wierzchołek znajdujący się w największej liczbie krain.
        /// 
        /// W przypadku remisu należy zwrócić wierzchołek o mniejszym numerze.
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        public int VertexInMostProvinces(Graph G)
        {
            int t = 0;
            int[] disc = new int[G.VertexCount];
            int[] low = new int[G.VertexCount];
            int[] parent = new int[G.VertexCount];
            int[] areas = new int[G.VertexCount];
            Stack<Edge> stack = new Stack<Edge>();
            List<List<Edge>> list = new List<List<Edge>>();
            for (int i = 0; i < G.VertexCount; i++)
            {
                disc[i] = -1;
                low[i] = -1;
                parent[i] = -1;
            }
            for (int i = 0; i < G.VertexCount; i++)
            {
                if (disc[i] == -1)
                    Rec(G, list, i, t, stack, disc, low, parent);
                if (stack.Count() > 0)
                {
                    var temp = new List<Edge>();
                    while (stack.Count > 0)
                    {
                        temp.Add(stack.Pop());
                    }
                    list.Add(temp);
                }
            }
            foreach(var l in list)
            {
                if (l.Count > 1)
                {
                    var temp = l.Select(x => x.To).Distinct().ToList();
                    for (int i = temp.Count - 1; i >= 0; i--)
                        areas[temp[i]]++;
                }
            }
            int indexOfMax = 0;
            for (int i = 0; i < areas.Length; i++)
                if (areas[i] > areas[indexOfMax])
                    indexOfMax = i;
            return indexOfMax;
        }
        void Rec(Graph G, List<List<Edge>> list, int vertex, int t, Stack<Edge> stack, int[] disc, int[] low, int[] parent)
        {
            t++;
            disc[vertex] = t;
            low[vertex] = t;
            int count = 0;
            foreach(var i in G.OutNeighbors(vertex))
            {
                if (disc[i] == -1)
                {
                    count++;
                    parent[i] = vertex;
                    stack.Push(new Edge(vertex, i));
                    Rec(G, list, i, t, stack, disc, low, parent);
                    if (low[vertex] > low[i])
                        low[vertex] = low[i];
                    if ((disc[vertex] == 1 && count > 1) || (disc[vertex] > 1 && low[i] >= disc[vertex]))
                    {
                        var temp = new List<Edge>();
                        while(stack.Peek().From != vertex || stack.Peek().To != i)
                        {
                            temp.Add(stack.Pop());
                        }
                        temp.Add(stack.Pop());
                        list.Add(temp);
                    }
                }
                else if (i != parent[vertex] && disc[i] < disc[vertex])
                {
                    if (low[vertex] > disc[i])
                        low[vertex] = disc[i];

                    stack.Push(new Edge(vertex, i));
                }
            }
        }
    }

   
}

//ALGORITHM IDEAS: https://www.geeksforgeeks.org/biconnected-components/
//                 https://www.geeksforgeeks.org/articulation-points-or-cut-vertices-in-a-graph/