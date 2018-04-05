using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnsortedFileGenerator
{
    public static class PhrasesFactory
    {
        static PhrasesFactory()
        {
            Phrases = Properties.Resources.Phrases.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] Phrases { get; }
        //= new List<string>()
        //{
        //    "Banana is yellow",
        //    "Apple",
        //    "Cherry is the best",
        //    "Strawbery is sweet",
        //    "Stars is light",
        //    "Orange is orange",
        //    "Lemon is yellow",
        //    "Lime is Green",
        //    "Mango is like a pin",
        //    "Something something something",
        //};

        static Random Random = new Random();

        public static string GetRandomPhrase()
        {
            var ind = Random.Next(0, Phrases.Length - 1);

            return Phrases[ind];
        }
    }
}


