using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie
{
    public class ReprezentacjaKlasaMrowki : IReprezentacjaPola
    {
        public int Dlugosc
        {
            get;
            set;
        } = 3;

        public string Okresl(Mrowka mrowka)
        {
            if (mrowka != null)
            {
                Console.BackgroundColor = (ConsoleColor)(mrowka.Klasa % 16);
                return Formatuj(mrowka.Klasa.ToString());
            }
            else
            {
                Console.ResetColor();
                return Formatuj(String.Empty);
            }
        }

        private string Formatuj(string tekst)
        {
            return tekst.PadLeft(Dlugosc);
        }
    }
}
