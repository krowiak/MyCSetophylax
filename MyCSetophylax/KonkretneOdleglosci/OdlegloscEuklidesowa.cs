using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.KonkretneOdleglosci
{
    public class OdlegloscEuklidesowa : IOdleglosc<double[]>
    {
        public double OkreslOdleglosc(double[] obj1, double[] obj2)
        {
            double odleglosc = 0.0;

            for (int i = 0; i < obj1.Length; i++)
            {
                double roznica = obj1[i] - obj2[i];
                odleglosc += roznica * roznica;
            }

            return Math.Sqrt(odleglosc);
        }
    }
}