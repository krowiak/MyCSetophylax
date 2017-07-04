using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie
{
    public class ReprezentacjaIdMrowki : IReprezentacjaPola
    {
        public int Dlugosc
        {
            get;
            set;
        } = 3;

        public string Okresl(Mrowka mrowka)
        {
            if (mrowka != null)
            {
                return Formatuj(mrowka.Id.ToString());
            }
            else
            {
                return Formatuj(String.Empty);
            }
        }

        private string Formatuj(string tekst)
        {
            return tekst.PadLeft(Dlugosc);
        }
    }
}
