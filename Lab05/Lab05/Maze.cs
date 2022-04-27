using ASD.Graphs;
using System;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Maze : MarshalByRefObject
    {

        /// <summary>
        /// Wersje zadania I oraz II
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt bez dynamitów lub z dowolną ich liczbą
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="withDynamite">informacja, czy dostępne są dynamity 
        /// Wersja I zadania -> withDynamites = false, Wersja II zadania -> withDynamites = true</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany (dotyczy tylko wersji II)</param> 
        public int FindShortestPath(char[,] maze, bool withDynamite, out string path, int t = 0)
        {
            //Console.WriteLine();
            //for (int i = 0; i < maze.GetLength(0); i++)
            //{
            //    for (int j = 0; j < maze.GetLength(1); j++)
            //        Console.Write("{0} ", maze[i, j]);
            //    Console.WriteLine();
            //}
            var g = new DiGraph<int>(maze.Length);
            var start = (0, 0);
            var end = (0, 0);
            int start_v = 0;
            int end_v = 0;
            for (int i = 0; i < maze.GetLength(0); i++)
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (maze[i, j] == 'S') start = (i, j);
                    if (maze[i, j] == 'E') end = (i, j);


                    if (withDynamite == true)
                    {
                        if (j != maze.GetLength(1) - 1)
                        {
                            g.AddEdge(maze.GetLength(1) * i + j, maze.GetLength(1) * i + j + 1);
                            if (maze[i, j + 1] == 'X') g.SetEdgeWeight(maze.GetLength(1) * i + j, maze.GetLength(1) * i + j + 1, t);
                            else g.SetEdgeWeight(maze.GetLength(1) * i + j, maze.GetLength(1) * i + j + 1, 1);

                            g.AddEdge(maze.GetLength(1) * i + j + 1, maze.GetLength(1) * i + j);
                            if (maze[i, j] == 'X') g.SetEdgeWeight(maze.GetLength(1) * i + j + 1, maze.GetLength(1) * i + j, t);
                            else g.SetEdgeWeight(maze.GetLength(1) * i + j + 1, maze.GetLength(1) * i + j, 1);
                        }
                        if (i != maze.GetLength(0) - 1)
                        {
                            g.AddEdge(maze.GetLength(1) * i + j, maze.GetLength(1) * (i + 1) + j);
                            if (maze[i + 1, j] == 'X') g.SetEdgeWeight(maze.GetLength(1) * i + j, maze.GetLength(1) * (i + 1) + j, t);
                            else g.SetEdgeWeight(maze.GetLength(1) * i + j, maze.GetLength(1) * (i + 1) + j, 1);

                            g.AddEdge(maze.GetLength(1) * (i + 1) + j, maze.GetLength(1) * i + j);
                            if (maze[i, j] == 'X') g.SetEdgeWeight(maze.GetLength(1) * (i + 1) + j, maze.GetLength(1) * i + j, t);
                            else g.SetEdgeWeight(maze.GetLength(1) * (i + 1) + j, maze.GetLength(1) * i + j, 1);
                        }
                    }
                    else
                    {
                        if (j != maze.GetLength(1) - 1)
                        {
                            if (maze[i, j + 1] != 'X')
                            {
                                g.AddEdge(maze.GetLength(1) * i + j, maze.GetLength(1) * i + j + 1);
                                g.SetEdgeWeight(maze.GetLength(1) * i + j, maze.GetLength(1) * i + j + 1, 1);
                            }
                            if (maze[i, j] != 'X')
                            {
                                g.AddEdge(maze.GetLength(1) * i + j + 1, maze.GetLength(1) * i + j);
                                g.SetEdgeWeight(maze.GetLength(1) * i + j + 1, maze.GetLength(1) * i + j, 1);
                            }
                        }
                        if (i != maze.GetLength(0) - 1)
                        {
                            if (maze[i + 1, j] != 'X')
                            {
                                g.AddEdge(maze.GetLength(1) * i + j, maze.GetLength(1) * (i + 1) + j);
                                g.SetEdgeWeight(maze.GetLength(1) * i + j, maze.GetLength(1) * (i + 1) + j, 1);
                            }

                            if (maze[i, j] != 'X')
                            {
                                g.AddEdge(maze.GetLength(1) * (i + 1) + j, maze.GetLength(1) * i + j);
                                g.SetEdgeWeight(maze.GetLength(1) * (i + 1) + j, maze.GetLength(1) * i + j, 1);
                            }
                        }
                    }
                }
            start_v = start.Item1 * maze.GetLength(1) + start.Item2;
            end_v = end.Item1 * maze.GetLength(1) + end.Item2;
            var pi = Paths.Dijkstra<int>(g, start_v);
            StringBuilder sb = new StringBuilder();
            if (pi.Reachable(start_v, end_v) == true)
            {
                var p = pi.GetPath(start_v, end_v);
                for(int i = 0; i < p.Length - 1; i++)
                {
                    if (p[i + 1] - p[i] == 1) sb.Append('E');
                    else if (p[i + 1] - p[i] == -1) sb.Append('W');
                    else if (p[i + 1] - p[i] > 1) sb.Append('S');
                    else if (p[i + 1] - p[i] < -1) sb.Append('N');
                }
                path = sb.ToString();
                return pi.GetDistance(start_v, end_v);
            }
            else
            {
                path = "";
                return -1;
            }
        }

        /// <summary>
        /// Wersja III i IV zadania
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt z użyciem co najwyżej k lasek dynamitu
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="k">liczba dostępnych lasek dynamitu, dla wersji III k=1</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany</param>
        public int FindShortestPathWithKDynamites(char[,] maze, int k, out string path, int t)
        {
            char[,] temp = maze;
            int dist = FindShortestPath(maze, true, out path, t);
            var g = new DiGraph<int>(maze.Length * (k + 1));
            int start_v = 0;
            int[] end_v = new int[k + 1];
            for (int i = 0; i < maze.GetLength(0); i++)
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if(maze[i, j] != 'X')
                    {
                        if (maze[i, j] == 'S') start_v = i * maze.GetLength(1) + j;
                        if (maze[i, j] == 'E') 
                            for(int n = 0; n < k + 1; n++)
                                end_v[n] = i * maze.GetLength(1) + j + maze.Length * n;
                        if(i > 0)
                            if(maze[i - 1, j] == 'X')
                                for (int n = 0; n < k; n++)
                                    g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, (i - 1) * maze.GetLength(1) + j + (n + 1) * maze.Length, t);
                            else
                                for (int n = 0; n < k + 1; n++)
                                    g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, (i - 1) * maze.GetLength(1) + j + n * maze.Length, 1);
                        if (i < maze.GetLength(0) - 1)
                            if (maze[i + 1, j] == 'X')
                                for (int n = 0; n < k; n++)
                                    g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, (i + 1) * maze.GetLength(1) + j + (n + 1) * maze.Length, t);
                            else
                                for (int n = 0; n < k + 1; n++)
                                    g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, (i + 1) * maze.GetLength(1) + j + n * maze.Length, 1);
                        if (j > 0)
                            if (maze[i, j - 1] == 'X')
                                for (int n = 0; n < k; n++)
                                    g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, i * maze.GetLength(1) + j - 1 + (n + 1) * maze.Length, t);
                            else
                                for (int n = 0; n < k + 1; n++)
                                    g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, i * maze.GetLength(1) + j - 1 + n * maze.Length, 1);
                        if (j < maze.GetLength(1) - 1)
                        
                            if (maze[i, j + 1] == 'X')
                                for (int n = 0; n < k; n++)
                                    g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, i * maze.GetLength(1) + j + 1 + (n + 1) * maze.Length, t);
                            else
                                for (int n = 0; n < k + 1; n++)
                                    g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, i * maze.GetLength(1) + j + 1 + n * maze.Length, 1);
                    }
                    else
                    {
                        if (i > 0 && maze[i - 1, j] != 'X')
                            for (int n = 0; n < k + 1; n++)
                                g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, (i - 1) * maze.GetLength(1) + j + n * maze.Length, 1);
                        if (i > 0 && maze[i - 1, j] == 'X')
                            for (int n = 0; n < k; n++)
                                g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, (i - 1) * maze.GetLength(1) + j + (n + 1) * maze.Length, 1);
                        if (i < maze.GetLength(0) - 1 && maze[i + 1, j] != 'X')
                            for (int n = 0; n < k + 1; n++)
                                g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, (i + 1) * maze.GetLength(1) + j + n * maze.Length, 1);
                        if (i < maze.GetLength(0) - 1 && maze[i + 1, j] == 'X')
                            for (int n = 0; n < k; n++)
                                g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, (i + 1) * maze.GetLength(1) + j + (n + 1) * maze.Length, 1);
                        if (j > 0 && maze[i, j - 1] != 'X')
                            for (int n = 0; n < k + 1; n++)
                                g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, i * maze.GetLength(1) + j - 1 + n * maze.Length, 1);
                        if (j > 0 && maze[i, j - 1] == 'X')
                            for (int n = 0; n < k; n++)
                                g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, i * maze.GetLength(1) + j - 1 + (n + 1) * maze.Length, 1);
                        if (j < maze.GetLength(1) - 1 && maze[i, j + 1] != 'X')
                            for (int n = 0; n < k + 1; n++)
                                g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, i * maze.GetLength(1) + j + 1 + n * maze.Length, 1);
                        if (j < maze.GetLength(1) - 1 && maze[i, j + 1] == 'X')
                            for (int n = 0; n < k; n++)
                                g.AddEdge(i * maze.GetLength(1) + j + n * maze.Length, i * maze.GetLength(1) + j + 1 + (n + 1) * maze.Length, 1);
                    }
                   
                }
            for(int n = 0; n < k; n++)
                g.AddEdge(end_v[k - n], end_v[k - n - 1], 0);
            var pi = Paths.Dijkstra<int>(g, start_v);
            StringBuilder sb = new StringBuilder();
            if (pi.Reachable(start_v, end_v[0]) == true)
            {
                var p = pi.GetPath(start_v, end_v[0]);
                for (int i = 0; i < p.Length - 1; i++)
                {
                    if (p[i + 1] % maze.Length - p[i] % maze.Length == 1) sb.Append('E');
                    else if (p[i + 1] % maze.Length - p[i] % maze.Length == -1) sb.Append('W');
                    else if (p[i + 1] % maze.Length - p[i] % maze.Length > 1) sb.Append('S');
                    else if (p[i + 1] % maze.Length - p[i] % maze.Length < -1) sb.Append('N');
                }
                path = sb.ToString();
                return pi.GetDistance(start_v, end_v[0]);
            }
            else
            {
                path = "";
                return -1;
            }
        }
    }
}