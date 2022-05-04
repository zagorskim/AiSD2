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
            public int maxRange;
            public int maxDist;
            public int maxEggs;

            public List<int> currPath;
            public int currEggsOnboard;
            public int currRange;
            public Stack<int> currSold;
            public Stack<int> currDist;
            public int currRangeLost;
            public int currEggsLost;

            public List<int> resPath;
            public bool resExists;

            public int bestRange = 0;
            public int bestCount;
            int distance = 0;
            int count;
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
                int temp = tankEngineRange;
                foreach (var i in isRefuelStation)
                    if (i == true)
                        temp += tankEngineRange;
                maxRange = temp;
                temp = 0;
                foreach (var i in railway.DFS().SearchAll())
                        temp += i.weight;
                maxRange = temp;
                temp = 0;
                foreach (var i in railway.OutNeighbors(0))
                    temp += truckCapacity;
                maxEggs = temp;

                currPath = new List<int>();
                currRange = tankEngineRange;
                currEggsOnboard = truckCapacity;
                currSold = new Stack<int>();
                currDist = new Stack<int>();
                currRangeLost = 0;

                resPath = new List<int>();
                resExists = false;

                DeliverEggs(0);

                return resExists;
            }
            public void DeliverEggs(int place)
            {
                if (anySolution && resExists)
                    return;
                int added1 = 0;
                if (place == 0)
                {
                    added1 = truckCapacity - currEggsOnboard;
                    currEggsLost += added1;
                    currEggsOnboard = truckCapacity;
                    int dist = 0, prev = 0;
                    count = 0;
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

                int added2 = 0;
                if(isRefuelStation[place])
                {
                    added2 = tankEngineRange - currRange;
                    currRangeLost += tankEngineRange - currRange;
                    currRange = tankEngineRange;
                }
                currDist.Push(currRange);

                ContinueTravel(place);

                currEggsLost -= added1;
                currRangeLost -= added2;
                currSold.Pop();
                currDist.Pop();
                currPath.RemoveAt(currPath.Count - 1);
            }
            public void ContinueTravel(int place)
            {
                //if (maxEggs - currEggsLost < railway.VertexCount - (currPath.Count - count))
                //    return;
                if (bestRange < distance && bestCount != 0)
                    return;
                foreach(var e in railway.OutEdges(place))
                {
                    if (currPath.Contains(e.To) && e.To != 0)
                        continue;
                    if (currDist.Peek() >= e.weight && eggDemand[e.To] <= currSold.Peek())
                    {
                        currEggsOnboard = currSold.Peek();
                        currRange = currDist.Peek();
                        currRange -= e.weight;
                        distance += e.weight;
                        DeliverEggs(e.To);
                        distance -= e.weight;
                    }
                }
            }
            public void CurrToRes()
            {
                resPath = new List<int>(currPath);
                resPath.Add(0);
            }
        }
    }
}
