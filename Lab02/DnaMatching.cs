using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Lab2
{
    public class DnaMatching : MarshalByRefObject
    {
        /// <summary>
        ///   Wariant I z prostym systemem oceny jakości dopasowania dwóch sekwencji DNA
        /// </summary>
        /// <param name="seq1"> pierwsza niepusta sekwencja DNA złożona ze znaków 'A', 'C', 'G', 'T'</param>
        /// <param name="seq2"> druga niepusta sekwencja DNA złożona ze znaków 'A', 'C', 'G', 'T'</param>
        /// <returns>(dopasowanie [ciąg 1], dopasowanie [ciąg 2], wartość całego dopasowania). 
        ///  w pierwszym etapie można zwracać nulle zamiast ciągów dopasowania </returns>
        public (string matchingSeq1, string matchingSeq2, int bestMatchingValue) FindMatchingV1(string seq1, string seq2)
        {
            const int matchValue = 1;
            const int mismatchValue = -3;
            const int gapValue = -2;
            var tab1 = new int[seq1.Length + 1, seq2.Length + 1];
            var tab2 = new (char, char)[seq1.Length + 1, seq2.Length + 1];

            for (int i = 0; i <= seq1.Length; i++)
            {
                tab1[i, 0] = gapValue * i;
                tab2[i, 0].Item1 = '-';
                tab2[i, 0].Item2 = '-';
            }
            for (int i = 1; i <= seq2.Length; i++)
            {
                tab1[0, i] = gapValue * i;
                tab2[0, i].Item1 = '-';
                tab2[0, i].Item2 = '-';
            }

            for (int i = 1; i <= seq1.Length; i++)
            {
                for (int j = 1; j <= seq2.Length; j++)
                {
                    tab1[i, j] = Math.Max(tab1[i - 1, j - 1] + (seq1[i - 1] == seq2[j - 1] ? matchValue : mismatchValue),
                        Math.Max(tab1[i, j - 1] + gapValue, tab1[i - 1, j] + gapValue));
                    if(seq1[i - 1] == seq2[j - 1] || tab1[i - 1, j - 1] + mismatchValue >= Math.Max(tab1[i, j - 1] + gapValue, tab1[i - 1, j] + gapValue))
                    {
                        tab2[i, j].Item1 = seq1[i - 1];
                        tab2[i, j].Item2 = seq2[j - 1];
                    }
                    else if(tab1[i, j - 1] + gapValue >= tab1[i - 1, j] + gapValue)
                    {
                        tab2[i, j].Item1 = '-';
                        tab2[i, j].Item2 = seq2[j - 1];
                    }
                    else
                    {
                        tab2[i, j].Item1 = seq1[i - 1];
                        tab2[i, j].Item2 = '-';
                    }
                }
            }

            StringBuilder res1 = new StringBuilder("");
            StringBuilder res2 = new StringBuilder("");
            int a = seq1.Length;
            int b = seq2.Length;
            while(a-1 >= 0 && b-1 >= 0)
            {
                res1.Append(tab2[a, b].Item1);
                res2.Append(tab2[a, b].Item2);
                int up = tab1[a - 1, b];
                int left = tab1[a, b - 1];
                int diag = tab1[a - 1, b - 1];
                if (diag >= up && diag >= left)
                {
                    a--;
                    b--;
                }
                else if (up >= left) a--;
                else b--;
            }
            while (a - 1 >= 0)
            {
                res1.Append(seq1[a - 1]);
                res2.Append("-");
                a--;
            }
            while (b - 1 >= 0)
            {
                res1.Append("-");
                res2.Append(seq2[b - 1]);
                b--;
            }
            char[] r1 = res1.ToString().ToCharArray();
            char[] r2 = res2.ToString().ToCharArray();
            Array.Reverse(r1);
            Array.Reverse(r2);
            string re1 = new string(r1);
            string re2 = new string(r2);
            return (re1, re2, tab1[seq1.Length, seq2.Length]);
        }


        /// <summary>
        ///   Wariant II z zaawansowanym systemem oceny jakości dopasowania dwóch sekwencji DNA
        /// </summary>
        /// <param name="seq1"> pierwsza niepusta sekwencja DNA złożona ze znaków 'A', 'C', 'G', 'T'</param>
        /// <param name="seq2"> druga niepusta sekwencja DNA złożona ze znaków 'A', 'C', 'G', 'T'</param>
        /// <returns>(dopasowanie [ciąg 1], dopasowanie [ciąg 2], wartość całego dopasowania). 
        ///  w trzecim etapie można zwracać nulle zamiast ciągów dopasowania </returns>
        public (string matchingSeq1, string matchingSeq2, int bestMatchingValue) FindMatchingV2(string seq1, string seq2)
        {
            const int matchValue = 1;
            const int mismatchValue = -3;
            const int gapStartValue = -5;
            const int gapContinuationValue = -2;

            // Metoda do zaimplementowania w etapach 3 i 4

            return (null, null, 0);
        }
    }
}