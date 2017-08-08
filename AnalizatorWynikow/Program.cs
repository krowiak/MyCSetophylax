using MyCSetophylax;
using MyCSetophylax.Klasy;
using MyCSetophylax.PrzestrzenZyciowa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AnalizatorWynikow
{
    class Program
    {
        static void Main(string[] args)
        {
            bool klasyfikujPoFakcie = false;
            string sciezkaDoFolderu =
                @"E:\Dokumenty\visual studio 2017\Projects\MyCSetophylax\MyCSetophylax\bin\x64\Release\wyniki\ASM_2017\2017-08-07--15-45-10_wińa\ASM2017_A4C_s1";
            var dirInfo = new DirectoryInfo(sciezkaDoFolderu);
            string sciezkaDoWynikow = "wyniki/analiza/ASM2017/";
            if (klasyfikujPoFakcie)
            {
                sciezkaDoWynikow = sciezkaDoWynikow + "pofakcie/";
            }
            string nazwaPlikuWynikowego = $"{dirInfo.Name}.txt";

            var formatter = new BinaryFormatter();
            var plikiWynikow = dirInfo.EnumerateFiles("*.a4c");

            int liczbaMrowek = 0;
            List<int> bledyPolaczeniaWIteracjach = new List<int>();
            List<int> mrowkiWBledziePodzieleniaWIteracjach = new List<int>();
            List<int> bledyPodzieleniaWIteracjach = new List<int>();
            List<long> czasyTrwaniaIteracji = new List<long>();
            foreach (FileInfo info in plikiWynikow)
            {
                using (var strumien = info.OpenRead())
                {
                    WynikAlgorytmu wynik = (WynikAlgorytmu)formatter.Deserialize(strumien);
                    liczbaMrowek = wynik.Mrowki.Count;

                    if (klasyfikujPoFakcie)
                    {
                        KlasyfikujPoFakcie(wynik.Przestrzen, wynik.Mrowki, wynik.S_x, wynik.S_y);
                    }

                    var klasyDocelowe = wynik.SlownikKlasDocelowych.Values
                        .GroupBy(klasa => klasa)
                        .ToList();
                    var liczbaMrowekWKazdejKlasieDocelowej = klasyDocelowe
                        .Select(grupa => new { Klucz = grupa.Key, Liczba = grupa.Count() })
                        .ToDictionary(liczbaKlasy => liczbaKlasy.Klucz, liczbaKlasy => liczbaKlasy.Liczba);
                    var grupyMrowek = wynik.Mrowki.GroupBy(mrowka => mrowka.Klasa).ToList();

                    var globRozmiarNajwiekszejGrupyKlasyDocelowej = new int[klasyDocelowe.Count];
                    var globLiczbaGrupKlasyDocelowej = new int[klasyDocelowe.Count];
                    int liczbaBledowPolaczenia = 0;
                    int liczbaMrowekWBledziePodzielenia = 0;
                    foreach (var grupa in grupyMrowek)
                    {
                        var lookupGrupDocelowych = grupa.ToLookup(mrowka => wynik.SlownikKlasDocelowych[mrowka.Id]);
                        var uporzadkowaneGrupyDocelowe = lookupGrupDocelowych.OrderByDescending(grupaDocelowa => grupaDocelowa.Count());
                        var lokalNajwiekszaGrupaDocelowa = uporzadkowaneGrupyDocelowe.First();  // nie wiem, czy trzeba się przejmować, ale co gdy np A.Count()=50, B.Count()=50?
                        var przyjetaKlasaTejGrupy = lokalNajwiekszaGrupaDocelowa.Key;  // tzn. mrówki tej klasy nie są traktowane jako błąd połączenia, bo ich najwięcej
                        var licznoscMrowekPrzyjetejKlasy = lokalNajwiekszaGrupaDocelowa.Count();

                        var lokalLiczbaBledowPolaczenia = uporzadkowaneGrupyDocelowe.Skip(1)
                            .Sum(grupaInnejKlasyDocelowej => grupaInnejKlasyDocelowej.Count());
                        liczbaBledowPolaczenia += lokalLiczbaBledowPolaczenia;

                        globLiczbaGrupKlasyDocelowej[przyjetaKlasaTejGrupy]++;

                        if (globLiczbaGrupKlasyDocelowej[przyjetaKlasaTejGrupy] > 1)
                        {
                            var dotychczasowyMaksRozmiarGrupyTejKlasyDocelowej = globRozmiarNajwiekszejGrupyKlasyDocelowej[przyjetaKlasaTejGrupy];
                            if (dotychczasowyMaksRozmiarGrupyTejKlasyDocelowej < licznoscMrowekPrzyjetejKlasy)
                            {
                                liczbaMrowekWBledziePodzielenia += dotychczasowyMaksRozmiarGrupyTejKlasyDocelowej;
                                globRozmiarNajwiekszejGrupyKlasyDocelowej[przyjetaKlasaTejGrupy] = licznoscMrowekPrzyjetejKlasy;
                            }
                            else
                            {
                                liczbaMrowekWBledziePodzielenia += licznoscMrowekPrzyjetejKlasy;
                            }
                        }
                        else
                        {
                            globRozmiarNajwiekszejGrupyKlasyDocelowej[przyjetaKlasaTejGrupy] = licznoscMrowekPrzyjetejKlasy;
                            // To była pierwsza grupa tej klasy docelowej, więc błędów podziału nie ma/jeszcze nie sposób wykryć
                        }
                    }

                    bledyPolaczeniaWIteracjach.Add(liczbaBledowPolaczenia);
                    bledyPodzieleniaWIteracjach.Add(globLiczbaGrupKlasyDocelowej.Where(wart => wart > 0).Select(wart => wart - 1).Sum());
                    mrowkiWBledziePodzieleniaWIteracjach.Add(liczbaMrowekWBledziePodzielenia);
                    czasyTrwaniaIteracji.Add(wynik.CzasTrwaniaMs);
                }
            }

            var minBledowPolaczenia = bledyPolaczeniaWIteracjach.Min();
            var maxBledowPolaczenia = bledyPolaczeniaWIteracjach.Max();
            var srednioBledowPolaczenia = bledyPolaczeniaWIteracjach.Average();

            var minBledowPodzielenia = bledyPodzieleniaWIteracjach.Min();
            var maxBledowPodzielenia = bledyPodzieleniaWIteracjach.Max();
            var srednioBledowPodzielenia = bledyPodzieleniaWIteracjach.Average();

            var minMrowekWBledziePodzielenia = mrowkiWBledziePodzieleniaWIteracjach.Min();
            var maxMrowekWBledziePodzielenia = mrowkiWBledziePodzieleniaWIteracjach.Max();
            var srednioMrowekWBledziePodzielenia = mrowkiWBledziePodzieleniaWIteracjach.Average();

            var sumyBledow = Enumerable.Range(0, plikiWynikow.Count())
                .Select(i => bledyPolaczeniaWIteracjach[i] + mrowkiWBledziePodzieleniaWIteracjach[i])
                .ToList();
            var minBledow = sumyBledow.Min();
            var maxBledow = sumyBledow.Max();
            var srednioBledowMrowek = sumyBledow.Average();

            var minCzasTrwania = czasyTrwaniaIteracji.Min();
            var maxCzasTrwania = czasyTrwaniaIteracji.Max();
            var srednioCzasTrwania = czasyTrwaniaIteracji.Average();

            Directory.CreateDirectory(sciezkaDoWynikow);
            using (var strumien = File.Create(sciezkaDoWynikow + nazwaPlikuWynikowego))
            using (var pisarz = new StreamWriter(strumien))
            {
                pisarz.WriteLine($"Błędów połączenia:");
                pisarz.WriteLine($"\tMin: {minBledowPolaczenia} - iteracja {bledyPolaczeniaWIteracjach.IndexOf(minBledowPolaczenia)}");
                pisarz.WriteLine($"\tŚrednio: {srednioBledowPolaczenia}");
                pisarz.WriteLine($"\tMax: {maxBledowPolaczenia} - iteracja {bledyPolaczeniaWIteracjach.IndexOf(maxBledowPolaczenia)}");
                pisarz.WriteLine();

                pisarz.WriteLine($"Błędów podzielenia:");
                pisarz.WriteLine($"\tMin: {minBledowPodzielenia} - iteracja {bledyPodzieleniaWIteracjach.IndexOf(minBledowPodzielenia)}");
                pisarz.WriteLine($"\tŚrednio: {srednioBledowPodzielenia}");
                pisarz.WriteLine($"\tMax: {maxBledowPodzielenia} - iteracja {bledyPodzieleniaWIteracjach.IndexOf(maxBledowPodzielenia)}");
                pisarz.WriteLine();

                pisarz.WriteLine($"Mrówek w błędach podzielenia:");
                pisarz.WriteLine($"\tMin: {minMrowekWBledziePodzielenia} - iteracja {mrowkiWBledziePodzieleniaWIteracjach.IndexOf(minMrowekWBledziePodzielenia)}");
                pisarz.WriteLine($"\tŚrednio: {srednioMrowekWBledziePodzielenia}");
                pisarz.WriteLine($"\tMax: {maxMrowekWBledziePodzielenia} - iteracja {mrowkiWBledziePodzieleniaWIteracjach.IndexOf(maxMrowekWBledziePodzielenia)}");
                pisarz.WriteLine();

                pisarz.WriteLine($"Błędów mrówek łącznie:");
                pisarz.WriteLine($"\tMin: {minBledow} ({(double)minBledow * 100 / liczbaMrowek}%) - iteracja {sumyBledow.IndexOf(minBledow)}");
                pisarz.WriteLine($"\tŚrednio: {srednioBledowMrowek} ({srednioBledowMrowek * 100 / liczbaMrowek}%)");
                pisarz.WriteLine($"\tMax: {maxBledow} ({(double)maxBledow * 100 / liczbaMrowek}%) - iteracja {sumyBledow.IndexOf(maxBledow)}");
                pisarz.WriteLine();

                pisarz.WriteLine($"Czas trwania:");
                pisarz.WriteLine($"\tMin: {minCzasTrwania}ms - iteracja {czasyTrwaniaIteracji.IndexOf(minCzasTrwania)}");
                pisarz.WriteLine($"\tŚrednio: {srednioCzasTrwania}ms");
                pisarz.WriteLine($"\tMax: {maxCzasTrwania}ms - iteracja {czasyTrwaniaIteracji.IndexOf(maxCzasTrwania)}");
                pisarz.WriteLine();

                pisarz.WriteLine($"Liczba mrówek: {liczbaMrowek}.");
                pisarz.Flush();
            }
        }

        private static void KlasyfikujPoFakcie(Przestrzen przestrzen, IEnumerable<Mrowka> mrowki, int sX, int sY)
        {
            Sasiedztwo sasiedztwo = new Sasiedztwo(przestrzen, sX, sY);
            OkreslaczKlas okreslacz = new OkreslaczKlas(sasiedztwo);
            Dictionary<Mrowka, int> noweKlasyMrowek = new Dictionary<Mrowka, int>();

            for (int i = 0; i < 100; i++)
            {
                for (int x = 0; x < przestrzen.DlugoscBoku; x++)
                {
                    for (int y = 0; y < przestrzen.DlugoscBoku; y++)
                    {
                        var mrowka = przestrzen[y][x];
                        if (mrowka is Mrowka m)
                        {
                            var nowaKlasa = okreslacz.OkreslKlase(mrowka, (x, y));
                            noweKlasyMrowek[mrowka] = nowaKlasa;
                        }
                    }
                }

                foreach (var mrowkaIKlasa in noweKlasyMrowek)
                {
                    var mrowka = mrowkaIKlasa.Key;
                    var klasa = mrowkaIKlasa.Value;
                    mrowka.Klasa = klasa;
                }
            }

            //MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie.ReprezentacjaKlasaMrowki a = new MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie.ReprezentacjaKlasaMrowki();
            //MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie.WyswietlaczPrzestrzeni b = new MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie.WyswietlaczPrzestrzeni(a);
            //b.Wyswietl(przestrzen);
        }
    }
}
