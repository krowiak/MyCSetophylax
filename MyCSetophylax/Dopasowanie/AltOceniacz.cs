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
    /// <summary>
    /// Oceniacz bazujący na "AN ADAPTIVE ANT COLONY CLUSTERING ALGORITHM" zamiast "A novel ant clustering algorithm based on
    /// cellular automata"
    /// </summary>
    public class AltOceniacz : IOceniacz
    {
        private readonly Sasiedztwo sasiedztwo;
        private readonly IOdleglosc<Mrowka> sloOdleglosci;
        private readonly ISrednieOdleglosci srednieOdleglosci;

        public AltOceniacz(IOdleglosc<Mrowka> slownikOdleglosci, ISrednieOdleglosci srednieOdleglosci, Sasiedztwo sasiedztwo)
        {
            sloOdleglosci = slownikOdleglosci;
            this.srednieOdleglosci = srednieOdleglosci;
            this.sasiedztwo = sasiedztwo;
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

            var odlKwadrat = odleglosc * odleglosc;
            var sredniaOdlKwadrat = sredniaOdlDlaOcenianej * sredniaOdlDlaOcenianej;
            var stosunekSredniejDoFaktycznej = sredniaOdlKwadrat / (odlKwadrat + sredniaOdlKwadrat);
            return stosunekSredniejDoFaktycznej;
        }

        public double Ocen(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            (int x, int y) = pozycjaMrowki;
            var dzielnik = ObliczDzielnikOceny();
            var mrowkiWSasiedztwie = sasiedztwo.MrowkiWSasiedztwie(x, y);
            var sumaSkladowych = mrowkiWSasiedztwie.Sum(sasiadka => OkreslSkladowaOceny(mrowka, sasiadka));
            var wynikObliczen = sumaSkladowych / dzielnik;
            // var ocena = Math.Max(wynikObliczen, 0.0);  // Brak maxa w tym wzorze z powodu braku odejmowania
            return wynikObliczen;
        }
    }
}
