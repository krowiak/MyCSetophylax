using MyCSetophylax.KonkretneOdleglosci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.SrednieOdleglosci
{
    public class NajsredniejszaOdleglosc : ISrednieOdleglosci
    {
        private readonly double sredniaOdleglosc;

        public NajsredniejszaOdleglosc(IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> odleglosci)
        {
            sredniaOdleglosc = obliczSredniaOdleglosc(mrowki, odleglosci);
        }

        private double obliczSredniaOdleglosc(IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> odleglosci)
        {
            var uporzadkowaneMrowki = mrowki.OrderBy(mrowka => mrowka.Id).ToList();
            
            double sumaOdleglosci = 0.0;
            for (int i = 0; i < uporzadkowaneMrowki.Count; i++)
            {
                var m1 = uporzadkowaneMrowki[i];
                for (int j = i + 1; j < uporzadkowaneMrowki.Count; j++)
                {
                    var m2 = uporzadkowaneMrowki[j];
                    sumaOdleglosci += odleglosci.OkreslOdleglosc(m1, m2);
                }
            }

            int liczbaOdleglosci = (uporzadkowaneMrowki.Count * (uporzadkowaneMrowki.Count - 1)) / 2;
            double sredniaOdleglosci = sumaOdleglosci / liczbaOdleglosci;
            return sredniaOdleglosci;
        }

        private double OkreslOdleglosc(double[] obj1, double[] obj2)
        {
            double odleglosc = 0.0;

            for (int i = 0; i < obj1.Length; i++)
            {
                double roznica = obj1[i] - obj2[i];
                odleglosc += roznica * roznica;
            }

            return Math.Sqrt(odleglosc);
        }

        public double OkreslSredniaOdleglosc(Mrowka sprawdzanaMrowka)
        {
            return sredniaOdleglosc;
        }
    }
}
