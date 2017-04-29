using MyCSetophylax.Dopasowanie;
using MyCSetophylax.PrzestrzenZyciowa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Przemieszczenie
{
    public class ZachlannyPrzemieszczacz : IPrzemieszczacz
    {
        private readonly double szansaNaZachlannosc;
        private readonly IPrzemieszczacz alternatywnyPrzemieszczacz;
        private readonly Oceniacz oceniacz;
        private readonly Przestrzen przestrzen;
        private readonly Sasiedztwo sasiedztwo;
        private readonly Random los;

        public ZachlannyPrzemieszczacz(double szansaNaZachlannosc, IPrzemieszczacz alternatywnyPrzemieszczacz, Oceniacz oceniacz,
            Przestrzen przestrzen, Sasiedztwo sasiedztwo, Random los)
        {
            this.szansaNaZachlannosc = szansaNaZachlannosc;
            this.alternatywnyPrzemieszczacz = alternatywnyPrzemieszczacz;
            this.oceniacz = oceniacz;
            this.przestrzen = przestrzen;
            this.sasiedztwo = sasiedztwo;
            this.los = los;
        }

        public void Przemiesc(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            var losowaLiczba = los.NextDouble();
            var czyZachlannie = losowaLiczba <= szansaNaZachlannosc;
            if (czyZachlannie)
            {
                PrzemiescZachlannie(mrowka, pozycjaMrowki);
            }
            else
            {
                PrzemiescKlasycznie(mrowka, pozycjaMrowki);
            }
        }

        private void PrzemiescZachlannie(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            (int x, int y) = pozycjaMrowki;
            var pustePola = sasiedztwo.PustePolaWSasiedztwie(x, y).ToArray();
            if (pustePola.Any())
            {
                przestrzen[y][x] = null;
                var isUponAs = pustePola
                    .Select(nowaPozycja => new { Pole = nowaPozycja, Ocena = oceniacz.Ocen(mrowka, nowaPozycja) });
                var fighter = isUponAs
                    .OrderByDescending(poleZOcena => poleZOcena.Ocena);
                var face = fighter.First();
                var najlepszePole = pustePola
                    .Select(nowaPozycja => new {Pole = nowaPozycja, Ocena = oceniacz.Ocen(mrowka, nowaPozycja)})
                    .OrderByDescending(poleZOcena => poleZOcena.Ocena)
                    .First()
                    .Pole;
                przestrzen[najlepszePole.Y][najlepszePole.X] = mrowka;
            }

            // else?
        }

        private void PrzemiescKlasycznie(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            alternatywnyPrzemieszczacz.Przemiesc(mrowka, pozycjaMrowki);
        }
    }
}



//let przemiescZachlannieWlasciwe funOceny(przestrzen:Przestrzen) sasiedztwo(mrowkaX, mrowkaY) =
//    let przemieszczanaMrowka = przestrzen.[mrowkaX, mrowkaY] |> Option.get
//    let pusteSasiedztwo = sasiedztwo |> PustePolaZSasiedztwa przestrzen
//    if Seq.length pusteSasiedztwo > 0 then
//        przestrzen.[mrowkaX, mrowkaY] <- None
//        let (najlepszeX, najlepszeY) = pusteSasiedztwo |> Seq.maxBy(funOceny przemieszczanaMrowka)
//        przestrzen.[najlepszeX, najlepszeY] <- Some przemieszczanaMrowka