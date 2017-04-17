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
                for (int j = 0; j < uporzadkowaneMrowki.Count; j++)
                {
                    if (i != j)
                    {
                        var m1 = uporzadkowaneMrowki[i];
                        var m2 = uporzadkowaneMrowki[j];
                        sumaOdleglosci += odleglosci.OkreslOdleglosc(m1, m2);
                    }
                }
            }

            int liczbaOdleglosci = uporzadkowaneMrowki.Count * (uporzadkowaneMrowki.Count - 1);
            double sredniaOdleglosci = sumaOdleglosci / liczbaOdleglosci;
            return sredniaOdleglosci;
        }

        public double OkreslSredniaOdleglosc(Mrowka sprawdzanaMrowka)
        {
            return sredniaOdleglosc;
        }
    }
}
