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
            return new List<int>();
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
            return new List<int>();
        }
    }
}