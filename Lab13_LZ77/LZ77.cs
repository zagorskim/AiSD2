using System;
using System.Collections.Generic;

namespace ASD
{
    public class LZ77 : MarshalByRefObject
    {
        /// <summary>
        /// Odkodowywanie napisu zakodowanego algorytmem LZ77. Dane kodowanie jest poprawne (nie trzeba tego sprawdzać).
        /// </summary>
        public string Decode(List<EncodingTriple> encoding)
        {
            if (encoding == null)
                return null;
            int count = 0;
            foreach (var i in encoding)
                count += i.c + 1;
            var res = new char[count];
            var sb = new System.Text.StringBuilder();
            int pos = 0;
            foreach (var i in encoding)
            {
                var l = pos - 1;
                for(int j = 0; j < i.c; j++)
                {
                    res[pos] = res[l - i.p + j];
                    pos++;
                    //sb.Append(sb[l - 1 - i.p + j]);
                }
                res[pos++] = i.s;
                //sb.Append(i.s);
            }
            return new string(res);
        }

        /// <summary>
        /// Kodowanie napisu s algorytmem LZ77
        /// </summary>
        /// <returns></returns>
        public List<EncodingTriple> Encode(string s, int maxP)
        {
            return null;
        }
    }

    [Serializable]
    public struct EncodingTriple
    {
        public int p, c;
        public char s;

        public EncodingTriple(int p, int c, char s)
        {
            this.p = p;
            this.c = c;
            this.s = s;
        }
    }
}
