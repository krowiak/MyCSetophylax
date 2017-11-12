using MyCSetophylax;
using MyCSetophylax.Dopasowanie;
using MyCSetophylax.KonkretneOdleglosci;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DalszeAnalizatrony
{
    class Program
    {
        static void Main(string[] args)
        {
            string sciezkaDoFolderu =
                @"E:\Dokumenty\visual studio 2017\Projects\MyCSetophylax\MyCSetophylax\bin\x64\Release\wyniki\ASM_2017\2017-08-07--15-45-10_wińa\ASM2017_A4C";
            var dirInfo = new DirectoryInfo(sciezkaDoFolderu);
            string sciezkaDoWynikow = "wyniki/boczykanaliza/wine_s2/";
            string nazwaPlikuWynikowego = $"{dirInfo.Name}.txt";

            var formatter = new BinaryFormatter();
            var plikiWynikow = dirInfo.EnumerateFiles("*.a4c");

            var bledyKwadratowe = new List<double>();
            var indeksyDunna = new List<double>();
            foreach (FileInfo info in plikiWynikow)
            {
                using (var strumien = info.OpenRead())
                {
                    var wynik = (WynikAlgorytmu)formatter.Deserialize(strumien);
                    var mrowki = wynik.Mrowki.ToList();
                    //x1 = (0, 2), x2 = (0, 0), x3 = (1.5, 0), x4 = (5, 0), x5 = (5, 2)
                    //C1 = {x1, x2, x4} i C2 = {x3, x5}
                    //var mrowki = new List<Mrowka>() {
                    //    new Mrowka(1, new double[] { 0, 2 }) { Klasa = 1 },
                    //    new Mrowka(2, new double[] { 0, 0 }) { Klasa = 1 },
                    //    new Mrowka(3, new double[] { 1.5, 0 }) { Klasa = 2 },
                    //    new Mrowka(4, new double[] { 5, 0 }) { Klasa = 1 },
                    //    new Mrowka(5, new double[] { 5, 2 }) { Klasa = 2 }
                    //};

                    //C1' = {x1, x2, x3} i C2' = {x4, x5}
                    //var mrowki = new List<Mrowka>() {
                    //    new Mrowka(1, new double[] { 0, 2 }) { Klasa = 1 },
                    //    new Mrowka(2, new double[] { 0, 0 }) { Klasa = 1 },
                    //    new Mrowka(3, new double[] { 1.5, 0 }) { Klasa = 1 },
                    //    new Mrowka(4, new double[] { 5, 0 }) { Klasa = 2 },
                    //    new Mrowka(5, new double[] { 5, 2 }) { Klasa = 2 }
                    //};
                    var bladKwadratowy = ObliczLacznyBladKwadratowy(mrowki);
                    var indeksDunna = ObliczIndeksDunna(mrowki);

                    bledyKwadratowe.Add(bladKwadratowy);
                    indeksyDunna.Add(indeksDunna);
                }
            }

            var minBladKwadratowy = bledyKwadratowe.Min();
            var avgBladKwadratowy = bledyKwadratowe.Average();
            var maxBladKwadratowy = bledyKwadratowe.Max();

            var minDeksDunna = indeksyDunna.Min();
            var minDeksDunnaAleMniejMin = indeksyDunna.Where(idu => idu != 0).DefaultIfEmpty(0).Min();
            var avgIndeksDunna = indeksyDunna.Average();
            var maxIndeksDunna = indeksyDunna.Max();
            var maxIndeksDunnaAleDrugiNaWypadekJakbyPierwszyWynikalZZeraGdzies = indeksyDunna.Where(idu => idu != maxIndeksDunna).DefaultIfEmpty(double.MaxValue).Max();

            Directory.CreateDirectory(sciezkaDoWynikow);
            using (var strumien = File.Create(sciezkaDoWynikow + nazwaPlikuWynikowego))
            using (var pisarz = new StreamWriter(strumien))
            {
                pisarz.WriteLine($"Błędy kwadratowe:");
                pisarz.WriteLine($"\tMin: {minBladKwadratowy}");
                pisarz.WriteLine($"\tŚrednio: {avgBladKwadratowy}");
                pisarz.WriteLine($"\tMax: {maxBladKwadratowy}");
                pisarz.WriteLine();

                pisarz.WriteLine($"Indeksy Dunna:");
                pisarz.WriteLine($"\tMin: {minDeksDunna}");
                pisarz.WriteLine($"\tMin, ale mniej: {minDeksDunnaAleMniejMin}");
                pisarz.WriteLine($"\tŚrednio: {avgIndeksDunna}");
                pisarz.WriteLine($"\tMax: {maxIndeksDunna}");
                pisarz.WriteLine($"\tMax na zapas: {maxIndeksDunnaAleDrugiNaWypadekJakbyPierwszyWynikalZZeraGdzies}");
                pisarz.WriteLine();

                pisarz.WriteLine($"Polecam i pozdrawiam");
                pisarz.WriteLine($"Piotr Fronczewski.");
                pisarz.Flush();
            }
        }

        private static Mrowka OkreslCentrumKlastra(int idKlastra, IEnumerable<Mrowka> klaster)
        {
            var liczbaMroffek = klaster.Count();
            var dlugoscWektora = klaster.First().Dane.Length;
            var wektorDoSumowania = new double[dlugoscWektora];
            foreach (var wektor in klaster.Select(mrowka => mrowka.Dane))
            {
                for (int i = 0; i < dlugoscWektora; i++)
                {
                    wektorDoSumowania[i] += wektor[i] / liczbaMroffek;
                }
            }

            //var liczbaMroffek = klaster.Count();
            //for (int i = 0; i < dlugoscWektora; i++)
            //{
            //    wektorDoSumowania[i] /= liczbaMroffek;
            //}

            return new Mrowka(-idKlastra, wektorDoSumowania);
        }

        private static double ObliczBladKwadratowyPomiedzy(Mrowka centrum, Mrowka faktyczna)
        {
            var dlugoscMrowkiWMm = centrum.Dane.Length;
            var blad = 0.0;
            for (int i = 0; i < dlugoscMrowkiWMm; i++)
            {
                var bladDlaWymiaru = Math.Pow(faktyczna.Dane[i] - centrum.Dane[i], 2);
                blad += bladDlaWymiaru;
            }

            return blad;
        }

        private static double ObliczBladKwadratowyDlaKlastra(IGrouping<int, Mrowka> klaster)
        {
            var idKlasy = klaster.Key;
            var mrowki = klaster.ToList();
            var centrum = OkreslCentrumKlastra(idKlasy, mrowki);
            var bladDlaKlastra = mrowki.Sum(mrowka => ObliczBladKwadratowyPomiedzy(centrum, mrowka));
            return bladDlaKlastra;
        }

        private static double ObliczLacznyBladKwadratowy(IEnumerable<Mrowka> pogrupowaneMrowki)
        {
            var pogrupowanePogrupowaneMrowki = pogrupowaneMrowki.GroupBy(mrowka => mrowka.Klasa);
            var listaBledow = new List<double>();
            foreach (var grupa in pogrupowanePogrupowaneMrowki)
            {
                var bladDlaGrupy = ObliczBladKwadratowyDlaKlastra(grupa);
                listaBledow.Add(bladDlaGrupy);
            }
            var lacznyBlad = listaBledow.Sum();
            return lacznyBlad;
        }

        private static double ObliczOdlegloscWewnatrzklastrowa(IGrouping<int, Mrowka> klaster)
        {
            var mrowki = klaster.ToList();

            if (mrowki.Count < 2)
            {
                return 0;
            }

            var odleglosc = new MrowkowaOdleglosc(new OdlegloscEuklidesowa());
            var maksOdleglosc = double.MinValue;
            for (int i = 0; i < mrowki.Count; i++)
            {
                var m1 = mrowki[i];
                for (int j = i + 1; j < mrowki.Count; j++)
                {
                    var m2 = mrowki[j];
                    var odlegloscMiedzyMrowkami = odleglosc.OkreslOdleglosc(m1, m2);
                    maksOdleglosc = (odlegloscMiedzyMrowkami > maksOdleglosc) ? odlegloscMiedzyMrowkami : maksOdleglosc;
                }
            }

            return maksOdleglosc;
        }

        private static double ObliczOdlegloscMiedzyklastrowa(IGrouping<int, Mrowka> klaster1, IGrouping<int, Mrowka> klaster2)
        {
            var mrowki1 = klaster1.ToList();
            var mrowki2 = klaster2.ToList();
            var centrum1 = OkreslCentrumKlastra(klaster1.Key, mrowki1);
            var centrum2 = OkreslCentrumKlastra(klaster2.Key, mrowki2);
            var obliczaczOdleglosci = new MrowkowaOdleglosc(new OdlegloscEuklidesowa());
            var odleglosc = obliczaczOdleglosci.OkreslOdleglosc(centrum1, centrum2);
            return odleglosc;
        }

        private static double OkreslNajwiekszaOdlegloscWewnatrzklastrowa(IEnumerable<IGrouping<int, Mrowka>> pogrupowaneMrowki)
        {
            var najwiekszaOdleglosc = pogrupowaneMrowki
                .Select(grupa => ObliczOdlegloscWewnatrzklastrowa(grupa))
                .Max();
            return najwiekszaOdleglosc;
        }

        private static double OkreslNajmniejszaOdlegloscMiedzyklastrowa(IEnumerable<IGrouping<int, Mrowka>> pogrupowaneMrowki)
        {
            var listaGrup = pogrupowaneMrowki.ToList();
            var minOdleglosc = double.MaxValue;

            if (listaGrup.Count < 2)
            {
                return 0;
            }

            for (int i = 0; i < listaGrup.Count; i++)
            {
                var grupa1 = listaGrup[i];
                for (int j = i + 1; j < listaGrup.Count; j++)
                {
                    var grupa2 = listaGrup[j];
                    var odlegloscMiedzyGrupami = ObliczOdlegloscMiedzyklastrowa(grupa1, grupa2);
                    minOdleglosc = odlegloscMiedzyGrupami < minOdleglosc ? odlegloscMiedzyGrupami : minOdleglosc;
                }
            }

            return minOdleglosc;
        }

        private static double ObliczIndeksDunna(IEnumerable<Mrowka> pogrupowaneMrowki)
        {
            var pogrupowanePogrupowaneMrowki = pogrupowaneMrowki.GroupBy(mrowka => mrowka.Klasa);
            var minOdlegloscMiedzy = OkreslNajmniejszaOdlegloscMiedzyklastrowa(pogrupowanePogrupowaneMrowki);
            var maksOdlegloscWewnatrz = OkreslNajwiekszaOdlegloscWewnatrzklastrowa(pogrupowanePogrupowaneMrowki);

            if (maksOdlegloscWewnatrz == 0)
            {
                return 0;
            }

            var indeksDunna = minOdlegloscMiedzy / maksOdlegloscWewnatrz;
            return indeksDunna;
        }
    }
}
