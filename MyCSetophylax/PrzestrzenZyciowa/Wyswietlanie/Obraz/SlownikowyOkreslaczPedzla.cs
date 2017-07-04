using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie.Obraz
{
    public class SlownikowyOkreslaczPedzla : IOkreslaczPedzla
    {
        private readonly Dictionary<int, int> slownik;
        private readonly int wartDomyslnaBrak;
        private readonly int wartDomyslnaNull;
        private readonly Func<Mrowka, int> okreslaczKlucza;

        public SlownikowyOkreslaczPedzla(Dictionary<int, int> slownik, Func<Mrowka, int> okreslaczKlucza, int wartDomyslnaNull, int wartDomyslnaBrak)
        {
            this.slownik = slownik;
            this.okreslaczKlucza = okreslaczKlucza;
            this.wartDomyslnaNull = wartDomyslnaNull;
            this.wartDomyslnaBrak = wartDomyslnaBrak;
        }

        public SlownikowyOkreslaczPedzla(Dictionary<int, int> slownik, Func<Mrowka, int> okreslaczKlucza, int wartDomyslna)
            : this(slownik, okreslaczKlucza, wartDomyslna, wartDomyslna)
        {
        }

        public int ZwrocBazeDoOkresleniaPedzla(Mrowka mrowkaDoNamalowania)
        {
            if (mrowkaDoNamalowania is null) return wartDomyslnaNull;
            if (slownik.TryGetValue(okreslaczKlucza(mrowkaDoNamalowania), out int baza)) return baza;
            return wartDomyslnaBrak;
        }
    }
}
