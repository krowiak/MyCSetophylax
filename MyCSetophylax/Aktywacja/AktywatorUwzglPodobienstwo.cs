using MyCSetophylax.KonkretneOdleglosci;
using MyCSetophylax.PrzestrzenZyciowa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Aktywacja
{
    public class AktywatorUwzglPodobienstwo
    {
        private readonly Sasiedztwo sasiedztwo;
        private readonly IOdleglosc<Mrowka> odleglosci;
        private readonly int minSasiadow;
        private readonly double progNiepodobienstwa;

        public AktywatorUwzglPodobienstwo(Sasiedztwo sasiedztwo, IOdleglosc<Mrowka> odleglosci, int minSasiadow, double progNiepodobienstwa)
        {
            this.sasiedztwo = sasiedztwo;
            this.odleglosci = odleglosci;
            this.minSasiadow = minSasiadow;
            this.progNiepodobienstwa = progNiepodobienstwa;
        }

        public bool CzyAktywowac(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            var (x, y) = pozycjaMrowki;
            var sasiadki = sasiedztwo.MrowkiWSasiedztwie(x, y).ToList();
            if (sasiadki.Count >= minSasiadow)
            {
                var saNiepodobneSasiadki = sasiadki.Any(sasiadka => odleglosci.OkreslOdleglosc(mrowka, sasiadka) >= progNiepodobienstwa);
                return saNiepodobneSasiadki;
            }
            return false;
        }
    }
}
