using System;
using ASD.Graphs;
using ASD.Graphs.Testing;
using System.Collections.Generic;

namespace ASD
{

    class Lab03Main
    {

        const int ReverseTestSize = 5;
        const int BipartiteTestSize = 5;
        const int KruskalTestSize = 5;
        const int AcyclicTestSize = 5;

        static bool maskExceptions = false;
        public class SquareofGraphTestCase : TestCase
        {
            private Graph input_graph;
            private Graph graph;
            private Graph expected_result_graph;
            private Graph result_graph;

            public SquareofGraphTestCase(double timeLimit, Exception expectedException, string description, Graph graph, Graph expected_result_graph)
                : base(timeLimit, expectedException, description)
            {
                this.graph = graph;
                this.expected_result_graph = expected_result_graph;
                input_graph = (Graph)graph.Clone(); //zapamietuje graf wejsciowy, zeby potem sprawdzic czy algorytm go nie zmienia
            }

            protected override void PerformTestCase(object prototypeObject)
            {
                result_graph = ((Lab03)prototypeObject).Square(graph);
            }

            protected override (Result resultCode, string message) VerifyTestCase(object settings = null)
            {
                if (!input_graph.Equals(graph))
                    return (Result.WrongResult, "Changed input graph");
                if (result_graph == null)
                    return (Result.WrongResult, "No solution");
                if (result_graph.Representation.GetType() != expected_result_graph.Representation.GetType())
                    return (Result.WrongResult, "Incorrect result graph representation");
                //tu rozwiazanie jest jednoznaczne - sprawdzamy tylko rownosc grafu, izomorfizm niepotrzebny
                if (!expected_result_graph.Equals(result_graph))
                    return (Result.WrongResult, "Incorrect result graph");
                return (Result.Success, $"OK (time:{PerformanceTime,6:#0.000})");
            }

        }

        public class LineGraphTestCase : TestCase
        {
            private Graph input_graph;
            private Graph graph;
            private Graph expected_result_graph;
            private Graph result_graph;
            private (int, int)[] names;

            public LineGraphTestCase(double timeLimit, Exception expectedException, string description, Graph graph, Graph res_graph)
                : base(timeLimit, expectedException, description)
            {
                this.graph = graph;
                this.expected_result_graph = res_graph;
                input_graph = (Graph)graph.Clone(); //zapamietuje graf wejsciowy, zeby potem sprawdzic czy algorytm go nie zmienia
            }

            protected override void PerformTestCase(object prototypeObject)
            {
                result_graph = ((Lab03)prototypeObject).LineGraph(graph, out names);
            }

            protected override (Result resultCode, string message) VerifyTestCase(object settings = null)
            {
                string comm = "";
                if (!input_graph.Equals(graph))
                    return (Result.WrongResult, "Changed input graph");
                if (result_graph == null)
                    return (Result.WrongResult, "No solution");
                if (result_graph.GetType() != expected_result_graph.GetType())
                    return (Result.WrongResult, "Incorrect result graph representation");

                if (expected_result_graph.VertexCount != result_graph.VertexCount)
                    return (Result.WrongResult, "Incorrect result graph");
                if (!(result_graph.VertexCount >= 4900 || expected_result_graph.Equals(result_graph)))
                    comm += "(result graph not verified)";

                int[] degrees_expected = new int[expected_result_graph.VertexCount];
                int[] degrees_result = new int[result_graph.VertexCount];

                for (int i = 0; i < degrees_expected.GetLength(0); i++)
                {
                    foreach (int u in expected_result_graph.OutNeighbors(i))
                        degrees_expected[i]++;
                    foreach (int u in result_graph.OutNeighbors(i))
                        degrees_result[i]++;
                }

                Array.Sort(degrees_expected);
                Array.Sort(degrees_result);
                for (int i = 0; i < degrees_expected.GetLength(0); i++)
                    if (degrees_result[i] != degrees_expected[i])
                        return (Result.WrongResult, "Incorrect result graph");

                if (names == null)
                    return (Result.WrongResult, "names table is null");
                if (names.Length != expected_result_graph.VertexCount)
                    return (Result.WrongResult, "Incorrect size of names table");
                foreach (var elem in names)
                    if (!graph.HasEdge(elem.Item1, elem.Item2))
                        return (Result.WrongResult, $"Incorrect names table element ({elem.Item1},{elem.Item2})");

                return (Result.Success, $"OK {comm} (time:{PerformanceTime,6:#0.000})");
            }

        }

