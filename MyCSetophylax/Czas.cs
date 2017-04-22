using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax
{
    public class Czas
    {
        public Czas(int maksCzas)
        {
            Aktualny = 0;
            Maksymalny = maksCzas;
        }

        public int Aktualny
        {
            get;
            private set;
        }

        public int Maksymalny
        {
            get;
            private set;
        }

        public bool CzyUplynal
        {
            get
            {
                return Maksymalny > Aktualny;
            }
        }

        public void Uplywaj()
        {
            Aktualny += 1;
        }
    }
}
