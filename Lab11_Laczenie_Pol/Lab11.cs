using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class Lab11 : System.MarshalByRefObject
    {

        // iloczyn wektorowy
        private int Cross((double, double) o, (double, double) a, (double, double) b)
        {
            double value = (a.Item1 - o.Item1) * (b.Item2 - o.Item2) - (a.Item2 - o.Item2) * (b.Item1 - o.Item1);
            return Math.Abs(value) < 1e-10 ? 0 : value < 0 ? -1 : 1;
        }

        // Etap 1
        // po prostu otoczka wypukła
        public (double, double)[] ConvexHull((double, double)[] points)
        {
            var l1 = new List<(double, double)>(points);
            l1.Sort();
            var l2 = new List<(double, double)>();
            foreach (var i in l1.Distinct())
                l2.Add(i);
            if (l2.Count < 2)
                return l2.ToArray();
            var l3 = new List<(double, double)>();
            foreach(var i in l2)
            {
                while (l3.Count > 1 && Cross(l3[l3.Count - 2], l3[l3.Count - 1], i) <= 0)
                    l3.RemoveAt(l3.Count - 1);
                l3.Add(i);
            }
            l2.Reverse();
            var l4 = new List<(double, double)>();
            foreach (var i in l2)
            {
                while (l4.Count > 1 && Cross(l4[l4.Count - 2], l4[l4.Count - 1], i) <= 0)
                    l4.RemoveAt(l4.Count - 1);
                l4.Add(i);
            }
            l3.RemoveAt(l3.Count - 1);
            l4.RemoveAt(l4.Count - 1);
            l3.AddRange(l4);
            return l3.ToArray();
        }

        // Etap 2
        // oblicza otoczkę dwóch wielokątów wypukłych
        public (double, double)[] ConvexHullOfTwo((double, double)[] poly1, (double, double)[] poly2)
        {
            (var lower1, var upper1) = ToHalfs(Align(poly1));
            (var lower2, var upper2) = ToHalfs(Align(poly2));

            upper1.Reverse();
            upper2.Reverse();

            var lower = Merge(lower1, lower2);
            var upper = Merge(upper1, upper2);
            upper.Reverse();

            var bottom = Half(lower);
            bottom.RemoveAt(bottom.Count - 1);

            var top = Half(upper);
            top.RemoveAt(top.Count - 1);

            bottom.AddRange(top);
            return bottom.ToArray();
        }
        public (List<(double, double)>, List<(double, double)>) ToHalfs((double, double)[] poly)
        {
            var max = poly.Max(x => x.Item1);
            var lower = new List<(double, double)>();
            foreach(var i in poly)
            {
                if (i.Item1 < max)
                    lower.Add(i);
                else break;
            }
            for(int i = lower.Count; i < poly.Length; i++)
            {
                if (poly[i].Item1 == max)
                    lower.Add(poly[i]);
            }
            var upper = poly.Skip(lower.Count - 1).ToList();
            upper.Add(poly[0]);
            return (lower, upper);
        }
        public (double, double)[] Align((double, double)[] poly)
        {
            var min = poly.Min(x => x.Item1);
            if(poly[0].Item1 == min)
            {
                for(int i = 1; i < poly.Length; i++)
                {
                    if (poly[i].Item1 != min)
                        return poly.Skip(i - 1).Concat(poly.Take(i - 1)).ToArray();
                }
            }
            else
            {
                for(int i = poly.Length - 1; i >= 0; i--)
                {
                    if (poly[i].Item1 == min)
                        return poly.Skip(i).Concat(poly.Take(i)).ToArray();
                }
            }
            return poly;
        }
        public List<(double, double)> Merge(List<(double, double)> a, List<(double, double)> b)
        {
            var res = new List<(double, double)>();
            int counta = 0;
            int countb = 0;

            while(a.Count != counta || b.Count != countb)
            {
                if(a.Count == counta)
                {
                    res.Add(b[countb++]);
                }
                else if(b.Count == countb)
                {
                    res.Add(a[counta++]);
                }
                else
                {
                    var ela = a[counta];
                    var elb = b[countb];
                    if(ela.Item1 < elb.Item1 || (ela.Item1 == elb.Item1 && ela.Item2 < elb.Item2))
                    {
                        counta++;
                        res.Add(ela);
                    }
                    else
                    {
                        countb++;
                        res.Add(elb);
                    }
                }
            }
            return res;
        }
        public List<(double, double)> Half(List<(double, double)> poly)
        {
            var res = new List<(double, double)>();
            foreach(var i in poly)
            {
                while (res.Count > 1 && Cross(res[res.Count - 2], res[res.Count - 1], i) <= 0)
                    res.RemoveAt(res.Count - 1);
                res.Add(i);
            }
            return res;
        }
    }
}