        public class VertexColoringTestCase : TestCase
        {
            private Graph input_graph;
            private Graph graph;
            private int expected_colors_number;
            private int colors_number;
            private int[] expected_colors;
            private int[] colors;

            public VertexColoringTestCase(double timeLimit, Exception expectedException, string description, Graph graph, int num, int[] colors)
                : base(timeLimit, expectedException, description)
            {
                this.graph = graph;
                this.expected_colors_number = num;
                this.expected_colors = colors;
                input_graph = (Graph)graph.Clone(); //zapamietuje graf wejsciowy, zeby potem sprawdzic czy algorytm go nie zmienia
            }

            protected override void PerformTestCase(object prototypeObject)
            {
                colors_number = ((Lab03)prototypeObject).VertexColoring(graph, out colors);
            }

            protected override (Result resultCode, string message) VerifyTestCase(object settings = null)
            {
                if (!input_graph.Equals(graph))
                    return (Result.WrongResult, "Changed input graph");
                if (colors == null)
                    return (Result.WrongResult, "No solution");
                if (expected_colors_number != colors_number)
                    return (Result.WrongResult, $"Incorrect numbers of colors: {colors_number}, should be: {expected_colors_number}");

                //algorytm podany jest w taki, sposob, ze kolorowanie mozliwe jest tylko jedno - sprawdzam czy tablice sa identyczne
                for (int id = 0; id < colors_number; ++id)
                {
                    if (expected_colors[id] != colors[id])
                    {
                        string message = $"Incorrect coloring";
                        string c1 = "", c2 = "";
                        for (int i = 0; i < graph.VertexCount; ++i)
                        {
                            c1 += "[ " + i + ": " + colors[i] + "] ";
                            c2 += "[ " + i + ": " + expected_colors[i] + "] ";
                        }
                        message += "\nResult: \n" + c1 + "\n" + "Expected: \n" + c2;
                        //wyswietlic kolorowanie
                        return (Result.WrongResult, message);
                    }
                }
                return (Result.Success, $"OK (time:{PerformanceTime,6:#0.000})");
            }

        }

        public class StrongEdgeColoringTestCase : TestCase
        {
            private Graph input_graph;
            private Graph graph;
            private Graph expected_result_graph;
            private Graph<int> result_graph;
            private int colors_number;

            public StrongEdgeColoringTestCase(double timeLimit, Exception expectedException, string description, Graph graph, Graph<int> res_graph)
                : base(timeLimit, expectedException, description)
            {
                this.graph = graph;
                this.expected_result_graph = res_graph;
                input_graph = (Graph)graph.Clone(); //zapamietuje graf wejsciowy, zeby potem sprawdzic czy algorytm go nie zmienia
            }

            protected override void PerformTestCase(object prototypeObject)
            {
                colors_number = ((Lab03)prototypeObject).StrongEdgeColoring(graph, out result_graph);
            }

