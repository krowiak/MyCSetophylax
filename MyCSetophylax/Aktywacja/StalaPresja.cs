using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Aktywacja
{
    public class StalaPresja : IPresja
    {
        public StalaPresja(double presja = 2.0)
        {
            Wartosc = presja;
        }

        public double Wartosc
        {
            get;
            private set;
        }

        public double OkreslPresje()
        {
            return Wartosc;
        }
    }
}
