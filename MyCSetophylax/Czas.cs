using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax
{
    public class Czas
    {
        private readonly int maksCzas;

        public Czas(int maksCzas)
        {
            Aktualny = 0;
            this.maksCzas = maksCzas;
        }

        public int Aktualny
        {
            get;
            private set;
        }

        public bool CzyUplynal
        {
            get
            {
                return maksCzas > Aktualny;
            }
        }

        public void Uplywaj()
        {
            Aktualny += 1;
        }
    }
}
