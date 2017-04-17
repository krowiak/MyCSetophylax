using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.KonkretneOdleglosci
{
    public interface IOdleglosc<T>
    {
        double OkreslOdleglosc(T obj1, T obj2);
    }
}