            protected override (Result resultCode, string message) VerifyTestCase(object settings = null)
            {
                // kolorowanie, a nawet liczba uzytych kolorow, moze byc inna niż w naszym rozwiazaniu
                // ponieważ po drodze może byc inny (chociaż izomorficzny) graf krawedziowy

                if (!input_graph.Equals(graph))
                    return (Result.WrongResult, "Changed input graph");
                if (result_graph == null)
                    return (Result.WrongResult, "No solution");
                if (result_graph.Representation.GetType() != expected_result_graph.Representation.GetType())
                    return (Result.WrongResult, "Incorrect result graph representation");

                if (expected_result_graph.VertexCount != result_graph.VertexCount)
                    return (Result.WrongResult, "Incorrect result graph");

                if (expected_result_graph.Equals(result_graph))
                    return (Result.Success, $"OK (time:{PerformanceTime,6:#0.000})");   // kolorowanie takie jak u nas

                if (colors_number < 0)
                    return (Result.WrongResult, $"Incorrect numbers of colors: {colors_number}");

                bool[] used = new bool[colors_number];
                //tu sprawdzam czy zwrocone kolorowanie krawedzi jest poprawne
                for (int i = 0; i < result_graph.VertexCount; ++i)
                    foreach (Edge<int> e1 in result_graph.OutEdges(i))
                    {
                        if (e1.weight < 0 || e1.weight >= colors_number)
                            return (Result.WrongResult, $"Invalid color: {e1.weight}");
                        used[(int)e1.weight] = true;
                        foreach (Edge<int> e2 in result_graph.OutEdges(e1.To))
                        {
                            if (e1.From != e2.To && e1.weight == e2.weight)
                                return (Result.WrongResult, $"Incorrect coloring. Edges [{e1.From}, {e1.To}] and [{e2.From}, {e2.To}] have the same color!");
                            foreach (Edge<int> e3 in result_graph.OutEdges(e2.To))
                                if (!(e1.From == e3.From && e1.To == e3.To) && !(e1.From == e3.To && e1.To == e3.From) && e1.weight == e3.weight)
                                    return (Result.WrongResult, $"Incorrect coloring. Edges [{e1.From}, {e1.To}] and [{e3.From}, {e3.To}] have the same color!");
                        }
                    }
                if (!System.Linq.Enumerable.All(used, x => x))
                    return (Result.WrongResult, "Incorrect result graph");

                return (Result.Success, $"OK (time:{PerformanceTime,6:#0.000})");
            }

        }


        class Lab03TestModule : TestModule
        {

