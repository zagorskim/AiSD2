using System;
using System.Linq;
using System.Collections.Generic;
using ASD.Graphs;

namespace Lab10
{
    public class DeliveryPlanner : MarshalByRefObject
    {

        /// <param name="railway">Graf reprezentujący sieć kolejową</param>
        /// <param name="eggDemand">Zapotrzebowanie na jajka na poszczególnyhc stacjach. Zerowy element tej tablicy zawsze jest 0</param>
        /// <param name="truckCapacity">Pojemność wagonu na jajka</param>
        /// <param name="tankEngineRange">Zasięg parowozu</param>
        /// <param name="isRefuelStation">na danym indeksie true, jeśli na danej stacji można uzupelnić węgiel i wodę</param>
        /// <param name="anySolution">Czy znaleźć jakiekolwiek rozwiązanie (true, etap 1), czy najkrótsze (false, etap 2)</param>
        /// <returns>Informację czy istnieje trasa oraz tablicę reprezentującą kolejne wierzchołki w trasie (pierwszy i ostatni element tablicy musi być 0). W przypadku, gdy zwracany jest false, wartość tego pola nie jest sprawdzana, może być null.</returns>
        public (bool routeExists, int[] route) PlanDelivery(Graph<int> railway, int[] eggDemand, int truckCapacity, int tankEngineRange, bool[] isRefuelStation, bool anySolution)
        {
            //int[] a = { 5, 14, 24, 29};
            //Console.WriteLine();
            //for (int i = 0; i < railway.VertexCount; i++)
            //{
            //    if(a.Contains(i))
            //        Console.ForegroundColor = System.ConsoleColor.Red;
            //    else
            //        Console.ForegroundColor = System.ConsoleColor.Blue;
            //    Console.Write("{0}", i);
            //    Console.ForegroundColor = System.ConsoleColor.White;
            //    foreach (var v in railway.OutEdges(i))
            //        Console.Write(" {0}", v.To);
            //    Console.WriteLine();
            //}

            var dp = new DeliveryPath(railway, eggDemand, truckCapacity, tankEngineRange, isRefuelStation, anySolution);
            dp.FindPath();
            if (dp.resExists)
                return (true, dp.resPath.ToArray());
            return (false, null);
        }
        public class DeliveryPath
        {
            public Graph<int> railway;
            public int[] eggDemand;
            public int truckCapacity;
            public int tankEngineRange;
            public bool[] isRefuelStation;
            public bool anySolution;

            public List<int> currPath;
            public bool[] currRefuels;
            public int currEggsDelivered;
            public int currEggsOnboard;
            public int currRange;
            public Stack<int> currSold;
            public Stack<int> currDist;

            public List<int> resPath;
            public bool[] resRefuels;
            public int resEggsDelivered;
            public bool resExists;

            public int bestRange = 0;
            public int bestCount;
            public DeliveryPath(Graph<int> railway, int[] eggDemand, int truckCapacity, int tankEngineRange, bool[] isRefuelStation, bool anysolution)
            {
                this.railway = railway;
                this.eggDemand = eggDemand;
                this.truckCapacity = truckCapacity;
                this.tankEngineRange = tankEngineRange;
                this.isRefuelStation = isRefuelStation;
                this.anySolution = anysolution;
            }
            public bool FindPath()
            {
                currPath = new List<int>();
                currRefuels = new bool[railway.VertexCount];
                currRange = tankEngineRange;
                currEggsDelivered = 0;
                currEggsOnboard = truckCapacity;
                currSold = new Stack<int>();
                currDist = new Stack<int>();

                resPath = new List<int>();
                resRefuels = new bool[railway.VertexCount];
                resExists = false;
                resEggsDelivered = 0;

                DeliverEggs(0);

                return resExists;
            }
            public void DeliverEggs(int place)
            {
                if (anySolution && resExists) return;
                if (place == 0)
                {
                    currEggsOnboard = truckCapacity;
                    int count = 0, dist = 0, prev = 0;
                    foreach (var i in currPath)
                    {
                        if(count != 0) dist += railway.GetEdgeWeight(prev, i);
                        if (i == 0) count++;
                        prev = i;
                    }
                    if (count != 0) dist += railway.GetEdgeWeight(prev, 0);
                    if (currPath.Count - count >= railway.VertexCount - 1 && (bestRange > dist || resExists == false))
                    {
                        bestCount = currPath.Count;
                        bestRange = dist;
                        resExists = true;
                        CurrToRes();
                    }
                }
                else
                    currEggsOnboard -= eggDemand[place];
                currSold.Push(currEggsOnboard);

                currPath.Add(place);
                int index = currPath.Count - 1;

                if(isRefuelStation[place])
                {
                    currRange = tankEngineRange;
                }
                currDist.Push(currRange);

                ContinueTravel(place);

                currSold.Pop();
                currDist.Pop();
                currPath.RemoveAt(index);
            }
            public void ContinueTravel(int place)
            {
                foreach(var e in railway.OutEdges(place))
                {
                    if (currPath.Count > bestCount && bestCount != 0 || currDist.Peek() == 0)
                        break;
                    if (currPath.Contains(e.To) && e.To != 0)
                        continue;
                    if (currDist.Peek() >= e.weight && eggDemand[e.To] <= currSold.Peek())
                    {
                        currEggsOnboard = currSold.Peek();
                        currRange = currDist.Peek();
                        currRange -= e.weight;
                        DeliverEggs(e.To);
                    }
                }
            }
            public void CurrToRes()
            {
                resPath = new List<int>(currPath);
                for (int i = 0; i < isRefuelStation.Length; i++)
                    resRefuels[i] = currRefuels[i];
                resPath.Add(0);
            }
        }
    }
}
