using MyCSetophylax.KonkretneOdleglosci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Dopasowanie
{
    public class SlownikOdleglosci : IOdleglosc<Mrowka>
    {
        private readonly double[][] slownikOdleglosci;
        private readonly IOdleglosc<Mrowka> odleglosc;

        public SlownikOdleglosci(IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> odleglosc)
        {
            this.odleglosc = odleglosc;
            slownikOdleglosci = TworzSlownikOdleglosci(mrowki);
        }

        private double[][] TworzSlownikOdleglosci(IEnumerable<Mrowka> mrowki)
        {
            var mrowkiWKolejnosci = mrowki.OrderBy(mrowka => mrowka.Id).ToArray();
            var liczbaMrowek = mrowkiWKolejnosci.Length;
            var slownik = new double[liczbaMrowek][];

            for (int i = 0; i < liczbaMrowek; i++)
            {
                var wierszMrowki = new double[liczbaMrowek - i];  // Zawsze o 1 za duże (na [0]), ale to nie powinna być duża różnica
                slownik[i] = wierszMrowki;
                var mI = mrowkiWKolejnosci[i];
                for (int j = i + 1; j < mrowkiWKolejnosci.Length; j++)
                {
                    var mJ = mrowkiWKolejnosci[j];
                    var idPary = OkreslIndeksowaniePary(mI, mJ);
                    var odlegloscPary = odleglosc.OkreslOdleglosc(mI, mJ);
                    slownik[idPary.mniejszeId][idPary.przesuniecie] = odlegloscPary;
                }
            }

            return slownik;
        }

        private (int mniejszeId, int przesuniecie) OkreslIndeksowaniePary(Mrowka m1, Mrowka m2)
        {
            if (m1.Id < m2.Id)
            {
                return (m1.Id, m2.Id - m1.Id);
            }
            else
            {
                return (m2.Id, m1.Id - m2.Id);
            }
        }

        public double OkreslOdleglosc(Mrowka m1, Mrowka m2)
        {
            var identyfikatorPary = OkreslIndeksowaniePary(m1, m2);
            return slownikOdleglosci[identyfikatorPary.mniejszeId][identyfikatorPary.przesuniecie];
        }
    }
}
