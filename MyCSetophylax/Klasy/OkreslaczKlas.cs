using MyCSetophylax.PrzestrzenZyciowa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Klasy
{
    public class OkreslaczKlas : IOkreslaczKlas
    {
        private readonly Sasiedztwo sasiedztwo;

        public OkreslaczKlas(Sasiedztwo sasiedztwo)
        {
            this.sasiedztwo = sasiedztwo;
        }

        public int OkreslKlase(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            var klasaDocelowa = mrowka.Klasa;
            (int x, int y) = pozycjaMrowki;
            var sasiadki = sasiedztwo.MrowkiWSasiedztwie(x, y).ToList();
            if (sasiadki.Count > 0)
            {
                klasaDocelowa = sasiadki.GroupBy(sasiadka => sasiadka.Klasa)
                    .OrderBy(grupa => grupa.Count())
                    .First()
                    .Key;
            }
            return klasaDocelowa;
        }
    }
}