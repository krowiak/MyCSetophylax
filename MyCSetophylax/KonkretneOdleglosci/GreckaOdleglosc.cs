using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.KonkretneOdleglosci
{
    public class GreckaOdleglosc : IOdleglosc<double[]>
    {
        // Na bazie tej globalnościowej Sirakoulisa
        public double OkreslOdleglosc(double[] obj1, double[] obj2)
        {
            var sumaProporcjiWszystkichWymiarow = 0.0;
            for (int i = 0; i < obj1.Length; i++)
            {
                var proporcjaTegoWymiaru = obj2[i] / (obj1[i] + obj2[i]);
                sumaProporcjiWszystkichWymiarow += proporcjaTegoWymiaru;
            }
            var uprocentowionaSuma = sumaProporcjiWszystkichWymiarow * 100;
            var iloraz = uprocentowionaSuma / obj1.Length;
            var roznica = iloraz - 50;
            var wartBezwzgledna = Math.Abs(roznica);
            var wynik = 2 * wartBezwzgledna;
            return wynik;
        }
    }
}
