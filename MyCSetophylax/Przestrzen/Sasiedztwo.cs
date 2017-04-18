using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Przestrzen
{
    public class Sasiedztwo
    {
        private Przestrzen przestrzen;
        private int sX;
        private int sY;

        public Sasiedztwo(Przestrzen przestrzen, int zasiegX, int zasiegY)
        {
            this.przestrzen = przestrzen;
            sX = zasiegX;
            sY = zasiegY;
        }

        public IEnumerable<(int X, int Y)> PolaWSasiedztwie(int x, int y)
        {
            var pola = new List<(int, int)>(sX*sY - 1);
            for (int iX = 0; iX < sX; iX++)
            {
                for (int iY = 0; iY < sY; iY++)
                {
                    if (iX != x || iY != y)
                    {
                        pola.Add((iX, iY));
                    }
                }
            }

            return pola;
        }

        public IEnumerable<(int X, int Y)> PustePolaWSasiedztwie(int x, int y)
        {
            return PolaWSasiedztwie(x, y)
                .Where(xY => przestrzen[xY.X][xY.Y] == null);
        }

        public IEnumerable<(int X, int Y)> PolaMrowekWSasiedztwie(int x, int y)
        {
            return PolaWSasiedztwie(x, y)
                .Where(xY => przestrzen[xY.X][xY.Y] != null);
        }

        public IEnumerable<Mrowka> MrowkiWSasiedztwie(int x, int y)
        {
            return PolaWSasiedztwie(x, y)
                .Select(xY => przestrzen[xY.X][xY.Y])
                .Where(mrowka => mrowka != null);
        }
    }
}
