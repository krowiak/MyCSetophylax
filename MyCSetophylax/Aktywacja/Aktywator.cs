using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Aktywacja
{
    public class Aktywator : IAktywator
    {
        private readonly double pAktywacji;
        private readonly Random los;
        private readonly IPresja presja;

        public Aktywator(Random los, double bazowePrawdopodobienstwoAktywacji, IPresja presjaAktywacji)
        {
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
            // Dla bardzo wysokiej presji wynik potęgowania może wynieść 0, co nie może być prawidłowe dla pAktywacji > 0
            var betaDoLambda = Math.Max(Math.Pow(pAktywacji, wartPresji), double.Epsilon);  
            var wplywOceny = Math.Pow(ocenaMrowki, wartPresji);

            var licznik = betaDoLambda;
            var mianownik = betaDoLambda + wplywOceny;
            var szansaAktywacji = licznik / mianownik;
            return szansaAktywacji;
        }
    }
}