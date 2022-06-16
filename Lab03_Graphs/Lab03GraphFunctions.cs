﻿using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;

namespace ASD
{

    public class Lab03GraphFunctions : System.MarshalByRefObject
    {

        // Część 1
        // Wyznaczanie odwrotności grafu
        //   0.5 pkt
        // Odwrotność grafu to graf skierowany o wszystkich krawędziach przeciwnie skierowanych niż w grafie pierwotnym
        // Parametry:
        //   g - graf wejściowy
        // Wynik:
        //   odwrotność grafu
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Graf wynikowy musi być w takiej samej reprezentacji jak wejściowy
        public DiGraph Lab03Reverse(DiGraph g)
        {
            DiGraph gNew = new DiGraph(g.VertexCount, g.Representation);

            foreach (var e in g.DFS().SearchAll())
            {
                gNew.AddEdge(e.To, e.From);
            }
            return gNew;
        }

        // Część 2
        // Badanie czy graf jest dwudzielny
        //   0.5 pkt
        // Graf dwudzielny to graf nieskierowany, którego wierzchołki można podzielić na dwa rozłączne zbiory
        // takie, że dla każdej krawędzi jej końce należą do róźnych zbiorów
        // Parametry:
        //   g - badany graf
        //   vert - tablica opisująca podział zbioru wierzchołków na podzbiory w następujący sposób
        //          vert[i] == 1 oznacza, że wierzchołek i należy do pierwszego podzbioru
        //          vert[i] == 2 oznacza, że wierzchołek i należy do drugiego podzbioru
        // Wynik:
        //   true jeśli graf jest dwudzielny, false jeśli graf nie jest dwudzielny (w tym przypadku parametr vert ma mieć wartość null)
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Podział wierzchołków może nie być jednoznaczny - znaleźć dowolny
        //   3) Pamiętać, że każdy z wierzchołków musi być przyporządkowany do któregoś ze zbiorów
        //   4) Metoda ma mieć taki sam rząd złożoności jak zwykłe przeszukiwanie (za większą będą kary!)
        public bool Lab03IsBipartite(Graph g, out int[] vert)
        {
            vert = new int[g.VertexCount];

            vert[0] = 1; // 1 - pierwszy kolor, 2 - drugi kolor
            //visited[0] = true;

            foreach (var e in g.DFS().SearchAll())
            {
                if (vert[e.From] == 0)
                {
                    switch (vert[e.To])
                    {
                        case 0:
                            vert[e.From] = 1;
                            vert[e.To] = 2;
                            break;

                            // wydaje mi sie ze tego nie trzeba, bo vert[e.From]==0 wystepuje tylko na poczatku skladowej
                            // a na poczatku skladowej e.To bedzie zawsze 0

                            //case 1:
                            //    vert[e.From] = 2;
                            //    break;
                            //case 2:
                            //    vert[e.From] = 1;
                            //    break;
                    }
                }

                else if (vert[e.From] == 1)
                {
                    switch (vert[e.To])
                    {
                        case 1:
                            vert = null;
                            return false;
                        case 0:
                            vert[e.To] = 2;
                            break;
                    }
                }

                else if (vert[e.From] == 2)
                {
                    switch (vert[e.To])
                    {
                        case 2:
                            vert = null;
                            return false;
                        case 0:
                            vert[e.To] = 1;
                            break;
                    }
                }
            }

            for (int i = 0; i < g.VertexCount; i++)
            {
                if (vert[i] == 0)
                {
                    vert[i] = 1;
                }
            }

            return true;
        }

        // Część 3
        // Wyznaczanie minimalnego drzewa rozpinającego algorytmem Kruskala
        //   1 pkt
        // Schemat algorytmu Kruskala
        //   1) wrzucić wszystkie krawędzie do "wspólnego worka"
        //   2) wyciągać z "worka" krawędzie w kolejności wzrastających wag
        //      - jeśli krawędź można dodać do drzewa to dodawać, jeśli nie można to ignorować
        //      - punkt 2 powtarzać aż do skonstruowania drzewa (lub wyczerpania krawędzi)
        // Parametry:
        //   g - graf wejściowy
        //   mstw - waga skonstruowanego drzewa (lasu)
        // Wynik:
        //   skonstruowane minimalne drzewo rozpinające (albo las)
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Wykorzystać klasę UnionFind z biblioteki Graph
        //   3) Jeśli graf g jest niespójny to metoda wyznacza las rozpinający
        //   4) Graf wynikowy (drzewo) musi być w takiej samej reprezentacji jak wejściowy
        public Graph<int> Lab03Kruskal(Graph<int> g, out int mstw)
        {
            PriorityQueue<int, Edge<int>> queue = new PriorityQueue<int, Edge<int>>();

            foreach (var e in g.DFS().SearchAll())
            {
                queue.Insert(e, e.weight);
            }

            UnionFind unionFind = new UnionFind(g.VertexCount);
            Graph<int> tree = new Graph<int>(g.VertexCount);
            mstw = 0;

            while (queue.Count > 0)
            {
                Edge<int> e = queue.Extract();

                if (unionFind.Find(e.From) != unionFind.Find(e.To))
                {
                    unionFind.Union(e.From, e.To);
                    tree.AddEdge(e.From, e.To);
                    mstw += e.weight;
                }
            }

            return tree;
        }

        // Część 4
        // Badanie czy graf nieskierowany jest acykliczny
        //   0.5 pkt
        // Parametry:
        //   g - badany graf
        // Wynik:
        //   true jeśli graf jest acykliczny, false jeśli graf nie jest acykliczny
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Najpierw pomysleć jaki, prosty do sprawdzenia, warunek spełnia acykliczny graf nieskierowany
        //      Zakodowanie tego sprawdzenia nie powinno zająć więcej niż kilka linii!
        //      Zadanie jest bardzo łatwe (jeśli wydaje się trudne - poszukać prostszego sposobu, a nie walczyć z trudnym!)
        public bool Lab03IsUndirectedAcyclic(Graph g)
        {
            bool[] visited = new bool[g.VertexCount];
            int components = 0;
            int edges = 0;
            int vertices = g.VertexCount;


            foreach (var e in g.DFS().SearchAll())
            {
                edges++;

                if (visited[e.From] == false)
                {
                    components++;
                }

                visited[e.From] = true;
                visited[e.To] = true;
            }

            edges /= 2;

            // dla wierzchołków izolowanych
            for (int i = 0; i < vertices; i++)
            {
                if (visited[i] == false)
                {
                    visited[i] = true;
                    components++;
                }
            }

            if (vertices == edges + components) return true;
            return false;
        }

    }

}