            public override void PrepareTestSets()
            {
                var rgg = new RandomGraphGenerator(12345);
                int n = 6;
                Graph[] graphs = new Graph[n];
                string[] descriptions = new string[n];

                Graph[] squares = new Graph[n];
                Graph[] linegraphs = new Graph[n];
                List<int[]> colors = new List<int[]>();
                int[] chn = new int[n]; //chromatic number
                Graph<int>[] sec_graphs = new Graph<int>[n];

                //przyklady z zajec
                graphs[0] = new Graph(5, new MatrixGraphRepresentation());
                graphs[0].AddEdge(0, 1);
                graphs[0].AddEdge(0, 3);
                graphs[0].AddEdge(1, 2);
                graphs[0].AddEdge(2, 0);
                graphs[0].AddEdge(2, 3);
                graphs[0].AddEdge(3, 4);

                //sciezka P6
                graphs[1] = new Graph(6);
                graphs[1].AddEdge(0, 1);
                graphs[1].AddEdge(1, 2);
                graphs[1].AddEdge(2, 3);
                graphs[1].AddEdge(3, 4);
                graphs[1].AddEdge(4, 5);

                graphs[2] = new Graph(9, new MatrixGraphRepresentation());
                graphs[2].AddEdge(0, 3);
                graphs[2].AddEdge(1, 0);
                graphs[2].AddEdge(6, 0);
                graphs[2].AddEdge(1, 5);
                graphs[2].AddEdge(5, 4);
                graphs[2].AddEdge(5, 2);
                graphs[2].AddEdge(5, 6);
                graphs[2].AddEdge(6, 7);
                graphs[2].AddEdge(7, 8);

                //cykl C5
                graphs[3] = new Graph(5);
                graphs[3].AddEdge(0, 1);
                graphs[3].AddEdge(1, 2);
                graphs[3].AddEdge(2, 3);
                graphs[3].AddEdge(3, 4);
                graphs[3].AddEdge(4, 0);

                //klika K5
                graphs[4] = rgg.Graph(5, 1.0);

                //duzy cykl
                int g5vc = 9000;
                graphs[5] = GraphExamples.Cycle(g5vc);

                squares[0] = (Graph)graphs[0].Clone();
                squares[0].AddEdge(0, 4);
                squares[0].AddEdge(1, 3);
                squares[0].AddEdge(2, 4);

                squares[1] = (Graph)graphs[1].Clone();
                squares[1].AddEdge(0, 2);
                squares[1].AddEdge(1, 3);
                squares[1].AddEdge(2, 4);
                squares[1].AddEdge(3, 5);

                squares[2] = (Graph)graphs[2].Clone();
                squares[2].AddEdge(0, 5);
                squares[2].AddEdge(0, 6);
                squares[2].AddEdge(0, 7);
                squares[2].AddEdge(1, 2);
                squares[2].AddEdge(1, 3);
                squares[2].AddEdge(1, 4);
                squares[2].AddEdge(1, 6);
                squares[2].AddEdge(2, 4);
                squares[2].AddEdge(2, 6);
                squares[2].AddEdge(3, 6);
                squares[2].AddEdge(4, 6);
                squares[2].AddEdge(5, 7);
                squares[2].AddEdge(6, 8);

                squares[3] = rgg.Graph(5, 1.0);

                squares[4] = rgg.Graph(5, 1.0);

                squares[5] = new Graph(g5vc);
                for (int i = 0; i < g5vc; ++i)
                {
                    squares[5].AddEdge(i, (i + 1) % g5vc);
                    squares[5].AddEdge(i, (i + 2) % g5vc);
                }

                linegraphs[0] = new Graph(6, new MatrixGraphRepresentation());
                linegraphs[0].AddEdge(0, 1);
                linegraphs[0].AddEdge(0, 2);
                linegraphs[0].AddEdge(0, 3);
                linegraphs[0].AddEdge(1, 2);
                linegraphs[0].AddEdge(1, 3);
                linegraphs[0].AddEdge(1, 4);
                linegraphs[0].AddEdge(2, 4);
                linegraphs[0].AddEdge(2, 5);
                linegraphs[0].AddEdge(3, 4);
                linegraphs[0].AddEdge(4, 5);

                linegraphs[1] = new Graph(5);
                linegraphs[1].AddEdge(0, 1);
                linegraphs[1].AddEdge(1, 2);
                linegraphs[1].AddEdge(2, 3);
                linegraphs[1].AddEdge(3, 4);

                linegraphs[2] = new Graph(9);
                linegraphs[2].AddEdge(0, 1);
                linegraphs[2].AddEdge(0, 2);
                linegraphs[2].AddEdge(0, 3);
                linegraphs[2].AddEdge(1, 2);
                linegraphs[2].AddEdge(2, 6);
                linegraphs[2].AddEdge(2, 7);
                linegraphs[2].AddEdge(3, 4);
                linegraphs[2].AddEdge(3, 5);
                linegraphs[2].AddEdge(3, 6);
                linegraphs[2].AddEdge(4, 5);
                linegraphs[2].AddEdge(4, 6);
                linegraphs[2].AddEdge(5, 6);
                linegraphs[2].AddEdge(6, 7);
                linegraphs[2].AddEdge(7, 8);

                linegraphs[3] = new Graph(5);
                linegraphs[3].AddEdge(0, 1);
                linegraphs[3].AddEdge(0, 2);
                linegraphs[3].AddEdge(1, 4);
                linegraphs[3].AddEdge(2, 3);
                linegraphs[3].AddEdge(3, 4);

                linegraphs[4] = new Graph(10, new MatrixGraphRepresentation());
                linegraphs[4].AddEdge(0, 1);
                linegraphs[4].AddEdge(0, 2);
                linegraphs[4].AddEdge(0, 3);
                linegraphs[4].AddEdge(0, 4);
                linegraphs[4].AddEdge(0, 5);
                linegraphs[4].AddEdge(0, 6);
                linegraphs[4].AddEdge(1, 2);
                linegraphs[4].AddEdge(1, 3);
                linegraphs[4].AddEdge(1, 4);
                linegraphs[4].AddEdge(1, 7);
                linegraphs[4].AddEdge(1, 8);
                linegraphs[4].AddEdge(2, 3);
                linegraphs[4].AddEdge(2, 5);
                linegraphs[4].AddEdge(2, 7);
                linegraphs[4].AddEdge(2, 9);
                linegraphs[4].AddEdge(3, 6);
                linegraphs[4].AddEdge(3, 8);
                linegraphs[4].AddEdge(3, 9);
                linegraphs[4].AddEdge(4, 5);
                linegraphs[4].AddEdge(4, 6);
                linegraphs[4].AddEdge(4, 7);
                linegraphs[4].AddEdge(4, 8);
                linegraphs[4].AddEdge(5, 6);
                linegraphs[4].AddEdge(5, 7);
                linegraphs[4].AddEdge(5, 9);
                linegraphs[4].AddEdge(6, 8);
                linegraphs[4].AddEdge(6, 9);
                linegraphs[4].AddEdge(7, 8);
                linegraphs[4].AddEdge(7, 9);
                linegraphs[4].AddEdge(8, 9);

                linegraphs[5] = new Graph(g5vc);
                for (int i = 0; i < g5vc; ++i)
                {
                    linegraphs[5].AddEdge(i, (i + 1) % g5vc);
                }

                colors.Add(new int[5]);
                colors[0][0] = 0;
                colors[0][1] = 1;
                colors[0][2] = 2;
                colors[0][3] = 1;
                colors[0][4] = 0;
                chn[0] = 3;

                colors.Add(new int[6]);
                colors[1][0] = 0;
                colors[1][1] = 1;
                colors[1][2] = 0;
                colors[1][3] = 1;
                colors[1][4] = 0;
                colors[1][5] = 1;
                chn[1] = 2;

                colors.Add(new int[9]);
                colors[2][0] = 0;
                colors[2][1] = 1;
                colors[2][2] = 0;
                colors[2][3] = 1;
                colors[2][4] = 0;
                colors[2][5] = 2;
                colors[2][6] = 1;
                colors[2][7] = 0;
                colors[2][8] = 1;
                chn[2] = 3;

                colors.Add(new int[5]);
                colors[3][0] = 0;
                colors[3][1] = 1;
                colors[3][2] = 0;
                colors[3][3] = 1;
                colors[3][4] = 2;
                chn[3] = 3;

                colors.Add(new int[5]);
                colors[4][0] = 0;
                colors[4][1] = 1;
                colors[4][2] = 2;
                colors[4][3] = 3;
                colors[4][4] = 4;
                chn[4] = 5;

                chn[5] = 2;
                int[] t = new int[g5vc];
                for (int a = 0; a < g5vc; a++)
                    t[a] = a % 2;
                colors.Add(t);

                sec_graphs[0] = new Graph<int>(graphs[0].VertexCount, graphs[0].Representation);
                sec_graphs[0].AddEdge(0, 1, 0);
                sec_graphs[0].AddEdge(0, 3, 2);
                sec_graphs[0].AddEdge(1, 2, 3);
                sec_graphs[0].AddEdge(2, 0, 1);
                sec_graphs[0].AddEdge(2, 3, 4);
                sec_graphs[0].AddEdge(3, 4, 5);

                sec_graphs[1] = new Graph<int>(graphs[1].VertexCount, graphs[1].Representation);
                sec_graphs[1].AddEdge(0, 1, 0);
                sec_graphs[1].AddEdge(1, 2, 1);
                sec_graphs[1].AddEdge(2, 3, 2);
                sec_graphs[1].AddEdge(3, 4, 0);
                sec_graphs[1].AddEdge(4, 5, 1);

                sec_graphs[2] = new Graph<int>(graphs[2].VertexCount, graphs[2].Representation);
                sec_graphs[2].AddEdge(0, 3, 1);
                sec_graphs[2].AddEdge(1, 0, 0);
                sec_graphs[2].AddEdge(6, 0, 2);
                sec_graphs[2].AddEdge(1, 5, 3);
                sec_graphs[2].AddEdge(5, 4, 4);
                sec_graphs[2].AddEdge(5, 2, 1);
                sec_graphs[2].AddEdge(5, 6, 5);
                sec_graphs[2].AddEdge(6, 7, 6);
                sec_graphs[2].AddEdge(7, 8, 0);

                sec_graphs[3] = new Graph<int>(graphs[3].VertexCount, graphs[3].Representation);
                sec_graphs[3].AddEdge(0, 1, 0);
                sec_graphs[3].AddEdge(1, 2, 2);
                sec_graphs[3].AddEdge(2, 3, 3);
                sec_graphs[3].AddEdge(3, 4, 4);
                sec_graphs[3].AddEdge(4, 0, 1);

                sec_graphs[4] = new Graph<int>(graphs[4].VertexCount, graphs[4].Representation);
                sec_graphs[4].AddEdge(0, 1, 0);
                sec_graphs[4].AddEdge(0, 2, 1);
                sec_graphs[4].AddEdge(0, 3, 2);
                sec_graphs[4].AddEdge(0, 4, 3);
                sec_graphs[4].AddEdge(1, 2, 4);
                sec_graphs[4].AddEdge(1, 3, 5);
                sec_graphs[4].AddEdge(1, 4, 6);
                sec_graphs[4].AddEdge(2, 3, 7);
                sec_graphs[4].AddEdge(2, 4, 8);
                sec_graphs[4].AddEdge(3, 4, 9);

                sec_graphs[5] = new Graph<int>(graphs[5].VertexCount, graphs[5].Representation);
                for (int i = 0; i < g5vc; ++i)
                    sec_graphs[5].AddEdge(i, (i + 1) % g5vc, i % 3);

                TestSets["LabSquareOfGraphTests"] = new TestSet(new Lab03(), "Lab. - Part 1 - square of graph");
                TestSets["LabLineGraphTests"] = new TestSet(new Lab03(), "Lab. - Part 2 - line graph");
                TestSets["LabVertexColoringTests"] = new TestSet(new Lab03(), "Lab. - Part 3 - vertex coloring");
                TestSets["LabStrongEdgeColoringTests"] = new TestSet(new Lab03(), "Lab. - Part 4 - strong edge coloring");

                for (int i = 0; i < n; ++i)
                {
                    TestSets["LabSquareOfGraphTests"].TestCases.Add(new SquareofGraphTestCase(i == 5 ? 30 : 1, null, descriptions[i], graphs[i], squares[i]));
                    TestSets["LabLineGraphTests"].TestCases.Add(new LineGraphTestCase(i == 5 ? 30 : 1, null, descriptions[i], graphs[i], linegraphs[i]));
                    TestSets["LabVertexColoringTests"].TestCases.Add(new VertexColoringTestCase(i == 5 ? 30 : 1, null, descriptions[i], graphs[i], chn[i], colors[i]));
                    TestSets["LabStrongEdgeColoringTests"].TestCases.Add(new StrongEdgeColoringTestCase(i == 5 ? 30 : 1, null, descriptions[i], graphs[i], sec_graphs[i]));
                }

            }

        }

