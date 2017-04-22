using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Dane
{
    public class ParserDanych
    {
        private IEnumerable<string> WczytajWiersze(Stream dane)
        {
            using (StreamReader czytnik = new StreamReader(dane))
            {
                List<string> wiersze = new List<string>();
                while (!czytnik.EndOfStream)
                {
                    wiersze.Add(czytnik.ReadLine());
                }
                return wiersze;
            }
        }

        public IList<double[]> ParsujDane(Stream strumienDanych)
        {
            return WczytajWiersze(strumienDanych)
                .Select(wiersz => wiersz.Split(new[] { ',' }))
                .Select(stringi => stringi
                    .Select(str => double.Parse(str))
                    .ToArray())
                .ToList();
        }
    }
}