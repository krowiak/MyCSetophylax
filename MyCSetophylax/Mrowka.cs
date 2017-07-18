using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax
{
    [Serializable]
    public class Mrowka
    {
        public Mrowka(int id, double[] dane)
        {
            Id = id;
            Dane = dane;
            Klasa = Id;
        }

        public int Id
        {
            get;
        }

        public double[] Dane
        {
            get;
        }

        public int Klasa
        {
            get;
            set;
        }
    }
}
