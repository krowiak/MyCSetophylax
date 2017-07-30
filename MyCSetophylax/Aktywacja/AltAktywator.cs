using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Aktywacja
{
    public class AltAktywator : IAktywator
    {
        private readonly Random los;
        private readonly IPresja presja;

        public AltAktywator(Random los, IPresja presjaAktywacji)
        {
            this.los = los;
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
            var cosinus = Math.Cos(0.5 * ocenaMrowki);
            var prawdopAktywacji = Math.Pow(cosinus, wartPresji);  // Nie wydaje mi się, żeby wynik potęgowania 0 było szkodliwe w tym wzorze, więc zostawiam bez Max(wynik, Epsilon)
            return prawdopAktywacji;
        }
    }
}
