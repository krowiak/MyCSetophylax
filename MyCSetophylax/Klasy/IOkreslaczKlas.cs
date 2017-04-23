using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Klasy
{
    public interface IOkreslaczKlas
    {
        int OkreslKlase(Mrowka mrowka, (int x, int y) pozycjaMrowki);
    }
}
