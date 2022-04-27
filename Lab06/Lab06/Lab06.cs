using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    public class Lab06 : System.MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 i 2 - szukanie trasy w nieplynacej rzece
        /// </summary>
        /// <param name="w"> Odległość między brzegami rzeki</param>
        /// <param name="l"> Długość fragmentu rzeki </param>
        /// <param name="lilie"> Opis lilii na rzece </param>
        /// <param name="sila"> Siła żabki - maksymalny kwadrat długości jednego skoku </param>
        /// <param name="start"> Początkowa pozycja w metrach od lewej strony </param>
        /// <returns> (int total, (int x, int y)[] route) - total - suma sił koniecznych do wszystkich skoków, route -
        /// lista par opisujących skoki. Opis jednego skoku (x,y) to dystans w osi x i dystans w osi y, jaki skok pokonuje</returns>
        public (int total, (int, int)[] route) Lab06_FindRoute(int w, int l, int[,] lilie, int sila, int start)
        {
            //Console.WriteLine();
            //for (int i = 0; i < lilie.GetLength(0); i++)
            //{
            //    for (int j = 0; j < lilie.GetLength(1); j++)
            //    {
            //        Console.Write("{0} ", lilie[i, j]);
            //    }
            //    Console.WriteLine();
            //}
            int[,] tab = new int[lilie.GetLength(0) + 2, lilie.GetLength(1)];
            int h = tab.GetLength(0);
            int sz = tab.GetLength(1);
            for(int i = 0; i < sz; i++)
            {
                if (i == start)
                {
                    tab[0, i] = 1;
                }
                tab[h - 1, i] = 1;
            }
            for (int i = 0; i < lilie.GetLength(0); i++)
                for (int j = 0; j < lilie.GetLength(1); j++)
                    tab[i + 1, j] = lilie[i, j];
            var g = new DiGraph<double>(tab.Length);
            for(int i = 0; i < h; i++)
                for(int j = 0; j < sz; j++)
                    if(tab[i, j] == 1)
                        for(int k = 0; k < h; k++)
                            for(int p = 0; p < sz; p++)
                                if(tab[k, p] == 1 && k != 0 && (i != k || j != p))
                                {
                                    double s = Math.Pow(i - k, 2) + Math.Pow((j - p), 2);
                                    if (s <= sila) g.AddEdge(i * sz + j, k * sz + p, s);
                                }
            var pi = Paths.Dijkstra<double>(g, start);
            double dist = int.MaxValue;
            int end = 0;
            for (int i = 0; i < sz; i++)
                if (pi.Reachable(start, sz * (h - 1) + i) == true && pi.GetDistance(start, sz * (h - 1) + i) < dist)
                {
                    dist = pi.GetDistance(start, sz * (h - 1) + i);
                    end = sz * (h - 1) + 1;
                }
            if (dist < int.MaxValue)
            {
                var pa = pi.GetPath(start, end);
                var res = new (int, int)[pa.Length - 1];
                var temp = (0, 0);
                var prev = (0, 0);
                int k = 0;
                foreach (var item in pa)
                    if (item < sz)
                    {
                        prev = (0, item);
                        continue;
                    }
                    else
                    {
                        temp = ((((int)item - (int)item % sz / sz) / sz), (int)item % sz);
                        res[k] = (temp.Item1 - prev.Item1, temp.Item2 - prev.Item2);
                        prev = temp;
                        k++;
                    }
                return ((int)dist, res);
            }
            return (0, null);
        }

        /// <summary>
        /// Etap 3 i 4 - szukanie trasy w nieplynacej rzece
        /// </summary>
        /// <param name="w"> Odległość między brzegami rzeki</param>
        /// <param name="l"> Długość fragmentu rzeki </param>
        /// <param name="lilie"> Opis lilii na rzece </param>
        /// <param name="sila"> Siła żabki - maksymalny kwadrat długości jednego skoku </param>
        /// <param name="start"> Początkowa pozycja w metrach od lewej strony </param>
        /// <param name="max_skok"> Maksymalna ilość skoków </param>
        /// <param name="v"> Prędkość rzeki </param>
        /// <returns> (int total, (int x, int y)[] route) - total - suma sił koniecznych do wszystkich skoków, route -
        /// lista par opisujących skoki. Opis jednego skoku (x,y) to dystans w osi x i dystans w osi y, jaki skok pokonuje</returns>
        public (int total, (int, int)[] route) Lab06_FindRouteFlowing(int w, int l, int[,] lilie, int sila, int start, int max_skok, int v)
        {
            //Console.WriteLine();
            //for (int i = 0; i < lilie.GetLength(0); i++)
            //{
            //    for (int j = 0; j < lilie.GetLength(1); j++)
            //    {
            //        Console.Write("{0} ", lilie[i, j]);
            //    }
            //    Console.WriteLine();
            //}

            int[,,] tab = new int[lilie.GetLength(0) + 2, lilie.GetLength(1), max_skok + 1];
            int h = tab.GetLength(0);
            int sz = tab.GetLength(1);
            for(int j = 0; j < max_skok + 1; j++)
                for (int i = 0; i < sz; i++)
                {
                    if (i == start)
                    {
                        tab[0, i, j] = 1;
                    }
                    tab[h - 1, i, j] = 1;
                }
            for (int t = 0; t < max_skok + 1; t++)
                for (int i = 0; i < lilie.GetLength(0); i++)
                    for (int j = 0; j < lilie.GetLength(1); j++)
                        tab[i + 1, j, t] = lilie[i, ((j - t * v) + t * v * lilie.GetLength(1)) % lilie.GetLength(1)];

            var g = new DiGraph<double>(tab.Length);
            for(int t = 0; t < max_skok; t++)
                for(int i = 0; i < h; i++)
                    for(int j = 0; j < sz; j++)
                        if (tab[i, j, t] == 1)
                            for(int k = 0; k < h; k++)
                                for(int p = 0; p < sz; p++)
                                    if(tab[k, p, t + 1] == 1)
                                    {
                                        if (i == k && j == p)
                                        {
                                            if (i == h - 1)
                                                g.AddEdge(k * sz + p + (t + 1) * h * sz, i * sz + j + t * h * sz, 0.0000001);
                                            else
                                                g.AddEdge(i * sz + j + t * h * sz, k * sz + p + (t + 1) * h * sz, 0.0000001);
                                        }
                                        else
                                        {
                                            double s = Math.Pow(i - k, 2) + Math.Pow(j - p, 2);
                                            if(s <= sila)
                                                g.AddEdge(i * sz + j + t * h * sz, k * sz + p + (t + 1) * h * sz, s);
                                        }
                                    }


            //for (int i = 0; i < g.VertexCount; i++)
            //{
            //    Console.Write("{0} ", i);
            //    foreach (var d in g.OutNeighbors(i))
            //    {
            //        Console.Write("{0} ", d);
            //    }
            //    Console.WriteLine();
            //    if ((i + 1) % (sz * h) == 0 && i != 1)
            //        Console.WriteLine();
            //}
            //for (int t = 0; t < max_skok + 1; t++)
            //{
            //    Console.WriteLine();
            //    for (int i = 0; i < tab.GetLength(0); i++)
            //    {
            //        Console.WriteLine();
            //        for (int j = 0; j < tab.GetLength(1); j++)
            //        {
            //            Console.Write("{0} ", tab[i, j, t]);
            //        }
            //    }
            //}

            var pi = Paths.Dijkstra<double>(g, start);
            double dist = int.MaxValue;
            int end = 0;
            for (int i = 0; i < sz; i++)
                if (pi.Reachable(start, sz * (h - 1) + i) == true && pi.GetDistance(start, sz * (h - 1) + i) < dist)
                {
                    dist = pi.GetDistance(start, sz * (h - 1) + i);
                    end = sz * (h - 1) + 1;
                }
            if (dist < int.MaxValue)
            {
                int i = 0;
                var pa = pi.GetPath(start, end);
                while (Math.Floor((double)((int)pa.GetValue(pa.Length - 1 - i) % (h * sz)) / sz) == Math.Floor((double)((int)pa.GetValue(pa.Length - 2 - i) % (h * sz)) / sz))
                {
                    pa[pa.Length - 1 - i] = -1;
                    i++;
                }
                var res = new (int, int)[pa.Length - 1 - i];
                var temp = (0, 0);
                var prev = (0, 0);
                int k = 0;
                foreach (var item in pa)
                    if (item < sz)
                    {
                        prev = (0, item);
                        continue;
                    }
                    else if (item != -1)
                    {
                        temp = ((((int)item % (h * sz) - ((int)item % (h * sz)) % sz / sz) / sz), ((int)item % (h * sz)) % sz);
                        res[k] = (temp.Item1 - prev.Item1, temp.Item2 - prev.Item2);
                        //Console.WriteLine(res[k]);
                        prev = temp;
                        k++;
                    }
                return ((int)dist, res);
            }
            return (0, null);
        }
    }
}
