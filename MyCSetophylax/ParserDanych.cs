using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax
{
    public class ParserDanych
    {
        private IEnumerable<string> wczytajWiersze(Stream dane)
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
            return wczytajWiersze(strumienDanych)
                .Select(wiersz => wiersz.Split(new[] { ',' }))
                .Select(stringi => stringi
                    .Select(str => double.Parse(str))
                    .ToArray())
                .ToList();
        }
    }
}