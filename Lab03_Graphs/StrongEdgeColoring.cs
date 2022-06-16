
namespace ASD
{
    using ASD.Graphs;
    using System.Collections.Generic;

    public class Lab03 : System.MarshalByRefObject
    {
        // Część I
        // Funkcja zwracajaca kwadrat danego grafu.
        // Kwadratem grafu nazywamy graf o takim samym zbiorze wierzchołków jak graf pierwotny, w którym wierzchołki
        // połączone sa krawędzią jeśli w grafie pierwotnym były polączone krawędzia bądź ścieżką złożoną z 2 krawędzi
        // (ale pętli, czyli krawędzi o początku i końcu w tym samym wierzchołku, nie dodajemy!).
        public Graph Square(Graph graph)
        {
            Graph g = new Graph(graph.VertexCount, graph.Representation);

            for (int i = 0; i < graph.VertexCount; i++)
            {
                foreach (int neighbor in graph.OutNeighbors(i))
                {
                    if (i != neighbor) g.AddEdge(i, neighbor);
                    foreach (int neighbor2 in graph.OutNeighbors(neighbor))
                    {
                        if (i != neighbor2) g.AddEdge(i, neighbor2);
                    }
                }
            }

            return g;
        }

        // Część II
        // Funkcja zwracająca Graf krawędziowy danego grafu.
        // Wierzchołki grafu krawędziwego odpowiadają krawędziom grafu pierwotnego, wierzcholki grafu krawędziwego
        // połączone sa krawędzią jeśli w grafie pierwotnym z krawędzi odpowiadającej pierwszemu z nich można przejść
        // na krawędź odpowiadającą drugiemu z nich przez wspólny wierzchołek.

        // Tablicę names tworzymy i wypełniamy według następującej zasady.
        // Każdemu wierzchołkowi grafu krawędziowego odpowiada element tablicy names (o indeksie równym numerowi wierzchołka)
        // zawierający informację z jakiej krawędzi grafu pierwotnego wierzchołek ten powstał.
        // Np.dla wierzchołka powstałego z krawedzi <0,1> do tablicy zapisujemy krotke (0, 1) - przyda się w dalszych etapach
        public Graph LineGraph(Graph graph, out (int x, int y)[] names)
        {
            int edges = 0;
            foreach (Edge e in graph.DFS().SearchAll())
            {
                edges++;
            }
            edges /= 2;

            Graph linegraph = new Graph(edges, graph.Representation);
            names = new (int x, int y)[edges];

            int k = 0;
            foreach (Edge e in graph.DFS().SearchAll())
            {
                if (e.To > e.From) names[k++] = (e.From, e.To);
            }

            for (int i = 0; i < edges; i++)
            {
                for (int j = 0; j < edges; j++)
                {
                    if (i == j) continue;
                    if (names[i].x == names[j].x
                        || names[i].x == names[j].y
                        || names[i].y == names[j].x
                        || names[i].y == names[j].y)
                    {
                        if (linegraph.HasEdge(i, j) == false) linegraph.AddEdge(i, j);
                    }
                }
            }

            return linegraph;
        }

        // Część III
        // Funkcja znajdujaca poprawne kolorowanie wierzchołków danego grafu nieskierowanego.
        // Kolorowanie wierzchołków jest poprawne, gdy każde dwa sąsiadujące wierzchołki mają różne kolory
        // Funkcja ma szukać kolorowania według następujacego algorytmu zachłannego:

        // Dla wszystkich wierzchołków v (od 0 do n-1)
        // pokoloruj wierzcholek v kolorem o najmniejszym możliwym numerze(czyli takim, na który nie są pomalowani jego sąsiedzi)
        // Kolory numerujemy począwszy od 0.

        // UWAGA: Podany opis wyznacza kolorowanie jednoznacznie, jakiekolwiek inne kolorowanie, nawet jeśli spełnia formalnie
        // definicję kolorowania poprawnego, na potrzeby tego zadania będzie uznane za błędne.

        // Funkcja zwraca liczbę użytych kolorów (czyli najwyższy numer użytego koloru + 1),
        // a w tablicy colors zapamiętuje kolory poszczególnych wierzchołkow.
        public int VertexColoring(Graph graph, out int[] colors)
        {
            colors = new int[graph.VertexCount];
            int colUsed = 0;

            for (int i = 0; i < graph.VertexCount; i++)
            {
                colors[i] = -1;
            }

            for (int i = 0; i < graph.VertexCount; i++)
            {
                if (colUsed == 0)
                {
                    colors[colUsed] = colUsed++;
                }
                else // kolejne
                {
                    bool[] colOccupied = new bool[colUsed];
                    foreach (int neighbor in graph.OutNeighbors(i))
                    {
                        if (colors[neighbor] > -1)
                        {
                            colOccupied[colors[neighbor]] = true;
                        }
                    }

                    for (int j = 0; j < colUsed; j++)
                    {
                        if (colOccupied[j] == false)
                        {
                            colors[i] = j;
                            break;
                        }
                    }

                    if (colors[i] == -1)
                    {
                        colors[i] = colUsed++;
                    }
                }
            }


            return colUsed;
        }

        // Funkcja znajduje silne kolorowanie krawędzi danego grafu.
        // Silne kolorowanie krawędzi grafu jest poprawne gdy każde dwie krawędzie, które są ze sobą sąsiednie
        // (czyli można przejść z jednej na drugą przez wspólny wierzchołek)
        // albo są połączone inną krawędzią(czyli można przejść z jednej na drugą przez ową inną krawędź), mają różne kolory.

        // Należy zwrocić nowy graf, który będzie miał strukturę identyczną jak zadany graf,
        // ale w wagach krawędzi zostaną zapisane przydzielone kolory.

        // Wskazówka - to bardzo proste.Należy wykorzystać wszystkie poprzednie funkcje.
        // Zastanowić się co możemy powiedzieć o kolorowaniu wierzchołków kwadratu grafu krawędziowego?
        // Jak się to ma do silnego kolorowania krawędzi grafu pierwotnego?
        public int StrongEdgeColoring(Graph graph, out Graph<int> coloredGraph)
        {
            (int x, int y)[] names;
            Graph linegraph = LineGraph(graph, out names);

            Graph linegraphSquare = Square(linegraph);

            int[] colors;
            int colorsUsed = VertexColoring(linegraphSquare, out colors);

            coloredGraph = new Graph<int>(graph.VertexCount, graph.Representation);

            for (int i = 0; i < names.Length; i++)
            {
                coloredGraph.AddEdge(names[i].x, names[i].y, colors[i]);
            }


            //coloredGraph = null;
            return colorsUsed;
        }

    }

}
