using MyCSetophylax.Dopasowanie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Aktywacja
{
    public class PresjaZaleznaOdCzasu : IPresja
    {
        private readonly double kLambda;
        private readonly Czas czas;
        private readonly SrednieDopasowaniaMrowek srednieDopasowania;

        public PresjaZaleznaOdCzasu(Czas czas, SrednieDopasowaniaMrowek srednieDopasowania, double paramZnaczenieOcen)
        {
            this.czas = czas;
            this.srednieDopasowania = srednieDopasowania;
            kLambda = paramZnaczenieOcen;

        }

        public double OkreslPresje()
        {
            var wplywOcen = OkreslWplywOcen();
            var wplywCzasu = OkreslWplywCzasu();
            return 2.0 + wplywCzasu * wplywOcen;

        }

        private double OkreslWplywOcen()
        {
            return kLambda / srednieDopasowania[czas.Aktualny];
        }

        private double OkreslWplywCzasu()
        {
            double stosunekMaksDoAkt = (double)czas.Maksymalny / (double)czas.Aktualny;
            return Math.Log10(stosunekMaksDoAkt);
        }
    }
}