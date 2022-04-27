
using System;

namespace ASD
{

    class ChangeMaking
    {

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// bez ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to change = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości amount ( czyli rzędu o(amount) )
        /// </remarks>
        public int? NoLimitsDynamic(int amount, int[] coins, out int[] change)
        {
            int i, c, a;
            int n = coins.Length;
            int[,] t = new int[2, amount + 1];
            // j-ta kolumna opisuje wydanie reszty dla kwoty j (rozwiazaniem jest ostatnia kolumna)
            // w wierszu 0 pamiętamy ile monet uzyto do wydania kwoty j
            // w wierszu 1 pamiętamy ostatnio dodana monete
            t[0, 0] = 0;
            t[1, 0] = -1;  // nie dodaliśmy żadnej monety

            for (a = 1; a <= amount; ++a)
            {
                t[0, a] = int.MaxValue;  // nie umiemy wydać tej kwoty
                t[1, a] = -1;            // nie dodaliśmy żadnej monety
                for (i = 0; i < n; ++i)
                {
                    c = a - coins[i];
                    // przerywamy iteracje gdy zeszlismy z kwota ponizej 0 lub dotychczasowe rozw. jest lepsze
                    if (c < 0 || t[0, c] >= t[0, a]) continue;
                    t[0, a] = t[0, c] + 1;
                    t[1, a] = i;
                }
            }

            if (t[0, amount] == int.MaxValue)
            {
                change = null;
                return null;
            }

            change = new int[n];
            for (i = 0, c = amount; c > 0; ++i, c -= coins[t[1, c]])
                ++change[t[1, c]];
            return i;
        }

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// z uwzględnieniem ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="limits">Liczba dostępnych monet danego nomimału</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// limits[i] - dostepna liczba monet i-tego rodzaju (nominału)
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to change = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości iloczynu amount*(liczba rodzajów monet)
        /// ( czyli rzędu o(amount*(liczba rodzajów monet)) )
        /// </remarks>
        public int? Dynamic(int amount, int[] coins, int[] limits, out int[] change)
        {
            int k, i, c, a;
            int n = coins.Length;
            int[,] total = new int[n, amount + 1];
            int[,] last = new int[n, amount + 1];
            // w obu tablicach pomocniczych i-ty wiersz opisuje wydawanie reszty jedynie za pomocą monet o numerach 0..i
            // w total[i,j] pamietamy ogólną minimalną liczbę monet (o numerach 0..i) potrzeną od wydania kwoty j (int.MaxValue - jeśli się nie da)
            // w last[i,j] pamietamy ile monet rodzaju i jest uzyte w minimalnym rozwiazaniu

            // inicjalizacja wstępna - początkowo zadnej kwoty > 0 nie umiemy wydac
            for (a = 1; a <= amount; ++a)
                total[0, a] = int.MaxValue;

            // moneta nr 0 - umiemy wydać tylko wiekokrotnosci nominalu (i to jedynie do limitu uzycia)
            c = coins[0];
            for (k = 1; k <= limits[0] && k * c <= amount; ++k)
                total[0, k * c] = last[0, k * c] = k;

            // kolejne monety
            for (i = 1; i < n; ++i)
            {
                // inicjalizacja - kopiujemy rozw. nie korzystajace z i-tej monety
                for (a = 1; a <= amount; ++a)
                    total[i, a] = total[i - 1, a];

                c = coins[i];
                // dla kolejnych roznacych kwot
                for (a = 0; a <= amount; ++a)
                    // jesli kwote umiemy wydac bez i-tej monety
                    if (total[i - 1, a] != int.MaxValue)
                        // rozbudowujemy rozw. o wielokrotne (do limitu) uzycie i-tej monety
                        for (k = 1; k <= limits[i] && a + k * c <= amount; ++k)
                            // jesli jest lepiej - poprawiamy
                            if (total[i, a + k * c] > total[i - 1, a] + k)
                            {
                                total[i, a + k * c] = total[i - 1, a] + k;
                                last[i, a + k * c] = k;
                            }
            }

            // nie ma rozwiazania :(
            if (total[n - 1, amount] == int.MaxValue)
            {
                change = null;
                return null;
            }

            // odtwarzamy rozw. z tablicy last
            change = new int[n];
            c = 0;
            for (a = amount, i = n - 1; i >= 0; a -= coins[i] * change[i], --i)
            {
                change[i] = last[i, a];
                c += change[i];
            }
            return c;
        }

    }

}


