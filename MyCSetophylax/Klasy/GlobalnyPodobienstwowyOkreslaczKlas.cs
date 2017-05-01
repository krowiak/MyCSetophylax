using MyCSetophylax.KonkretneOdleglosci;
using MyCSetophylax.PrzestrzenZyciowa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Klasy
{
    public class GlobalnyPodobienstwowyOkreslaczKlas : IOkreslaczKlas
    {
        private readonly IOdleglosc<Mrowka> odleglosc;
        private readonly Sasiedztwo sasiedztwo;
        private readonly Czas czas;
        private Dictionary<int, List<Mrowka>> mrowkiKlasPopIter;
        private Dictionary<int, List<Mrowka>> mrowkiKlasAktIter;
        private int popIteracja;

        public GlobalnyPodobienstwowyOkreslaczKlas(IOdleglosc<Mrowka> odleglosc, Sasiedztwo sasiedztwo, Czas czas)
        {
            this.odleglosc = odleglosc;
            this.sasiedztwo = sasiedztwo;
            this.czas = czas;
            popIteracja = 0;
            mrowkiKlasAktIter = new Dictionary<int, List<Mrowka>>();
        }

        public int MinProgLicznosci
        {
            get;
            set;
        } = 5;

        public int OkreslKlase(Mrowka mrowka, (int x, int y) pozycjaMrowki)
        {
            if (czas.Aktualny > popIteracja)
            {
                mrowkiKlasPopIter = mrowkiKlasAktIter;
                mrowkiKlasAktIter = new Dictionary<int, List<Mrowka>>();
                popIteracja = czas.Aktualny;
            }

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
                    if (satysfakcjonujacoLiczneGrupy.Length == 1)
                    {
                        var jedynaOpcjaKlasy = satysfakcjonujacoLiczneGrupy[0].Key;
                        DodajDoAkt(mrowka, jedynaOpcjaKlasy);
                        return jedynaOpcjaKlasy;
                    }
                    else
                    {
                        int najlepszaKlasa = satysfakcjonujacoLiczneGrupy[0].Key;
                        double odlNajlepszejKlasy = double.MaxValue;
                        foreach (var grupa in satysfakcjonujacoLiczneGrupy)
                        {
                            var klasaGrupy = grupa.Key;
                            var odlegloscGrupy = ObliczSredniaOdlegloscDlaPopIter(mrowka, klasaGrupy);
                            if (odlNajlepszejKlasy > odlegloscGrupy)
                            {
                                najlepszaKlasa = klasaGrupy;
                                odlNajlepszejKlasy = odlegloscGrupy;
                            }
                        }
                        DodajDoAkt(mrowka, najlepszaKlasa);
                        return najlepszaKlasa;
                    }

                }
                else
                {
                    var najliczniejszaKlasa = grupyPoKlasach.First().Key;
                    DodajDoAkt(mrowka, najliczniejszaKlasa);
                    return najliczniejszaKlasa;
                }
            }
            else
            {
                DodajDoAkt(mrowka, mrowka.Klasa);
                return mrowka.Klasa;
            }
        }

        private void DodajDoAkt(Mrowka mrowka, int klasa)
        {
            if (!mrowkiKlasAktIter.TryGetValue(klasa, out var listaKlasy))
            {
                listaKlasy = new List<Mrowka>();
                mrowkiKlasAktIter[klasa] = listaKlasy;
            }
            listaKlasy.Add(mrowka);
        }

        private double ObliczSredniaOdlegloscDlaPopIter(Mrowka mrowka, int klasa)
        {
            // Niemożliwe, żeby lista była pusta/nullowa/klucz nie istniał/inne szaleństwo:
            // - w pierwszej iteracji wszystkie grupy będą miały liczność 1 i tu nic nie wejdzie
            // - w dalszych brany słownik z poprzedniej, który ma niepustą listę dla każdej klasy, która wtedy istniała
            // - raz utracona klasa nigdy nie powraca
            IEnumerable<Mrowka> listaKlasy = mrowkiKlasPopIter[klasa];
            if (mrowka.Klasa == klasa)
            {
                listaKlasy = listaKlasy.Where(innaMrowka => innaMrowka.Id != mrowka.Id);
            }
            return listaKlasy.Average(innaMrowka => odleglosc.OkreslOdleglosc(mrowka, innaMrowka));
        }
    }
}
