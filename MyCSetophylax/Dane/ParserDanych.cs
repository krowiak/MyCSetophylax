using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Dane
{
    public class ParserDanych
    {
        private readonly Dictionary<(int kolumna, string wartosc), int> wartosciSlow;
        private readonly Dictionary<int, int> maksWartosciSlowKolumn;

        public ParserDanych()
        {
            wartosciSlow = new Dictionary<(int kolumna, string wartosc), int>();
            maksWartosciSlowKolumn = new Dictionary<int, int>();
            Separator = ',';
            Limit = null;
        }

        public char Separator
        {
            get;
            set;
        }

        public bool DaneZawierajaEtykiety
        {
            get;
            set;
        }

        public Dictionary<int, int> OdczytaneEtykiety
        {
            get;
            private set;
        }

        public int? Limit
        {
            get;
            set;
        }

        private int PobierzNowaWartoscSlowa(int kolumna)
        {
            int nowaWartosc;
            if (maksWartosciSlowKolumn.TryGetValue(kolumna, out int staraWartosc))
            {
                nowaWartosc = staraWartosc + 1;
            }
            else
            {
                nowaWartosc = 0;
            }

            maksWartosciSlowKolumn[kolumna] = nowaWartosc;
            return nowaWartosc;
        }

        private int PobierzWartoscSlowa(int kolumna, string slowo)
        {
            int wartosc;
            if (wartosciSlow.TryGetValue((kolumna, slowo), out int znanaWartosc))
            {
                wartosc = znanaWartosc;
            }
            else
            {
                wartosc = PobierzNowaWartoscSlowa(kolumna);
                wartosciSlow[(kolumna, slowo)] = wartosc;
            }
            return wartosc;
        }        

        public IList<double[]> ParsujDane(Stream strumienDanych)
        {
            wartosciSlow.Clear();
            maksWartosciSlowKolumn.Clear();
            OdczytaneEtykiety = DaneZawierajaEtykiety ? new Dictionary<int, int>() : null;

            NumberStyles sposobParsowania = NumberStyles.Float;
            var wyniki = new List<double[]>();

            using (StreamReader czytnik = new StreamReader(strumienDanych))
            {
                int numerKolejny = 0;
                while (!czytnik.EndOfStream)
                {
                    if (numerKolejny >= Limit)
                    {
                        break;
                    }

                    var liniaStr = czytnik.ReadLine();
                    var podzielonaLinia = liniaStr.Split(new[] { Separator });
                    var dlugoscWektora = DaneZawierajaEtykiety ? podzielonaLinia.Length : podzielonaLinia.Length - 1;
                    var wektor = new double[dlugoscWektora];

                    for (int i = 0; i < dlugoscWektora; i++)
                    {
                        var wartosc = podzielonaLinia[i];
                        if (double.TryParse(wartosc, sposobParsowania,
                            CultureInfo.InvariantCulture, out double sparsowanaWartosc))
                        {
                            wektor[i] = sparsowanaWartosc;
                        }
                        else
                        {
                            wektor[i] = PobierzWartoscSlowa(i, wartosc);
                        }
                    }

                    if(DaneZawierajaEtykiety)
                    {
                        int indeksOstKolumny = podzielonaLinia.Length - 1;
                        OdczytaneEtykiety[numerKolejny] = PobierzWartoscSlowa(indeksOstKolumny, podzielonaLinia.Last());
                    }

                    wyniki.Add(wektor);
                    numerKolejny++;
                }
            }

            return wyniki;
        }
    }
}