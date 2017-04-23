using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.PrzestrzenZyciowa
{
    public class Sasiedztwo
    {
        private Przestrzen przestrzen;

        public Sasiedztwo(Przestrzen przestrzen, int zasiegX, int zasiegY)
        {
            this.przestrzen = przestrzen;
            ZasiegX = zasiegX;
            ZasiegY = zasiegY;
            RozmiarSasiedztwa = ZasiegX * ZasiegY - 1;
        }

        public int RozmiarSasiedztwa { get; }
        public int ZasiegX { get; }
        public int ZasiegY { get; }

        public IEnumerable<(int X, int Y)> PolaWSasiedztwie(int x, int y)
        {
            var pola = new List<(int, int)>(ZasiegX * ZasiegY - 1);
            for (int iX = (x-ZasiegX); iX <= (x+ZasiegX); iX++)
            {
                for (int iY = (y-ZasiegY); iY <= (y+ZasiegY); iY++)
                {
                    if (iX != x || iY != y)
                    {
                        var modX = PrawdziweModulo(iX, przestrzen.DlugoscBoku);
                        var modY = PrawdziweModulo(iY, przestrzen.DlugoscBoku);
                        pola.Add((modX, modY));
                    }
                }
            }

            return pola;
        }

        /// <summary>
        /// Autorzy biblioteki standardowej w swej nieskończonej mądrości postanowili, że -3 % 10 = -3 zamiast 7.
        /// Ta metoda uważa inaczej.
        /// </summary>
        /// <param name="n">Modulowana liczba.</param>
        /// <param name="modulo">Przez co modulować.</param>
        /// <returns>Sensowny wynik.</returns>
        private int PrawdziweModulo(int n, int modulo)
        {
            return ((n % modulo) + modulo) % modulo;
        }

        public IEnumerable<(int X, int Y)> PustePolaWSasiedztwie(int x, int y)
        {
            return PolaWSasiedztwie(x, y)
                .Where(xY => przestrzen[xY.Y][xY.X] == null);
        }

        public IEnumerable<(int X, int Y)> PolaMrowekWSasiedztwie(int x, int y)
        {
            return PolaWSasiedztwie(x, y)
                .Where(xY => przestrzen[xY.Y][xY.X] != null);
        }

        public IEnumerable<Mrowka> MrowkiWSasiedztwie(int x, int y)
        {
            return PolaWSasiedztwie(x, y)
                .Select(xY => przestrzen[xY.Y][xY.X])
                .Where(mrowka => mrowka != null);
        }
    }
}
