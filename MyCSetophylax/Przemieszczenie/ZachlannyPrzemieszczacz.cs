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
                var najlepszePole = pustePola
                    .Select(nowaPozycja => new { Pole = nowaPozycja, Ocena = oceniacz.Ocen(mrowka, nowaPozycja) })
                    .OrderByDescending(poleZOcena => poleZOcena.Ocena)
                    .First()
                    .Pole;
                przestrzen[najlepszePole.Y][najlepszePole.X] = mrowka;
            }

            /////////////
            //// Poniżej: wyrzucanie mrówki dalej, jeśli brak pustych pól w sąsiedztwie.
            //// Na szybko && brzydko, ale nie szkodzi, wyniki praktycznie identyczne.
            //// Klasyczny przemieszczacz nie miał czegoś takiego, ale że odpowiada za 10% przypadków raczej nie pomoże.
            /////////////

            //else
            //{
            //    var pustePolaWSasiedztwiePlus = new Sasiedztwo(przestrzen, sasiedztwo.ZasiegX * 2, sasiedztwo.ZasiegY * 2).PustePolaWSasiedztwie(x, y).ToList();
            //    if (pustePolaWSasiedztwiePlus.Any())
            //    {
            //        przestrzen[y][x] = null;
            //        var najlepszePole = pustePolaWSasiedztwiePlus
            //            .Select(nowaPozycja => new { Pole = nowaPozycja, Ocena = oceniacz.Ocen(mrowka, nowaPozycja) })
            //            .OrderByDescending(poleZOcena => poleZOcena.Ocena)
            //            .First()
            //            .Pole;
            //        przestrzen[najlepszePole.Y][najlepszePole.X] = mrowka;
            //        Console.WriteLine("Pomogło!");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Nie pomogło...");
            //    }
            //}
        }

        private void PrzemiescKlasycznie(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            alternatywnyPrzemieszczacz.Przemiesc(mrowka, pozycjaMrowki);
        }
    }
}