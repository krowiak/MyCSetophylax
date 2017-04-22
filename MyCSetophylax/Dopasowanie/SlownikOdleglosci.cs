using MyCSetophylax.KonkretneOdleglosci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Dopasowanie
{
    public class SlownikOdleglosci
    {
        private readonly Dictionary<(int mniejszeId, int wiekszeId), double> slownikOdleglosci;
        private readonly IOdleglosc<Mrowka> odleglosc;

        public SlownikOdleglosci(IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> odleglosc)
        {
            this.odleglosc = odleglosc;
            slownikOdleglosci = TworzSlownikOdleglosci(mrowki);
        }

        private Dictionary<(int mniejszeId, int wiekszeId), double> TworzSlownikOdleglosci(IEnumerable<Mrowka> mrowki)
        {
            var slownik = new Dictionary<(int mniejszeId, int wiekszeId), double>();
            var mrowkiWKolejnosci = mrowki.OrderBy(mrowka => mrowka.Id).ToArray();

            for (int i = 0; i < mrowkiWKolejnosci.Length; i++)
            {
                var mI = mrowkiWKolejnosci[i];
                for (int j = i + 1; j < mrowkiWKolejnosci.Length; j++)
                {
                    var mJ = mrowkiWKolejnosci[j];
                    var idPary = OkreslIdentyfikatorPary(mI, mJ);
                    var odlegloscPary = odleglosc.OkreslOdleglosc(mI, mJ);
                    slownik[idPary] = odlegloscPary;
                }
            }

            return slownik;
        }

        private (int mniejszeId, int wiekszeId) OkreslIdentyfikatorPary(Mrowka m1, Mrowka m2)
        {
            if (m1.Id < m2.Id)
            {
                return (m1.Id, m2.Id);
            }
            else
            {
                return (m2.Id, m1.Id);
            }
        }

        public double OkreslOdleglosc(Mrowka m1, Mrowka m2)
        {
            var identyfikatorPary = OkreslIdentyfikatorPary(m1, m2);
            return slownikOdleglosci[identyfikatorPary];
        }
    }
}
