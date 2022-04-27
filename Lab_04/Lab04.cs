using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;

namespace ASD
{

    public class Lab04 : System.MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - szukanie trasy z miasta start_v do miasta end_v, startując w dniu day
        /// </summary>
        /// <param name="g">Ważony graf skierowany będący mapą</param>
        /// <param name="start_v">Indeks wierzchołka odpowiadającego miastu startowemu</param>
        /// <param name="end_v">Indeks wierzchołka odpowiadającego miastu docelowemu</param>
        /// <param name="day">Dzień startu (w tym dniu należy wyruszyć z miasta startowego)</param>
        /// <param name="days_number">Liczba dni uwzględnionych w rozkładzie (tzn. wagi krawędzi są z przedziału [0, days_number-1])</param>
        /// <returns>(result, route) - result ma wartość true gdy podróż jest możliwa, wpp. false, 
        /// route to tablica z indeksami kolejno odwiedzanych miast (pierwszy indeks to indeks miasta startowego, ostatni to indeks miasta docelowego),
        /// jeżeli result == false to route ustawiamy na null</returns>
        /// 
        //public (bool result, int[] route) Lab04_FindRoute(DiGraph<int> g, int start_v, int end_v, int day, int days_number)
        //{
        //    int curr_city = start_v, curr_day = day;
        //    bool res1 = false;
        //    int[] res2 = null;

        //    if(curr_city == end_v)
        //    {
        //        return (true, null);
        //    }

        //    foreach(var e in g.OutEdges(curr_city))
        //    {
        //        if(e.weight == curr_day)
        //        {
        //            (res1, res2) = Lab04_FindRoute(g, e.To, end_v, (curr_day + 1) % days_number, days_number);
        //            if (res1 == true) break;
        //        }
        //    }

        //    return (res1, null);
        //}

        //public (bool result, int[] route) Lab04_FindRoute(DiGraph<int> g, int start_v, int end_v, int day, int days_number)
        //{
        //    int curr_city = start_v, curr_day = day, n = 0, curr_n = 0, neighs = 0, flag;
        //    Stack<(int, int, int)> s = new Stack<(int, int, int)>();

        //    while (curr_city != end_v)
        //    {
        //        n = 0;
        //        neighs = 0;
        //        flag = 0;
        //        foreach (var v in g.OutNeighbors(curr_city)) neighs++;
        //        foreach (var v in g.OutNeighbors(curr_city))
        //        {
        //            n++;
        //            if (g.GetEdgeWeight(curr_city, v) == curr_day && n > curr_n)
        //            {
        //                if (s.Count != 0) 
        //                    if(s.Contains((curr_city, curr_day, n)))
        //                    {
        //                        while (s.Peek() != (curr_city, curr_day, n)) s.Pop();
        //                        (curr_city, curr_day, curr_n) = s.Pop();
        //                        curr_n++;
        //                        continue;
        //                    }
        //                s.Push((curr_city, curr_day, n));
        //                curr_city = v;
        //                curr_day = (curr_day + 1) % days_number;
        //                curr_n = 0;
        //                flag = 1;
        //                break;
        //            }
        //        }
        //        if (s.Count == 0) return (false, null);
        //        if (flag == 0) (curr_city, curr_day, curr_n) = s.Pop();
        //        Console.WriteLine(curr_city);
        //    }
        //    s.Push((curr_city, curr_day, n));
        //    return (true, null);
        //}

        //public (bool result, int[] route) Lab04_FindRoute(DiGraph<int> g, int start_v, int end_v, int day, int days_number)
        //{
        //    DiGraph temp = new DiGraph(days_number * g.VertexCount);
        //    Stack<Edge> q = new Stack<Edge>();
        //    int flag = 0;

        //    foreach(var e in g.DFS().SearchAll())
        //    {
        //        temp.AddEdge(days_number * e.From + e.weight, days_number * e.To + (e.weight + 1) % days_number);
        //    }

        //    foreach (var e in temp.DFS().SearchFrom(days_number * start_v + day))
        //    {
        //        q.Push(e);
        //        for (int i = 0; i < days_number; i++)
        //            if (e.To == days_number * end_v + i)
        //            {
        //                flag = 1;
        //                break;
        //            }
        //    }

