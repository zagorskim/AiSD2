using System;
using System.Linq;
using System.Collections.Generic;

namespace ASD
{
    public class SheepHeard : MarshalByRefObject
    {
        /// <param name="dogs">Tablica opisującapozycje psow</param>
        /// <param name="shed">Pozycja sciany budynku gospodarczego</param>
        /// <returns>Wierzchołki wielokatku w którym owce są bezpieczne</returns>
        public (double x, double y)[] SafePolygon((double x, double y)[] dogs, (float A, float B, float C) shed)
        {
            if (dogs.Length < 2)
                return null;
            var list = new List<(double, double)>(dogs);
            list.Sort();
            var help = new List<(double, double)>();
            var s = new Solver(shed);
            foreach(var i in list)
            {
                help.Add(s.OrtogonalProjection(i));
            }
            help.AddRange(dogs);
            help.Sort();
            var lower = new List<(double, double)>();
            foreach(var i in help)
            {
                while (lower.Count >= 2 && s.Cross(lower[lower.Count - 2], lower[lower.Count - 1], i) <= 0)
                    lower.RemoveAt(lower.Count - 1);
                lower.Add(i);
            }
            help.Reverse();
            var upper = new List<(double, double)>();
            foreach(var i in help)
            {
                while (upper.Count >= 2 && s.Cross(upper[upper.Count - 2], upper[upper.Count - 1], i) <= 0)
                    upper.RemoveAt(upper.Count - 1);
                upper.Add(i);
            }
            upper.RemoveAt(upper.Count - 1);
            lower.RemoveAt(lower.Count - 1);
            lower.AddRange(upper);
            return lower.ToArray();
        }

        /// <param name="sheeps">Tablica opisującapozycje owiec</param>
        /// <param name="dogs">Tablica opisującapozycje psow</param>
        /// <param name="shed">Pozycja sciany budynku gospodarczego</param>
        /// <returns>Liczba bezpiecznych owiec</returns>
        public int CheckCoverage((double x, double y)[] sheeps, (double x, double y)[] dogs, (float A, float B, float C) shed)
        {
            var s = new Solver(shed);
            var sp = SafePolygon(dogs, shed);
            if (sp == null)
                return 0;
            int curr = sp.Length / 2;
            int prev1 = curr;
            int prev2 = prev1;
            int count = 0;
            int flag;
            int prevflag = 1;
            foreach (var i in sheeps)
            {
                flag = 0;
                prevflag = 1;
                curr = sp.Length / 2;
                prev1 = curr;
                prev2 = prev1;
                do
                {
                    if (s.Cross(sp[0], sp[curr], i) > 0)
                    {
                        prevflag = flag;
                        flag = 1;
                        //if (prevflag == -1)
                        //{
                        //    prev2 = prev1;
                        //    prev1 = curr;
                        //    curr++;
                        //    break;
                        //}
                        //else
                        //{
                            prev2 = prev1;
                            prev1 = curr;
                            curr = (curr + sp.Length) / 2;
                        //}
                    }
                    else if (s.Cross(sp[0], sp[curr], i) < 0)
                    {
                        prevflag = flag;
                        flag = -1;
                        //if (prevflag == -1)
                        //{
                        //    prev2 = prev1;
                        //    prev1 = curr;
                        //    curr++;
                        //    break;
                        //}
                        //else
                        //{
                            prev2 = prev1;
                            prev1 = curr;
                            curr = curr / 2;
                        //}
                    }
                    else
                    {
                        flag = 0;
                        if (s.Distance(sp[curr], i) <= s.Distance(sp[0], sp[curr]) && s.Distance(sp[0], i) <= s.Distance(sp[0], sp[curr]))
                            count++;
                        break;
                    }
                }
                while (curr != prev1 && flag + prevflag != 0);
                if (flag == prevflag)
                {
                    if(flag == 1)
                    {
                        if (s.Cross(sp[prev1], sp[0], i) >= 0)
                            count++;
                    }
                    else if (flag == -1)
                    {
                        if (s.Cross(sp[prev1], sp[sp.Length - 1], i) <= 0)
                            count++;
                    }
                }
                else if (flag == 1)
                {
                    if (s.Cross(sp[prev2], sp[prev1], i) <= 0)
                        count++;
                }
                else if (flag == -1)
                {
                    if (s.Cross(sp[prev2], sp[prev1], i) >= 0)
                        count++;
                }
            }
            return count;
        }
    }
    public class Solver
    {
        public (float A, float B, float C) eq;
        public Solver((float A, float B, float C) e)
        {
            eq = e;
        }
        public (double, double) OrtogonalProjection((double, double) point)
        {
            if (eq.B == 0)
                return (-eq.C / eq.A, point.Item2);
            if (eq.A == 0)
                return (point.Item1, -eq.C / eq.B);
            return ((-1 *eq.A * eq.B * point.Item2 + Math.Pow(eq.B, 2) * point.Item1 - eq.A * eq.C) / (Math.Pow(eq.A, 2) + Math.Pow(eq.B, 2)),
                ((-1 * eq.A * (-1 * eq.A * eq.B * point.Item2 + Math.Pow(eq.B, 2) * point.Item1 - eq.A * eq.C)) / (eq.B * (Math.Pow(eq.A, 2) + Math.Pow(eq.B, 2)))) - eq.C / eq.B);
        }
        public int Cross((double, double) o, (double, double) a, (double, double) b)
        {
            double value = (a.Item1 - o.Item1) * (b.Item2 - o.Item2) - (a.Item2 - o.Item2) * (b.Item1 - o.Item1);
            return Math.Abs(value) < 1e-10 ? 0 : value < 0 ? -1 : 1;
        }
        public double Distance((double, double) point1, (double, double) point2)
        {
            return Math.Sqrt(Math.Pow(point1.Item1 - point2.Item1, 2) + Math.Pow(point1.Item2 - point2.Item2, 2));
        }
    }
}