        static void Main(string[] args)
        {
            Console.WriteLine("\nPart 1 - Reverse");
            TestReverse();
            Console.WriteLine("\nPart 2 - Bipartite");
            TestBipartite();
            Console.WriteLine("\nPart 3 - Kruskal");
            TestKruskal();
            Console.WriteLine("\nPart 4 - Acyclic");
            TestAcyclic();
            Console.WriteLine();
            Lab03TestModule lab03test = new Lab03TestModule();
            lab03test.PrepareTestSets();
            foreach (var ts in lab03test.TestSets)
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);

            Console.ReadLine();
        }

        private static void TestReverse()
        {
            var rgg = new RandomGraphGenerator(12345);
            DiGraph[] g = new DiGraph[ReverseTestSize];
            DiGraph r, gg;
            g[0] = rgg.DiGraph(10, 0.7);
            g[1] = rgg.DiGraph(100, 0.1);
            rgg.SetRepresentation(new MatrixGraphRepresentation());
            g[2] = rgg.DiGraph(10, 0.5);
            g[3] = rgg.DiGraph(30, 0.1);
            g[4] = new DiGraph(50000);

            for (int i = 0; i < ReverseTestSize; ++i)
            {
                Console.Write($"  Test {i} - ");
                gg = (DiGraph)g[i].Clone();
                try
                {
                    r = (new Lab03GraphFunctions()).Lab03Reverse(g[i]);
                    if (r == null)
                    {
                        Console.WriteLine("Failed : null returned");
                        continue;
                    }
                    if (r.Representation.GetType() != g[i].Representation.GetType())
                    {
                        Console.WriteLine("Failed : invalid graph representation");
                        continue;
                    }
                    if (!g[i].Equals(gg))
                    {
                        Console.WriteLine("Failed : graph was destroyed");
                        continue;
                    }
                    bool ok = true;
                    if (r.VertexCount != gg.VertexCount)
                        ok = false;
                    if (ok)
                        for (int v = 0; v < r.VertexCount; v++)
                        {
                            foreach (int u in r.OutNeighbors(v))
                                if (!gg.HasEdge(u, v))
                                    ok = false;
                            foreach (int u in gg.OutNeighbors(v))
                                if (!r.HasEdge(u, v))
                                    ok = false;
                        }
                    if (!ok)
                    {
                        Console.WriteLine("Failed : bad result");
                        continue;
                    }
                    Console.WriteLine("Passed");
                }
                catch (System.Exception e) when (maskExceptions)
                {
                    Console.WriteLine($"Failed : {e.GetType()} : {e.Message}");
                }
            }
        }

