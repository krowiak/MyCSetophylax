using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.KonkretneOdleglosci
{
    public class OdleglosciPomiedzyMrowkami : IOdleglosc<Mrowka>
    {
        private Dictionary<(int, int), double> odleglosci;

        public OdleglosciPomiedzyMrowkami(IEnumerable<Mrowka> mrowki)
        {
            odleglosci = tworzSlownikOdleglosci(mrowki);
        }

        private Dictionary<(int, int), double> tworzSlownikOdleglosci(IEnumerable<Mrowka> mrowki)
        {
            var odlEuklidesowa = new OdlegloscEuklidesowa();
            var slownik = new Dictionary<(int, int), double>();
            var uporzadkowaneMrowki = mrowki.OrderBy(mrowka => mrowka.Id).ToList();
            for (int i = 0; i < uporzadkowaneMrowki.Count - 1; i++)
            {
                var m1 = uporzadkowaneMrowki[i];
                var m2 = uporzadkowaneMrowki[i + 1];
                var odleglosc = odlEuklidesowa.OkreslOdleglosc(m1.Dane, m2.Dane);
                slownik[(m1.Id, m2.Id)] = odleglosc;
            }
            return slownik;
        }

        private (int, int) UporzadkujId(Mrowka m1, Mrowka m2)
        {
            return m1.Id < m2.Id ?
                (m1.Id, m2.Id) :
                (m2.Id, m1.Id);
        }

        public double OkreslOdleglosc(Mrowka sprawdzanaMrowka, Mrowka odleglaMrowka)
        {
            var uporzadkowaneId = UporzadkujId(sprawdzanaMrowka, odleglaMrowka);
            return odleglosci[uporzadkowaneId];
        }
    }
}
