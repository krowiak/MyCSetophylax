using MyCSetophylax.PrzestrzenZyciowa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Klasy
{
    public class MniejZmiennyOkreslaczKlas : IOkreslaczKlas
    {
        private readonly Sasiedztwo sasiedztwo;

        public MniejZmiennyOkreslaczKlas(Sasiedztwo sasiedztwo)
        {
            this.sasiedztwo = sasiedztwo;
        }

        public int OkreslKlase(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            var klasaDocelowa = mrowka.Klasa;
            (int x, int y) = pozycjaMrowki;
            var sasiadki = sasiedztwo.MrowkiWSasiedztwie(x, y).ToList();
            if (sasiadki.Count > 0)
            {
                var grupyPoKlasach = sasiadki.GroupBy(sasiadka => sasiadka.Klasa)
                    .OrderBy(grupa => grupa.Count());
                var grupaAktKlasy = grupyPoKlasach.FirstOrDefault(grupa => grupa.Key == mrowka.Klasa);
                if (grupaAktKlasy != null)
                {
                    var liczebnoscAktKlasy = grupaAktKlasy.Count() + 1;  // Sprawdzana mrówka nie jest częścią sąsiedztwa, a tu dobrze by było ją uwzględnić
                    var najwiekszaKlasa = grupyPoKlasach.First();
                    var liczebnoscNajwiekszejKlasy = najwiekszaKlasa.Count();
                    if (liczebnoscNajwiekszejKlasy > liczebnoscAktKlasy)
                    {
                        klasaDocelowa = najwiekszaKlasa.Key;
                    }
                }
            }
            return klasaDocelowa;
        }
    }
}
