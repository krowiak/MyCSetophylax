using MyCSetophylax.KonkretneOdleglosci;
using MyCSetophylax.PrzestrzenZyciowa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Klasy
{
    public class PodobienstwowyOkreslaczKlasPlus : IOkreslaczKlas
    {
        private readonly IOdleglosc<Mrowka> odleglosci;
        private readonly Sasiedztwo sasiedztwo;

        public PodobienstwowyOkreslaczKlasPlus(IOdleglosc<Mrowka> odleglosciPomiedzyMrowkami, Sasiedztwo sasiedztwo)
        {
            odleglosci = odleglosciPomiedzyMrowkami;
            this.sasiedztwo = sasiedztwo;
        }

        public int MinProgLicznosci
        {
            get;
            set;
        } = 5;

        public int MaksRoznicaRozmiaru
        {
            get;
            set;
        } = 1000;

        public int OkreslKlase(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            var (x, y) = pozycjaMrowki;
            var mrowkiWSasiedztwie = sasiedztwo.MrowkiWSasiedztwie(x, y).ToList();
            if (mrowkiWSasiedztwie.Any())
            {
                var grupyPoKlasach = mrowkiWSasiedztwie
                    .GroupBy(innaMrowka => innaMrowka.Klasa)
                    .OrderByDescending(grupa => grupa.Count())
                    .ToList();
                var satysfakcjonujacoLiczneGrupy = grupyPoKlasach
                    .Where(grupa => grupa.Count() > MinProgLicznosci)
                    .ToArray();
                if (satysfakcjonujacoLiczneGrupy.Any())
                {
                    List<IGrouping<int, Mrowka>> doZbadania = new List<IGrouping<int, Mrowka>>();
                    doZbadania.Add(satysfakcjonujacoLiczneGrupy.First());
                    for (int i = 1; i < satysfakcjonujacoLiczneGrupy.Length; i++)
                    {
                        var aktGrupa = satysfakcjonujacoLiczneGrupy[i];
                        int rozmiarPop = satysfakcjonujacoLiczneGrupy[i - 1].Count();
                        int rozmiarAkt = aktGrupa.Count();
                        if (rozmiarPop - rozmiarAkt < MaksRoznicaRozmiaru)
                        {
                            doZbadania.Add(aktGrupa);
                        }
                        else
                        {
                            break;
                        }
                    }

                    return doZbadania.OrderByDescending(
                        klasa => klasa.Average(innaMrowka => odleglosci.OkreslOdleglosc(mrowka, innaMrowka))
                    ).First().Key;

                }
                else
                {
                    return grupyPoKlasach.First().Key;
                }
            }
            else
            {
                return mrowka.Klasa;
            }
        }
    }
}