        private static void TestBipartite()
        {
            var rgg = new RandomGraphGenerator(12345);
            Graph[] g = new Graph[BipartiteTestSize];
            bool?[] res = { true, false, false, false, true };
            Graph gg;
            bool r;
            int[] part;
            g[0] = rgg.BipartiteGraph(4, 3, 0.4);
            g[1] = rgg.Graph(100, 0.1);
            g[2] = rgg.Graph(10, 0.5);
            g[3] = GraphExamples.Cycle(50001);
            g[4] = new Graph(50000);

            for (int i = 0; i < BipartiteTestSize; ++i)
            {
                Console.Write($"  Test {i} - ");
                gg = (Graph)g[i].Clone();
                try
                {
                    r = (new Lab03GraphFunctions()).Lab03IsBipartite(g[i], out part);
                    if (!g[i].Equals(gg))
                    {
                        Console.WriteLine("Failed : graph was destroyed");
                        continue;
                    }
                    if (r != res[i])
                    {
                        Console.WriteLine("Failed : bad result");
                        continue;
                    }
                    if (r && !IsProperPartition(g[i], part))
                    {
                        Console.WriteLine("Failed : invalid partition");
                        continue;
                    }
                    if (!r && part != null)
                    {
                        Console.WriteLine("Failed : part==null expected");
                        continue;
                    }
                    Console.WriteLine("Passed");
                }
                catch (System.Exception e) when (maskExceptions)
                {
                    Console.WriteLine($"Failed : {e.GetType()} : {e.Message}");
                }
            }
        }

