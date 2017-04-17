using MyCSetophylax.KonkretneOdleglosci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.SrednieOdleglosci
{
    public class SrednieOdleglosciOdCzasu : ISrednieOdleglosci
    {
        private readonly SrednieDopasowaniaMrowek srednieDopasowaniaOdCzasu;
        private readonly int deltaT;
        private readonly double kAlfa;
        private readonly ISrednieOdleglosci zrodloWartPoczatkowych;
        private readonly Czas czas;

        public SrednieOdleglosciOdCzasu(Czas czas, SrednieDopasowaniaMrowek aktualneDopasowaniaMrowek, KonfiguracjaSredniejOdlOdCzasu konfiguracja)
        {
            this.czas = czas;
            srednieDopasowaniaOdCzasu = aktualneDopasowaniaMrowek;
            deltaT = konfiguracja.IleJednostekCzasuSpogladacWstecz;
            kAlfa = konfiguracja.StopienWplywuRoznicySrednichNaWynikWDanejJednostceCzasu;
            zrodloWartPoczatkowych = konfiguracja.SposobOkreslaniaWartosciPrzedUaktywnieniem;
        }

        public double OkreslSredniaOdleglosc(Mrowka sprawdzanaMrowka)
        {
            if (czas.Aktualny <= deltaT)
            {
                // Im bardziej na to patrzę, tym bardziej rozsądne wydaje mi się, żeby raczej używać stałej, choćby i 0.
                // Pierwsze kilka iteracji i tak jest praktycznie bez znaczenia.
                var wartosciPoczatkowa = zrodloWartPoczatkowych.OkreslSredniaOdleglosc(sprawdzanaMrowka);
                return wartosciPoczatkowa - kAlfa * wartosciPoczatkowa;
            }
            else
            {
                int staryCzas = czas.Aktualny - deltaT;
                double wartHistoryczna = srednieDopasowaniaOdCzasu[staryCzas];
                double roznicaSrednich = wartHistoryczna - srednieDopasowaniaOdCzasu[czas.Aktualny];
                return wartHistoryczna - kAlfa * roznicaSrednich;
            }
        }
    }
}