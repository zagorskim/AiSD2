using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    public class Lab06 : System.MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 i 2 - szukanie trasy w nieplynacej rzece
        /// </summary>
        /// <param name="w"> Odległość między brzegami rzeki</param>
        /// <param name="l"> Długość fragmentu rzeki </param>
        /// <param name="lilie"> Opis lilii na rzece </param>
        /// <param name="sila"> Siła żabki - maksymalny kwadrat długości jednego skoku </param>
        /// <param name="start"> Początkowa pozycja w metrach od lewej strony </param>
        /// <returns> (int total, (int x, int y)[] route) - total - suma sił koniecznych do wszystkich skoków, route -
        /// lista par opisujących skoki. Opis jednego skoku (x,y) to dystans w osi x i dystans w osi y, jaki skok pokonuje</returns>
        public (int total, (int, int)[] route) Lab06_FindRoute(int w, int l, int[,] lilie, int sila, int start)
        {
	    // uzupełnić

            return (0, null);
        }

        /// <summary>
        /// Etap 3 i 4 - szukanie trasy w nieplynacej rzece
        /// </summary>
        /// <param name="w"> Odległość między brzegami rzeki</param>
        /// <param name="l"> Długość fragmentu rzeki </param>
        /// <param name="lilie"> Opis lilii na rzece </param>
        /// <param name="sila"> Siła żabki - maksymalny kwadrat długości jednego skoku </param>
        /// <param name="start"> Początkowa pozycja w metrach od lewej strony </param>
        /// <param name="max_skok"> Maksymalna ilość skoków </param>
        /// <param name="v"> Prędkość rzeki </param>
        /// <returns> (int total, (int x, int y)[] route) - total - suma sił koniecznych do wszystkich skoków, route -
        /// lista par opisujących skoki. Opis jednego skoku (x,y) to dystans w osi x i dystans w osi y, jaki skok pokonuje</returns>
        public (int total, (int, int)[] route) Lab06_FindRouteFlowing(int w, int l, int[,] lilie, int sila, int start, int max_skok, int v)
        {
	    // uzupełnić
            return (0, null);

        }
    }
}
