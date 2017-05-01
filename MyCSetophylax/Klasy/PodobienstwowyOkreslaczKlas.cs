using MyCSetophylax.KonkretneOdleglosci;
using MyCSetophylax.PrzestrzenZyciowa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Klasy
{
    public class PodobienstwowyOkreslaczKlas : IOkreslaczKlas
    {
        private readonly IOdleglosc<Mrowka> odleglosci;
        private readonly Sasiedztwo sasiedztwo;

        public PodobienstwowyOkreslaczKlas(IOdleglosc<Mrowka> odleglosciPomiedzyMrowkami, Sasiedztwo sasiedztwo)
        {
            odleglosci = odleglosciPomiedzyMrowkami;
            this.sasiedztwo = sasiedztwo;
        }

        public int OkreslKlase(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            var (x, y) = pozycjaMrowki;
            var mrowkiWSasiedztwie = sasiedztwo.MrowkiWSasiedztwie(x, y).ToList();
            if (mrowkiWSasiedztwie.Any())
            {
                var najpodobniejszaKlasa = mrowkiWSasiedztwie
                    .GroupBy(innaMrowka => innaMrowka.Klasa)
                    .OrderByDescending(
                        klasa => klasa.Average(innaMrowka => odleglosci.OkreslOdleglosc(mrowka, innaMrowka))
                    ).First().Key;
                return najpodobniejszaKlasa;
            }
            else
            {
                return mrowka.Klasa;
            }
        }
    }
}
