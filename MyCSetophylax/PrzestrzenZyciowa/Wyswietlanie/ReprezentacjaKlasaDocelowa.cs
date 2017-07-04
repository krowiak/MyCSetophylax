using MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie
{
    public class ReprezentacjaKlasaDocelowa : IReprezentacjaPola
    {
        private readonly Func<Mrowka, string> okreslaczKlasyDocelowej;
        public ReprezentacjaKlasaDocelowa(Func<Mrowka, string> okreslaczKlasyDocelowej) => this.okreslaczKlasyDocelowej = okreslaczKlasyDocelowej;
        public string Okresl(Mrowka mrowka) => Formatuj(okreslaczKlasyDocelowej(mrowka));
        private string Formatuj(string tekst) => tekst.PadLeft(Dlugosc);

        public int Dlugosc
        {
            get;
            set;
        } = 3;
    }
}
