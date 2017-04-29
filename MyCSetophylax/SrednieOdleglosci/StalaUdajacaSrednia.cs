using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.SrednieOdleglosci
{
    public class StalaUdajacaSrednia : ISrednieOdleglosci
    {
        public StalaUdajacaSrednia(double stala) => WartoscStalej = stala;
        public double WartoscStalej { get; }
        public double OkreslSredniaOdleglosc(Mrowka sprawdzanaMrowka) => WartoscStalej;
    }
}
