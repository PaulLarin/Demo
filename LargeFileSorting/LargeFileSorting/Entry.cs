using System;
using System.Linq;

namespace LargeFileSorting
{
        class Entry : IComparable<Entry>
        {
            public Entry(string stringValue)
            {
                var splitterIndex = stringValue.IndexOf(Splitter);
                if (splitterIndex == -1)
                    throw new ArgumentException($"wrong {nameof(stringValue)} format, {nameof(stringValue)} = {stringValue}");

                var numberPart = stringValue.Substring(0, splitterIndex);
                var stringPart = stringValue.Substring(splitterIndex + Splitter.Length);

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
                if (other == null)
                    return 1;

                var res = String.CompareTo(other.String);
                if (res == 0)
                    res = Number.CompareTo(other.Number);

                return res;
            }

            public string ConvertToString()
            {
                return $"{Number}{Splitter}{String}";
            }

            static string Splitter = ". ";

            public override string ToString()
            {
                return ConvertToString();
            }
        }

    }
