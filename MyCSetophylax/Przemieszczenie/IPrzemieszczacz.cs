using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Przemieszczenie
{
    public interface IPrzemieszczacz
    {
        void Przemiesc(Mrowka mrowka, (int x, int y) pozycjaMrowki);
    }
}
