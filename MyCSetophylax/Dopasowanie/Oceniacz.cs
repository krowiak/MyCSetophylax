using MyCSetophylax.KonkretneOdleglosci;
using MyCSetophylax.PrzestrzenZyciowa;
using MyCSetophylax.SrednieOdleglosci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Dopasowanie
{
    public class Oceniacz : IOceniacz
    {
        private readonly Sasiedztwo sasiedztwo;
        private readonly IOdleglosc<Mrowka> sloOdleglosci;
        private readonly ISrednieOdleglosci srednieOdleglosci;

        public Oceniacz(IOdleglosc<Mrowka> slownikOdleglosci, ISrednieOdleglosci srednieOdleglosci, Sasiedztwo sasiedztwo)
        {
            sloOdleglosci = slownikOdleglosci;
            this.srednieOdleglosci = srednieOdleglosci;
            this.sasiedztwo = sasiedztwo;
        }

        public double Ocen(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            (int x, int y) = pozycjaMrowki;
            var dzielnik = ObliczDzielnikOceny();
            var mrowkiWSasiedztwie = sasiedztwo.MrowkiWSasiedztwie(x, y);
            var sumaSkladowych = mrowkiWSasiedztwie.Sum(sasiadka => OkreslSkladowaOceny(mrowka, sasiadka));
            //    + (sasiedztwo.RozmiarSasiedztwa - mrowkiWSasiedztwie.Count()) * 0.1;  // Próba zachęcenia mrówek do unikania niepodobnych sąsiadów bardziej niż pustki, niezbyt udana
            var wynikObliczen = sumaSkladowych / dzielnik;
            var ocena = Math.Max(wynikObliczen, 0.0);
            return ocena;
        }

        private double ObliczDzielnikOceny()
        {
            var komponentX = 1.0 + sasiedztwo.ZasiegX * 2.0;
            var komponentY = 1.0 + sasiedztwo.ZasiegY * 2.0;
            var dzielnik = komponentX * komponentY;
            return dzielnik;
        }

        private double OkreslSkladowaOceny(Mrowka oceniana, Mrowka zSasiedztwa)
        {
            var odleglosc = sloOdleglosci.OkreslOdleglosc(oceniana, zSasiedztwa);
            var sredniaOdlDlaOcenianej = srednieOdleglosci.OkreslSredniaOdleglosc(oceniana);
            var stosunekOdleglosciDoSredniej = odleglosc / sredniaOdlDlaOcenianej;
            var skladowaOceny = 1.0 - stosunekOdleglosciDoSredniej;
            return skladowaOceny;
        }
    }
}
