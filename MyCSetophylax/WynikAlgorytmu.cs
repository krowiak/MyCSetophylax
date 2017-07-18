using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax
{
    [Serializable]
    public class WynikAlgorytmu
    {
        public PrzestrzenZyciowa.Przestrzen Przestrzen
        {
            get;
            set;
        }

        public int S_x
        {
            get;
            set;
        }

        public int S_y
        {
            get;
            set;
        }

        public List<Mrowka> Mrowki
        {
            get;
            set;
        }

        public Dictionary<int, int> SlownikKlasDocelowych
        {
            get;
            set;
        }

        public long CzasTrwaniaMs
        {
            get;
            set;
        }
    }
}
