using System;
using System.Text;

namespace Lab15
{
    public static class stringExtender
    {
        /// <summary>
        /// Metoda zwraca okres słowa s, tzn. najmniejszą dodatnią liczbę p taką, że s[i]=s[i+p] dla każdego i od 0 do |s|-p-1.
        /// 
        /// Metoda musi działać w czasie O(|s|)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static public int Period(this string s)
        {
            int max = 0;
            var P1 = makeP(s);
            foreach (var i in P1)
                Console.Write($"{i} ");
            Console.WriteLine();
            for (int i = 0; i < s.Length; i++)
                Console.Write($"{P1[i]} ");
            Console.WriteLine();
            var P = new PTable(s);
            return s.Length - P1[s.Length];
        }
        static int[] makeP(string s)
        {
            int[] P = new int[s.Length + 1];
            int t = 0;

            for (int i = 2; i <= s.Length; i++)
            {
                while (s[t] != s[i - 1] && t > 0)
                    t = P[t];
                P[i] = t;
                if (s[i - 1] == s[t])
                    P[i] = ++t;
            }
            return P;
        }
        class PTable
        {
            // kompresowany napis
            string s;

            // indeks w napisie s od którego zaczyna się wzorzec
            int patternStart;

            // tablica przechowująca już obliczone wartości z tablicy P
            int[] internalTable;

            // liczba już obliczonych wartości z tablicy P
            int internalTableLen;

            public PTable(string s)
            {
                this.s = s;
                patternStart = internalTableLen = 0;
                internalTable = new int[Math.Max(s.Length, 2)];
                internalTableLen = 2;
                internalTable[0] = internalTable[1] = 0;
            }

            public int this[int i]
            {
                get
                {
                    if (i >= internalTableLen)
                    {
                        // Po prostu liczenie tablicy P jak w algorytmie KMP
                        for (int j = internalTableLen; j <= i; j++)
                        {
                            int t = internalTable[j - 1]; //TODO: test, który zepsuje jeżeli tu będzie j=ii-1
                            while (t > 0 && s[patternStart + j - 1] != s[patternStart + t])
                                t = internalTable[t];
                            if (s[patternStart + j - 1] == s[patternStart + t])
                                t++;
                            internalTable[j] = t;
                            internalTableLen++;
                        }
                    }
                    return internalTable[i];
                }
            }

            // Po wywołaniu tej metody tablica P będzie liczona dla wzorca zaczynającego się pod indeksem newStart w napisie s
            // Oczywiście trzeba zapomnieć wszystkie wcześniej wyliczone wartości poza pierwszymi dwoma (bo i tak są 0)
            public void SetNewStart(int newStart)
            {
                internalTableLen = 2;
                patternStart = newStart;
            }
        }
        /// <summary>
        /// Metoda wyznacza największą potęgę zawartą w słowie s.
        /// 
        /// Jeżeli x jest słowem, wówczas przez k-tą potęgę słowa x rozumiemy k-krotne powtórzenie słowa x
        /// (na przykład xyzxyzxyz to trzecia potęga słowa xyz).
        /// 
        /// Należy zwrócić największe k takie, że k-ta potęga jakiegoś słowa jest zawarta w s jako spójny podciąg.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startIndex">Pierwszy indeks fragmentu zawierającego znalezioną potęgę</param>
        /// <param name="endIndex">Pierwszy indeks po fragmencie zawierającym znalezioną potęgę</param>
        /// <returns></returns>
        static public int MaxPower(this string s, out int startIndex, out int endIndex)
        {
                startIndex = endIndex = -1;
                return -1;
        }
    }
}
