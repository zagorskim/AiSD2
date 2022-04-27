using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace ASD_lab08
{
    [Serializable]
    public struct Cat
    {
        /// <summary>
        /// Zawiera identyfikatory osób, które kot zaakceptuje
        /// </summary>
        public int[] AcceptablePeople { get; }

        public Cat(int[] acceptablePeople)
        {
            AcceptablePeople = acceptablePeople;
        }
    }

    [Serializable]
    public struct Person
    {
        /// <summary>
        /// Maksymalna liczba kotów, którymi zajmie się opiekun
        /// </summary>
        public int MaxCats { get; }

        /// <summary>
        /// Kwoty, które osoba życzy sobie za opiekę nad kotami (catId -> int)
        /// </summary>
        public int[] Salaries { get; }

        public Person(int maxCats, int[] salaries)
        {
            MaxCats = maxCats;
            Salaries = salaries;
        }
    }

    public class Cats : MarshalByRefObject
    {
        /// <summary>
        /// Zadanie pierwsze, w którym nie bierzemy pod uwagę pieniędzy jakie nam przyjdzie zapłacić opiekunom
        /// </summary>
        /// <param name="cats">Tablica zawierające nasze koty</param>
        /// <param name="people">Tablica zawierająca dostępnych opiekunów</param>
        /// <returns>
        /// isPossible: wartość logiczna oznaczająca, czy przypisanie jest możliwe, 
        /// assignment: przypisanie kotów do opiekunów (personId -> [catId])
        /// </returns>
        public (bool isPossible, int[][] assignment) StageOne(Cat[] cats, Person[] people)
        {
            bool res1 = false;
            int[][] res2 = new int[people.Length][];
            for (int i = 0; i < res2.GetLength(0); i++) res2[i] = new int[0];
            var graph = new DiGraph<int>(people.Length + cats.Length + 2);
            for (int i = 1; i < cats.Length + 1; i++)
                graph.AddEdge(0, i, 1);
            for (int i = 1; i < cats.Length + 1; i++)
                for (int j = 0; j < cats[i - 1].AcceptablePeople.Length; j++)
                    graph.AddEdge(i, cats.Length + cats[i - 1].AcceptablePeople[j] + 1, 1);
            for (int i = cats.Length + 1; i < people.Length + cats.Length + 1; i++)
                graph.AddEdge(i, cats.Length + people.Length + 1, people[i - cats.Length - 1].MaxCats);
            var (flowValue, f) = Flows.FordFulkerson<int>(graph, 0, cats.Length + people.Length + 1);
            if(flowValue >= cats.Length)
            {
                res1 = true;
                if (flowValue != 0)
                {
                    for (int i = cats.Length + 1; i < people.Length + cats.Length + 1; i++)
                    {
                        int count1 = 0, count2 = 0;
                        foreach (var item in f.OutEdges(i)) count1++;
                        if(f.HasEdge(i, graph.VertexCount - 1)) res2[i - cats.Length - 1] = new int[f.GetEdgeWeight(i, graph.VertexCount - 1)];
                        for (int j = 1; j < cats.Length + 1; j++)
                            if (f.HasEdge(j, i) == true)
                            {
                                res2[i - cats.Length - 1][count2] = j - 1;
                                count2++;
                            }
                    }
                }
            }
            return (res1, res2);
        }

        /// <summary>
        /// Zadanie drugie, w którym bierzemy pod uwagę kwoty jakie nam przyjdzie zapłacić
        /// </summary>
        /// <param name="cats">Tablica zawierające nasze koty</param>
        /// <param name="people">Tablica zawierająca dostępnych opiekunów</param>
        /// <returns>
        /// isPossible: wartość logiczna oznaczająca, czy przypisanie jest możliwe,
        /// assignment: przypisanie kotów do opiekunów (personId -> [catId]),
        /// minCost: minimalna suma pieniędzy do zapłacenia opiekunom za opiekę nad wszystkimi kotami
        /// </returns>
        public (bool isPossible, int[][] assignment, int minCost) StageTwo(Cat[] cats, Person[] people)
        {
            bool res1 = false;
            int[][] res2 = new int[people.Length][];
            int res3 = 0;
            for (int i = 0; i < res2.GetLength(0); i++) res2[i] = new int[0];
            var graph = new NetworkWithCosts<int, int>(people.Length + cats.Length + 2);
            for (int i = 1; i < cats.Length + 1; i++)
                graph.AddEdge(0, i, (1, 0));
            for (int i = 1; i < cats.Length + 1; i++)
                for (int j = 0; j < cats[i - 1].AcceptablePeople.Length; j++)
                    graph.AddEdge(i, cats.Length + cats[i - 1].AcceptablePeople[j] + 1, (1, people[cats[i - 1].AcceptablePeople[j]].Salaries[i - 1]));
            for (int i = cats.Length + 1; i < people.Length + cats.Length + 1; i++)
                graph.AddEdge(i, cats.Length + people.Length + 1, (people[i - cats.Length - 1].MaxCats, 0));
            var (flowValue, flowCost, f) = Flows.MinCostMaxFlow<int, int>(graph, 0, cats.Length + people.Length + 1);
            if (flowValue >= cats.Length)
            {
                res1 = true;
                res3 = flowCost;
                if (flowValue != 0)
                {
                    for (int i = cats.Length + 1; i < people.Length + cats.Length + 1; i++)
                    {
                        int count1 = 0, count2 = 0;
                        foreach (var item in f.OutEdges(i)) count1++;
                        if (f.HasEdge(i, graph.VertexCount - 1)) res2[i - cats.Length - 1] = new int[f.GetEdgeWeight(i, graph.VertexCount - 1)];
                        for (int j = 1; j < cats.Length + 1; j++)
                            if (f.HasEdge(j, i) == true)
                            {
                                res2[i - cats.Length - 1][count2] = j - 1;
                                count2++;
                            }
                    }
                }
            }
            return (res1, res2, res3);
        }
    }
}
