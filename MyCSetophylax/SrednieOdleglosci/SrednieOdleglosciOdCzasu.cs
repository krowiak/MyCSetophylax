using MyCSetophylax.Dopasowanie;
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
        private readonly Dictionary<int, double> poprzednieWartosci;
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
            poprzednieWartosci = new Dictionary<int, double>();
        }

        public double OkreslSredniaOdleglosc(Mrowka sprawdzanaMrowka)
        {
            if (czas.Aktualny <= deltaT)
            {
                // Im bardziej na to patrzę, tym bardziej rozsądne wydaje mi się, żeby raczej używać stałej, choćby i 0.
                // Pierwsze kilka iteracji i tak jest praktycznie bez znaczenia.
                var wartoscPoczatkowa = zrodloWartPoczatkowych.OkreslSredniaOdleglosc(sprawdzanaMrowka);
                poprzednieWartosci[czas.Aktualny] = wartoscPoczatkowa;
                return wartoscPoczatkowa;// - kAlfa * wartoscPoczatkowa;
            }
            else
            {
                int staryCzas = czas.Aktualny - deltaT;
                double wartHistorycznaDopasowania = srednieDopasowaniaOdCzasu[staryCzas];
                double roznicaSrednich = wartHistorycznaDopasowania - srednieDopasowaniaOdCzasu[czas.Aktualny - 1];  // Jeśli -1 tu nadal jest: to nieprzemyślana poprawka na szybko, bo średniej dla danego aktualnego t jeszcze nie było. Przemyśleć i ew. poprawić.
                double wartHistorycznaSredniejOdl = poprzednieWartosci[staryCzas];
                double wartAktualna = wartHistorycznaSredniejOdl - kAlfa * roznicaSrednich;
                poprzednieWartosci[czas.Aktualny] = wartAktualna;
                return wartAktualna;
            }
        }
    }
}