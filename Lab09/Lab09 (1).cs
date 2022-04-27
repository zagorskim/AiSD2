
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

/// <summary>
/// Klasa rozszerzająca klasę Graph o rozwiązania problemów największej kliki i izomorfizmu grafów metodą pełnego przeglądu (backtracking)
/// </summary>
public static class Lab10GraphExtender
{
    /// <summary>
    /// Wyznacza największą klikę w grafie i jej rozmiar metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <param name="clique">Wierzchołki znalezionej największej kliki - parametr wyjściowy</param>
    /// <returns>Rozmiar największej kliki</returns>
    /// <remarks>
    /// Nie wolno modyfikować badanego grafu.
    /// </remarks>
    public static int MaxClique(this Graph g, out int[] clique)
    {
        var curr = new List<int>();
        var res = new List<int>();
        FindClique(g, curr, ref res);
        clique = res.ToArray();
        return res.Count;
    }

    public static void FindClique(Graph g, List<int> curr, ref List<int> max)
    {
        for(int i = curr.Count == 0 ? 0 : curr[curr.Count - 1] + 1; i < g.VertexCount - max.Count + curr.Count; i++)
        {
            int j = 0;
            if (g.OutNeighbors(i).Count() < curr.Count) continue;
            for (j = 0; j < curr.Count; j++)
                if (!g.HasEdge(curr[j], i)) break;
            if (j < curr.Count) continue;
            curr.Add(i);
            if (max.Count < curr.Count) max = new List<int>(curr);
            FindClique(g, curr, ref max);
            curr.Remove(i);
        }
    }

    /// <summary>
    /// Bada izomorfizm grafów metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <param name="g">Pierwszy badany graf</param>
    /// <param name="h">Drugi badany graf</param>
    /// <param name="map">Mapowanie wierzchołków grafu h na wierzchołki grafu g (jeśli grafy nie są izomorficzne to null) - parametr wyjściowy</param>
    /// <returns>Informacja, czy grafy g i h są izomorficzne</returns>
    /// <remarks>
    /// 1) Uwzględniamy wagi krawędzi
    /// 3) Nie wolno modyfikować badanych grafów.
    /// </remarks>
    public static bool IsomorphismTest(this Graph<int> g, Graph<int> h, out int[] map)
    {
        map = null;
        if (g.VertexCount < h.VertexCount) 
            return false;
        var used = new bool[g.VertexCount];
        int vc = 0;
        map = new int[g.VertexCount];
        if(FindMapping(g, h, used, vc, ref map))
            return true;
        map = null;
        return false;
    }
    public static bool FindMapping(Graph<int> g, Graph<int> h, bool[] used, int vc, ref int[] iso)
    {
        if (vc == g.VertexCount)
            return true;
        for (int i = 0; i < g.VertexCount; i++)
        {
            int j = 0;
            if (!used[i] && g.OutNeighbors(i).Count() == h.OutNeighbors(vc).Count())
            {
                for (j = 0; j < vc; j++)
                    if (g.HasEdge(iso[j], i) != h.HasEdge(j, vc) || g.HasEdge(iso[j], i) && g.GetEdgeWeight(iso[j], i) != h.GetEdgeWeight(j, vc)) // h.HasEdge(j, vc) && !g.HasEdge(iso[j], i)
                        break;
                if (j < vc)
                    continue;
                iso[vc] = i;
                used[i] = true;
                if (FindMapping(g, h, used, vc + 1, ref iso))
                    return true;
                used[i] = false;
            }
        }
        return false;
    }
}

