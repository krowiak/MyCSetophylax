using MyCSetophylax.KonkretneOdleglosci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.SrednieOdleglosci
{
    public class KonfiguracjaSredniejOdlOdCzasu
    {
        public int IleJednostekCzasuSpogladacWstecz
        {
            get;
            set;
        }

        public ISrednieOdleglosci SposobOkreslaniaWartosciPrzedUaktywnieniem
        {
            get;
            set;
        }

        public double StopienWplywuRoznicySrednichNaWynikWDanejJednostceCzasu
        {
            get;
            set;
        }
    }
}
