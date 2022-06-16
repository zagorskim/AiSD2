using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD_lab14
{

    public class Substrings : MarshalByRefObject
    {
        /// <summary>
        /// Zadanie pierwsze, w którym musimy znaleźć najdłuższy fragment tekstu powtarzający się przynajmniej dwukrotnie
        /// </summary>
        /// <param name="text">Pierwszy string</param>
        /// <returns>
        /// length: długość najdłuższego fragmentu powtarzającego się przynajmniej 2 razy <br />
        /// longestCommonSubstring: najdłuższy fragment powtarzający się przynajmniej 2 razy
        /// </returns>
        //public (int length, string longestSubstring) StageOne(string text)
        //{
        //    int maxlen = 0, index = -1;
        //    string res = "";

        //    for (int i = 0; i < text.Length; i++)
        //    {
        //        string temp = text.Substring(i, text.Length - i);
        //        var p = makeP(temp);

        //        for (int j = 0; j <= temp.Length; j++)
        //            if (p[j] > maxlen)
        //            {
        //                if (p[j] > j / 2)
        //                {
        //                    if (j / 2 > maxlen)
        //                    {
        //                        index = i;
        //                        maxlen = (p[j] + 1) / 2;
        //                    }
        //                }
        //                else
        //                {
        //                    index = i;
        //                    maxlen = p[j];
        //                }
        //            }
        //    }
        //    if (index > -1)
        //        res = text.Substring(index, maxlen);

        //    return (maxlen, res);
        //}
        public (int length, string longestSubstring) StageOne(string text)
        {
            int maxRet = -1;
            string maxStr = "";

            for (int start = 0; start < text.Length; start++)
            {
                string s = text.Substring(start);

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

                for (int i = 0; i < P.Length; i++)
                {
                    if (P[i] > maxRet)
                    {
                        if (P[i] > i / 2)
                        {
                            if (i / 2 > maxRet)
                            {
                                maxRet = (P[i] + 1) /2;
                                maxStr = s.Substring(0, maxRet);
                            }
                        }
                        else
                        {
                            maxRet = P[i];
                            maxStr = s.Substring(0, maxRet);
                        }
                    }
                }
            }

            return (maxRet, maxStr);
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
        /// <summary>
        /// Zadanie drugie, w którym musimy znaleźć dwa najdłuższe powtarzające się fragmenty w dwóch stringach
        /// </summary>
        /// <param name="x">Pierwszy string</param>
        /// <param name="y">Drugi string</param>
        /// <returns>
        /// length: długość najdłuższego wspólnego fragmentu <br />
        /// longestCommonSubstring: najdłuższy wspólny fragment
        /// </returns>
        public (int length, string longestCommonSubstring) StageTwo(string x, string y)
        {
            string res = "";
            int maxlen = 0, index = 0;
            string text = x + y;
            int[,] tab = new int[x.Length + 1, y.Length + 1];
            if (x.Equals(y))
                return (x.Length, x);
            for (int i = 0; i < x.Length; i++)
                tab[i, 0] = 0;
            for (int i = 1; i < y.Length; i++)
                tab[0, i] = 0;

            for (int i = 1; i < x.Length + 1; i++)
                for(int j = 1; j < y.Length + 1; j++)
                {
                    if (x[i - 1] == y[j - 1])
                        tab[i, j] = tab[i - 1, j - 1] + 1;
                    else
                        tab[i, j] = 0;
                }

            for (int i = 1; i < x.Length + 1; i++)
                for (int j = 1; j < y.Length + 1; j++)
                {
                    if (tab[i, j] > maxlen)
                    {
                        index = i - 1;
                        maxlen = tab[i, j];
                    }
                }

            res = x.Substring(index - maxlen + 1, maxlen);

            return (maxlen, res);
        }
    }


            //for (int i = 0; i < x.Length; i++)
            //{
            //    for (int j = x.Length; j < text.Length; j++)
            //    {
            //        int[] p = makeP(text.Substring(i, j - i + 1));
            //        //if (maxlen == i + 1)
            //        //    break;
            //        //if (maxlen == j - x.Length)
            //        //    continue;
            //        if (p[j - i + 1] > maxlen && p[j - i + 1] <= x.Length - i && p[j - i + 1] <= j - x.Length + 1) //przerywać jak już dla danego j znajdę pasujący o długości j - x.Length
            //        {
            //            index = j;
            //            maxlen = p[j - i + 1];
            //        }
            //    }
            //    //if (maxlen == x.Length - i)
            //    //    break;
            //}
            //if (maxlen > 0)
            //    res = text.Substring(index - maxlen + 1, maxlen);
}
