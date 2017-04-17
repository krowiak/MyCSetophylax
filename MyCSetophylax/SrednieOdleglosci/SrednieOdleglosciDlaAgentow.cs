using MyCSetophylax.KonkretneOdleglosci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.SrednieOdleglosci
{
    public class SrednieOdleglosciDlaAgentow : ISrednieOdleglosci
    {
        private readonly Dictionary<int, double> srednieOdleglosci;

        public SrednieOdleglosciDlaAgentow(IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> odleglosci)
        {
            srednieOdleglosci = tworzSlownikSrednichOdleglosci(mrowki, odleglosci);
        }

        private Dictionary<int, double> tworzSlownikSrednichOdleglosci(IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> odleglosci)
        {
            var slownik = new Dictionary<int, double>();

            var listaMrowek = mrowki.ToList();
            for (int i = 0; i < listaMrowek.Count; i++)
            {
                var m1 = listaMrowek[i];
                double sumaOdleglosciMrowki = 0.0;
                for (int j = 0; j < listaMrowek.Count; j++)
                {
                    if (i != j)
                    {
                        var m2 = listaMrowek[i + 1];
                        sumaOdleglosciMrowki += odleglosci.OkreslOdleglosc(m1, m2);
                    }
                }
                var sredniaOdleglosciMrowki = sumaOdleglosciMrowki / listaMrowek.Count - 1;
                slownik[m1.Id] = sredniaOdleglosciMrowki;
            }

            return slownik;
        }

        public double OkreslSredniaOdleglosc(Mrowka sprawdzanaMrowka)
        {
            return srednieOdleglosci[sprawdzanaMrowka.Id];
        }
    }
}