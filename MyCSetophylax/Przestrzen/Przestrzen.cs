using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Przestrzen
{
    public class Przestrzen
    {
        private WierszPrzestrzeni[] wiersze;

        private static int obliczOdpowiedzniBokPrzestrzeniDla(int liczbaMrowek)
        {
            var n = (double)liczbaMrowek;
            var pierwiastek = Math.Sqrt(n);
            var zaokraglenie = (int)Math.Round(Math.Ceiling(pierwiastek), MidpointRounding.AwayFromZero);
            return 2 * (zaokraglenie + 1);
        }

        public static Przestrzen StworzPrzestrzenDla(int liczbaMrowek)
        {
            var pozadanaDlugoscBoku = obliczOdpowiedzniBokPrzestrzeniDla(liczbaMrowek);
            return new Przestrzen(pozadanaDlugoscBoku);
        }

        public Przestrzen(int dlugoscBoku)
        {
            wiersze = new WierszPrzestrzeni[dlugoscBoku];
            for (int i = 0; i < dlugoscBoku; i++)
            {
                wiersze[i] = new WierszPrzestrzeni(dlugoscBoku);
            }
        }

        public WierszPrzestrzeni this[int indeksWiersza]
        {
            get { return wiersze[indeksWiersza]; }
        }

        public class WierszPrzestrzeni
        {
            private Mrowka[] pola;

            public WierszPrzestrzeni(int dlugoscBoku)
            {
                pola = new Mrowka[dlugoscBoku];
            }

            public Mrowka this[int indeksKolumny]
            {
                get { return pola[indeksKolumny]; }
                set { pola[indeksKolumny] = value; }
            }
        }
    }
}