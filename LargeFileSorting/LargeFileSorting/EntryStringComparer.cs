using System;
using System.Collections.Generic;

namespace LargeFileSorting
{
    class EntryStringComparer : IComparer<string>
    {
        static string Splitter = ". ";

        public int Compare(string x, string y)
        {
            var splitterIndexX = x.IndexOf(Splitter);
            if (splitterIndexX == -1)
                throw new ArgumentException($"wrong {nameof(x)} format, {nameof(x)} = {x}");
            var splitterIndexY = y.IndexOf(Splitter);
            if (splitterIndexY == -1)
                throw new ArgumentException($"wrong {nameof(y)} format, {nameof(y)} = {y}");

            var stringPartX = x.Substring(splitterIndexX + Splitter.Length);
            var stringPartY = x.Substring(splitterIndexY + Splitter.Length);

            var res = String.Compare(stringPartX, stringPartY, StringComparison.Ordinal);

            if (res == 0)
            {
                var numberPartX = x.Substring(0, splitterIndexX);
                if (!int.TryParse(numberPartX, out var numberX))
                    throw new ArgumentException($"wrong 'number' value: {numberPartX}, {nameof(x)} = {x}");

                var numberPartY = x.Substring(0, splitterIndexY);
                if (!int.TryParse(numberPartX, out var numberY))
                    throw new ArgumentException($"wrong 'number' value: {numberPartY}, {nameof(y)} = {y}");

                return numberX.CompareTo(numberY);
            }

            return res;
        }
    }
}