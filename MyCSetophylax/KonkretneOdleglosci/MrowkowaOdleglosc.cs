using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.KonkretneOdleglosci
{
    public class MrowkowaOdleglosc : IOdleglosc<Mrowka>
    {
        private readonly IOdleglosc<double[]> odlegloscBazowa;

        public MrowkowaOdleglosc(IOdleglosc<double[]> odlegloscBazowa)
        {
            this.odlegloscBazowa = odlegloscBazowa;
        }

        public double OkreslOdleglosc(Mrowka obj1, Mrowka obj2)
        {
            return odlegloscBazowa.OkreslOdleglosc(obj1.Dane, obj2.Dane);
        }
    }
}