        private static void TestKruskal()
        {
            var rgg = new RandomGraphGenerator(12345);
            Graph<int>[] g = new Graph<int>[KruskalTestSize];
            Graph<int> r, gg;
            int[] res = { -133, -76248, 9, 20462, 0};
            int mstwr;
            g[0] = rgg.WeightedGraph(5, 0.7, -99, 99);
            g[1] = rgg.WeightedGraph(100, 0.1, -999, 999);
            g[2] = rgg.WeightedGraph(10, 0.5, 1, 1);
            g[3] = rgg.AssignWeights(GraphExamples.Cycle(50000), -99, 99);
            g[4] = new Graph<int>(50000);

            for (int i = 0; i < KruskalTestSize; ++i)
            {
                Console.Write($"  Test {i} - ");
                gg = (Graph<int>)g[i].Clone();
                try
                {
                    r = (new Lab03GraphFunctions()).Lab03Kruskal(g[i], out mstwr);
                    if (r == null)
                    {
                        Console.WriteLine("Failed : null returned");
                        continue;
                    }
                    if (r.Directed)
                    {
                        Console.WriteLine("Failed : returned graph is directed");
                        continue;
                    }
                    if (r.GetType() != g[i].GetType())
                    {
                        Console.WriteLine("Failed : invalid graph representation");
                        continue;
                    }
                    if (!g[i].Equals(gg))
                    {
                        Console.WriteLine("Failed : graph was destroyed");
                        continue;
                    }
                    if (mstwr != res[i])
                    {
                        Console.WriteLine($"Failed : bad result (expected = {res[i]}, actual = {mstwr})");
                        continue;
                    }
                    if(!IsSubtree(g[i], r, mstwr))
                    {
                        Console.WriteLine($"Failed : wrong spanning tree");
                        continue;
                    }
                    Console.WriteLine("Passed");
                }
                catch (System.Exception e) when (maskExceptions)
                {
                    Console.WriteLine($"Failed : {e.GetType()} : {e.Message}");
                }
            }
        }

