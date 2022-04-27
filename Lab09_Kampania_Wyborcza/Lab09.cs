using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public class Lab08 : MarshalByRefObject
    {
        /// <summary>
        /// Znajduje cykl rozpoczynający się w stolicy, który dla wybranych miast,
        /// przez które przechodzi ma największą sumę liczby ludności w tych wybranych
        /// miastach oraz minimalny koszt.
        /// </summary>
        /// <param name="cities">
        /// Graf miast i połączeń między nimi.
        /// Waga krawędzi jest kosztem przejechania między dwoma miastami.
        /// Koszty transportu między miastami są nieujemne.
        /// </param>
        /// <param name="citiesPopulation">Liczba ludności miast</param>
        /// <param name="meetingCosts">
        /// Koszt spotkania w każdym z miast.
        /// Dla części pierwszej koszt spotkania dla każdego miasta wynosi 0.
        /// Dla części drugiej koszty są nieujemne.
        /// </param>
        /// <param name="budget">Budżet do wykorzystania przez kandydata.</param>
        /// <param name="capitalCity">Numer miasta będącego stolicą, z której startuje kandydat.</param>
        /// <param name="path">
        /// Tablica dwuelementowych krotek opisująca ciąg miast, które powinen odwiedzić kandydat.
        /// Pierwszy element krotki to numer miasta do odwiedzenia, a drugi element decyduje czy
        /// w danym mieście będzie organizowane spotkanie wyborcze.
        /// 
        /// Pierwszym miastem na tej liście zawsze będzie stolica (w której można, ale nie trzeba
        /// organizować spotkania).
        /// 
        /// Zakładamy, że po odwiedzeniu ostatniego miasta na liście kandydat wraca do stolicy
        /// (na co musi mu starczyć budżetu i połączenie między tymi miastami musi istnieć).
        /// 
        /// Jeżeli kandydat nie wyjeżdża ze stolicy (stolica jest jedynym miastem, które odwiedzi),
        /// to lista `path` powinna zawierać jedynie jeden element: stolicę (wraz z informacją
        /// czy będzie tam spotkanie czy nie). Nie są wtedy ponoszone żadne koszty podróży.
        /// 
        /// W pierwszym etapie drugi element krotki powinien być zawsze równy `true`.
        /// </param>
        /// <returns>
        /// Liczba mieszkańców, z którymi spotka się kandydat.
        /// </returns>
        public int ComputeElectionCampaignPath(Graph<int> cities, int[] citiesPopulation,
            double[] meetingCosts, double budget, int capitalCity, out (int, bool)[] path)
        {
            var s = new ElectionPathSolver(cities, citiesPopulation, meetingCosts, budget, capitalCity);
            var(outCities, outMeetings, outPeopleMet) = s.Solve();
            path = outCities.Select(x => (x, outMeetings[x])).ToArray();
            return outPeopleMet;
        }
        public class ElectionPathSolver
        {
            private Graph<int> cities;
            private double budget;
            private int capitalCity;
            private int[] citiesPopulation;
            private double[] meetingCosts;

            private List<int> currSolutionCities;
            private bool[] currSolutionMeetings;
            private int currPeopleMet;
            private double currBudget;
            private HashSet<int> currVisitedCities;

            private List<int> resSolutionCities;
            private bool[] resSolutionMeetings;
            private int resPeopleMet;
            private double resBudget;

            public ElectionPathSolver(Graph<int> cities, int[] citiesPopulation,
            double[] meetingCosts, double budget, int capitalCity)
            {
                this.cities = cities;
                this.citiesPopulation = citiesPopulation;
                this.capitalCity = capitalCity;
                this.meetingCosts = meetingCosts;
                this.budget = budget;
            }
            public (int[] outCities, bool[] outMeetings, int outPeopleMet) Solve()
            {
                resBudget = budget;
                resSolutionMeetings = new bool[cities.VertexCount];
                resSolutionCities = new List<int>(cities.VertexCount);
                resPeopleMet = 0;
                resSolutionCities.Add(capitalCity);

                currBudget = budget;
                currSolutionCities = new List<int>(cities.VertexCount);
                currSolutionMeetings = new bool[cities.VertexCount];
                currVisitedCities = new HashSet<int>();
                currPeopleMet = 0;

                VisitCity(capitalCity);

                return (resSolutionCities.ToArray(), resSolutionMeetings, resPeopleMet);
            }
            private void VisitCity(int city)
            {
                if(city == capitalCity && currVisitedCities.Count > 1)
                {
                    if (IsBetter())
                        CurrToRes();
                    return;
                }
                currVisitedCities.Add(city);
                currSolutionCities.Add(city);

                if(currBudget >= meetingCosts[city])
                {
                    currBudget -= meetingCosts[city];
                    currPeopleMet += citiesPopulation[city];
                    currSolutionMeetings[city] = true;

                    TravelFrom(city);

                    currBudget += meetingCosts[city];
                    currPeopleMet -= citiesPopulation[city];
                    currSolutionMeetings[city] = false;
                }
                if (meetingCosts[city] > 0)
                    TravelFrom(city);

                currSolutionCities.Remove(city);
                currVisitedCities.Remove(city);
            }

            private void TravelFrom(int city)
            {
                if (IsCapital(city) && currVisitedCities.Count == 1 && IsBetter())
                    CurrToRes();

                foreach(var e in cities.OutEdges(city))
                {
                    if (currSolutionCities.Contains(e.To) && !IsCapital(e.To))
                        continue;

                    if (currBudget < e.weight)
                        continue;

                    currBudget -= e.weight;

                    VisitCity(e.To);

                    currBudget += e.weight;
                }
            }
            public void CurrToRes()
            {
                resSolutionCities = new List<int>(currSolutionCities);
                resPeopleMet = currPeopleMet;
                resBudget = currBudget;
                for(int i = 0; i < cities.VertexCount; i++)
                    resSolutionMeetings[i] = currSolutionMeetings[i];
            }
            public bool IsBetter()
            {
                if (currPeopleMet > resPeopleMet) return true;
                if (currPeopleMet >= resPeopleMet && currBudget > resBudget) return true;
                return false;
            }
            public bool IsCapital(int city)
            {
                return city == capitalCity;
            }
        }
    }
}
