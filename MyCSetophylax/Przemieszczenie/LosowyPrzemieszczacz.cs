using MyCSetophylax.PrzestrzenZyciowa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Przemieszczenie
{
    public class LosowyPrzemieszczacz : IPrzemieszczacz
    {
        private readonly Przestrzen przestrzen;
        private readonly Sasiedztwo sasiedztwo;
        private readonly Random los;

        public LosowyPrzemieszczacz(Przestrzen przestrzen, Sasiedztwo sasiedztwo, Random los)
        {
            this.przestrzen = przestrzen;
            this.sasiedztwo = sasiedztwo;
            this.los = los;
        }

        public void Przemiesc(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            (int x, int y) = pozycjaMrowki;
            var pustePola = sasiedztwo.PustePolaWSasiedztwie(x, y).ToArray();
            var liczbaPustychPol = pustePola.Count();
            if (liczbaPustychPol > 0)
            {
                var indeksWybranego = los.Next(liczbaPustychPol);
                var wybrane = pustePola[indeksWybranego];
                przestrzen[wybrane.Y][wybrane.X] = mrowka;
                przestrzen[y][x] = null;
            }
            // else?
        }
    }
}
