using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Entry : IComparable<Entry>
    {
        public Entry(string stringValue)
        {
            var splitterIndex = stringValue.IndexOf(Separator);
            //if (splitterIndex == -1)
            //    throw new ArgumentException($"wrong {nameof(stringValue)} format, {nameof(stringValue)} = {stringValue}");

            var numberPart = stringValue.Substring(0, splitterIndex);
            var stringPart = stringValue.Substring(splitterIndex + Separator.Length);

            if (!int.TryParse(numberPart, out var number))
                throw new ArgumentException($"wrong 'number' value: {numberPart}, {nameof(stringValue)} = {stringValue}");

            Number = number;
            String = stringPart;
        }

        public Entry(int number, string value)
        {
            Number = number;
            String = value;
        }

        public int Number { get; }
        public string String { get; }

        public int CompareTo(Entry other)
        {
            var res = String.CompareTo(other.String);
            if (res == 0)
                res = Number.CompareTo(other.Number);

            return res;
        }

        public string ConvertToString()
        {
            return $"{Number}{Separator}{String}";
        }

        static string Separator = ". ";

        public override string ToString()
        {
            return ConvertToString();
        }
    }
}
