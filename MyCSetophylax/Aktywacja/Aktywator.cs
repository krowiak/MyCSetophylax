using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Aktywacja
{
    public class Aktywator
    {
        private readonly double pAktywacji;
        private readonly Czas czas;
        private readonly Random los;
        private readonly IPresja presja;

        public Aktywator(Czas czas, Random los, double bazowePrawdopodobienstwoAktywacji, IPresja presjaAktywacji)
        {
            this.czas = czas;
            this.los = los;
            pAktywacji = bazowePrawdopodobienstwoAktywacji;
            presja = presjaAktywacji;
        }

        public bool CzyAktywowac(double ocenaMrowki)
        {
            var szansaAktywacji = OkreslSzanseAktywacji(ocenaMrowki);
            var wartoscLosowa = los.NextDouble();
            bool aktywuj = wartoscLosowa <= szansaAktywacji;
            return aktywuj;
        }

        private double OkreslSzanseAktywacji(double ocenaMrowki)
        {
            var wartPresji = presja.OkreslPresje();
            var betaDoLambda = Math.Pow(pAktywacji, wartPresji);
            var wplywOceny = Math.Pow(ocenaMrowki, wartPresji);

            var licznik = betaDoLambda;
            var mianownik = betaDoLambda + wplywOceny;
            var szansaAktywacji = licznik / mianownik;
            return szansaAktywacji;
        }
    }
}