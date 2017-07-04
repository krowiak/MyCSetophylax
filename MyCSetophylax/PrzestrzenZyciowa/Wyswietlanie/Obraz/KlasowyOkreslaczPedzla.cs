using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie.Obraz
{
    public class KlasowyOkreslaczPedzla : IOkreslaczPedzla
    {
        private readonly int wartDomyslna;

        public KlasowyOkreslaczPedzla(int wartDomyslna)
        {
            this.wartDomyslna = wartDomyslna;
        }

        public int ZwrocBazeDoOkresleniaPedzla(Mrowka mrowkaDoNamalowania)
        {
            return mrowkaDoNamalowania != null ?
                mrowkaDoNamalowania.Klasa : 
                wartDomyslna;
        }
    }
}
