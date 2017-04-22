using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie
{
    public class WyswietlaczPrzestrzeni
    {
        private IReprezentacjaPola reprezentacja;

        public WyswietlaczPrzestrzeni(IReprezentacjaPola reprezentacja)
        {
            this.reprezentacja = reprezentacja;
        }

        public void Wyswietl(Przestrzen przestrzen)
        {
            Console.Write("   ");
            for (int i = 0; i < przestrzen.DlugoscBoku; i++)
            {
                Console.Write($"{i.ToString().PadLeft(4)}");
            }
            Console.WriteLine();

            for (int y = 0; y < przestrzen.DlugoscBoku; y++)
            {
                Console.Write($"{y.ToString().PadLeft(3)}");
                for (int x = 0; x < przestrzen.DlugoscBoku; x++)
                {
                    var zawartoscPola = przestrzen[y][x];
                    Console.Write($"|{reprezentacja.Okresl(zawartoscPola)}|");
                }
                Console.WriteLine();
            }
        }
    }
}