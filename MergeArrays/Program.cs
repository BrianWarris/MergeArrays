using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeArrays
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rand = new Random();  // max signed int is 2147483647
            MyTimer mt = new MyTimer();
            mt.init();
            System.Globalization.CultureInfo modCulture = new System.Globalization.CultureInfo("en-US");
            NumberFormatInfo number = modCulture.NumberFormat;
            number.NumberDecimalDigits = 0;
            int maxAr1Count = 16354725;
            int maxAr2Count = 14970435;

            Stack<int> ar1 = new Stack<int>();
            Stack<int> ar2 = new Stack<int>();
            int iter = 0;
            int startingValue = 1;
            int candidate = 1;
            while (true)
            {
                candidate = rand.Next(startingValue, Math.Min(startingValue + 3, int.MaxValue));
                ar1.Push(candidate);
                if ((startingValue == int.MaxValue) || (ar1.Count == maxAr1Count))
                {
                    Console.Out.WriteLine($"Exiting on value {candidate} in ar1, with {ar1.Count.ToString("N", number)} elements.");
                    break;
                }
                startingValue = candidate + 1;
                iter++;
            }
            var source =    Enumerable.Range(1, ar1.Count + maxAr2Count);

            var q = from full in source
                    join alreadyAssigned in ar1
                        on full equals alreadyAssigned into gj
                        from fe in gj.DefaultIfEmpty()
                    where fe == 0
                        select new
                        {
                            full
                        };

            if (q.Any())
            {
                foreach (var qe in q)
                {
                    ar2.Push(qe.full);
                }
            }

            Console.Out.WriteLine($"ar2 is populated with {ar2.Count.ToString("N", number)} elements.");
            mt.end();
            Console.Out.WriteLine($"Time taken to set up two int arrays: {mt.FormattedElapsedTime}");
            mt.init();
            int[] mergedArray = GetSortedMergedArray(ar1.ToArray(), ar2.ToArray());
            mt.end();
            Console.Out.WriteLine($"Time taken to merge these two into an array of {mergedArray.Length.ToString("N", number)} elements: {mt.FormattedElapsedTime}");
            Debug.Assert(mergedArray.Length == (ar1.Count + ar2.Count));
        }
        static int[] GetSortedMergedArray(int[] a1, int[] a2)
        {
            int[] rv = new int[a1.Length + a2.Length];
            int i = 0;
            int j = 0;
            int dest = 0;
            while ((i < a1.Length) && (j < a2.Length))
            {
                if (a1[i].CompareTo(a2[j]) < 0)
                {
                    rv[dest] = a1[i];
                    dest++;
                    i++;
                }
                else // we were given array elements are mutually exclusive
                {
                    rv[dest] = a2[j];
                    dest++;
                    j++;
                }
            }
            while (i < a1.Length)
            {
                rv[dest] = a1[i];
                dest++;
                i++;
            }
            while (j < a2.Length)
            {
                rv[dest] = a2[j];
                dest++;
                j++;
            }
            return rv;
        }
    }
}
