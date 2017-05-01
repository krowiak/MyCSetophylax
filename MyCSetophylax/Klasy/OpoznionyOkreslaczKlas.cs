using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Klasy
{
    public class OpoznionyOkreslaczKlas : IOkreslaczKlas
    {
        private readonly IOkreslaczKlas faktycznyOkreslacz;
        private readonly Czas czas;
        private readonly int czasRozpoczecia;

        public OpoznionyOkreslaczKlas(Czas czas, int czasRozpoczeciaDzialania, IOkreslaczKlas faktycznyOkreslacz)
        {
            this.faktycznyOkreslacz = faktycznyOkreslacz;
            this.czas = czas;
            czasRozpoczecia = czasRozpoczeciaDzialania;
        }

        public int OkreslKlase(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            if (czasRozpoczecia <= czas.Aktualny)
            {
                return faktycznyOkreslacz.OkreslKlase(mrowka, pozycjaMrowki);
            }
            else
            {
                return mrowka.Klasa;
            }
        }
    }
}
