using MyCSetophylax.Aktywacja;
using MyCSetophylax.Dopasowanie;
using MyCSetophylax.PrzestrzenZyciowa;
using MyCSetophylax.SrednieOdleglosci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax
{
    public class KonfiguracjaGrupowania
    {
        public Random MaszynaLosujaca { get; set; }
        public int LiczbaIteracji { get; set; }
        public IAktywator Aktywator { get; set; }
        public ISrednieOdleglosci SrednieOdleglosci { get; set; }
        public IOceniacz Oceniacz { get; set; }
        public SrednieDopasowaniaMrowek SrednieDopasowania { get; set; }
        public Przestrzen Przestrzen { get; set; }
        public Sasiedztwo Sasiedztwo { get; set; }
        public Czas Czas { get; set; }
    }
}