        //    if (flag == 1)
        //    {
        //        var tab = new int[q.Count + 1];
        //        int j = 0;
        //        foreach (var i in q)
        //        {
        //            tab[j] = (i.To - i.To % days_number) / days_number;
        //            j++;
        //        }
        //        for (int i = 0; i < tab.Length - 2; i++) q.Pop();
        //        tab[tab.Length - 1] = (q.Peek().From - q.Peek().From % days_number) / days_number;
        //        Array.Reverse(tab);

        //        int count = 0;
        //        int n = 1;
        //        for(int i = 0; i < tab.Length - 2; i = n)
        //        {
        //            n = i + 1;
        //            while (n < tab.Length - 1 && g.HasEdge(tab[i], tab[n]) == false && g.HasEdge(tab[n], tab[i]) == false)
        //            {
        //                if (g.HasEdge(tab[i], tab[n]) == false)
        //                {
        //                    tab[n] = -1;
        //                    n++;
        //                    count++;
        //                }
        //            }
        //        }
        //        var res = new int[tab.Length - count];
        //        int k = 0;
        //        for(int i = 0; i < tab.Length; i++)
        //            if(tab[i] != -1)
        //            {
        //                res[k] = tab[i];
        //                k++;
        //            }
        //        for (int i = 0; i < res.Length - 1; i++)
        //            if(g.HasEdge(res[i + 1], res[i]))
        //            {
        //                int t = res[i];
        //                res[i] = res[i + 1];
        //                res[i + 1] = t;
        //            }
        //        return (true, res);
        //    }
        //    return (false, null);
        //}

        public (bool result, int[] route) Lab04_FindRoute(DiGraph<int> g, int start_v, int end_v, int day, int days_number)
        {
            DiGraph temp = new DiGraph(days_number * g.VertexCount);
            Stack<Edge> q = new Stack<Edge>();
            int flag = 0;
            foreach (var e in g.DFS().SearchAll())
            {
                temp.AddEdge(days_number * e.From + e.weight, days_number * e.To + (e.weight + 1) % days_number);
            }

            foreach (var e in temp.DFS().SearchFrom(days_number * start_v + day))
            {
                if (flag == 1) break;
                if(q.Count > 0)
                    while(q.Peek().To != e.From)
                    {
                        q.Pop();
                        if (q.Count <= 0) break;
                    }

                q.Push(e);
                for (int i = 0; i < days_number; i++)
                    if (e.To == days_number * end_v + i)
                    {
                        flag = 1;
                        break;
                    }
            }

            if (flag == 1)
            {
                var tab = new int[q.Count + 1];
                Edge last = new Edge();
                last = q.Peek();
                int i = 0;
                foreach(var e in q)
                {
                    tab[i] = (e.To - e.To % days_number) / days_number;
                    i++;
                }
                for (int j = 0; j < tab.Length - 1; j++) last = q.Pop();
                tab[tab.Length - 1] = (last.From - last.From % days_number) / days_number;
                Array.Reverse(tab);
                return (true, tab);
            }
            return (false, null);
        }
        /// <summary>
        /// Etap 2 - szukanie trasy z jednego z miast z tablicy start_v do jednego z miast z tablicy end_v (startować można w dowolnym dniu)
        /// </summary>
        /// <param name="g">Ważony graf skierowany będący mapą</param>
        /// <param name="start_v">Tablica z indeksami wierzchołków startowych (trasę trzeba zacząć w jednym z nich)</param>
        /// <param name="end_v">Tablica z indeksami wierzchołków docelowych (trasę trzeba zakończyć w jednym z nich)</param>
        /// <param name="days_number">Liczba dni uwzględnionych w rozkładzie (tzn. wagi krawędzi są z przedziału [0, days_number-1])</param>
        /// <returns>(result, route) - result ma wartość true gdy podróż jest możliwa, wpp. false, 
        /// route to tablica z indeksami kolejno odwiedzanych miast (pierwszy indeks to indeks miasta startowego, ostatni to indeks miasta docelowego),
        /// jeżeli result == false to route ustawiamy na null</returns>
        public (bool result, int[] route) Lab04_FindRouteSets(DiGraph<int> g, int[] start_v, int[] end_v, int days_number)
        {
            // TODO
            return (false, null);
        }
    }
}