        private static void TestAcyclic()
        {
            var rgg = new RandomGraphGenerator(12345);
            Graph[] g = new Graph[AcyclicTestSize];
            bool?[] res = { true, false, false, false, true };
            Graph gg;
            bool r;
            g[0] = rgg.Tree(7);
            g[1] = rgg.Graph(100, 0.1);
            g[2] = rgg.Graph(10, 0.5);
            g[3] = GraphExamples.Cycle(50000);
            g[4] = new Graph(50000);

            for (int i = 0; i < AcyclicTestSize; ++i)
            {
                Console.Write($"  Test {i} - ");
                gg = (Graph) g[i].Clone();
                try
                {
                    r = (new Lab03GraphFunctions()).Lab03IsUndirectedAcyclic(g[i]);
                    if (!g[i].Equals(gg))
                    {
                        Console.WriteLine("Failed : graph was destroyed");
                        continue;
                    }
                    if (r != res[i])
                    {
                        Console.WriteLine("Failed : bad result");
                        continue;
                    }
                    Console.WriteLine("Passed");
                }
                catch (System.Exception e) when (maskExceptions)
                {
                    Console.WriteLine($"Failed : {e.GetType()} : {e.Message}");
                }
            }
        }

        private static bool IsSubtree(Graph<int> g, Graph<int> t, int mstwr)
        {
            if (t == null)
                return false;
            int edges = 0;
            if (g.VertexCount != t.VertexCount) return false;
            for (int v = 0; v < t.VertexCount; ++v)
                foreach (int u in t.OutNeighbors(v))
                {
                    if (u < v)
                    {
                        edges++;
                        mstwr -= g.GetEdgeWeight(v, u);
                    }
                    if (!g.HasEdge(v, u))
                        return false;
                }
            if (mstwr != 0)
                return false;
            return edges <= g.VertexCount - 1;
        }

        private static bool IsProperPartition(Graph g, int[] part)
        {
            if (part == null || part.Length != g.VertexCount) return false;
            for (int v = 0; v < g.VertexCount; ++v)
                if (part[v] != 1 && part[v] != 2)
                    return false;
            for (int v = 0; v < g.VertexCount; ++v)
                foreach (int u in g.OutNeighbors(v))
                    if (part[u] == part[v])
                        return false;
            return true;
        }

    }  // class Lab03

}
