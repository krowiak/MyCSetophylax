using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax
{
    public class SrednieDopasowaniaMrowek
    {
        private Dictionary<int, double> dopasowanieOdCzasu;

        public SrednieDopasowaniaMrowek()
        {
            dopasowanieOdCzasu = new Dictionary<int, double>();
        }

        public double this[int czas]
        {
            get
            {
                return dopasowanieOdCzasu[czas];
            }
            set
            {
                dopasowanieOdCzasu[czas] = value;
            }
        }
    }
}
