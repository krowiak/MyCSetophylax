using MyCSetophylax.Aktywacja;
using MyCSetophylax.Dane;
using MyCSetophylax.Dopasowanie;
using MyCSetophylax.Klasy;
using MyCSetophylax.KonkretneOdleglosci;
using MyCSetophylax.Przemieszczenie;
using MyCSetophylax.PrzestrzenZyciowa;
using MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie;
using MyCSetophylax.SrednieOdleglosci;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax
{
    class Program
    {
        static void Main(string[] args)
        {
            var maszynaLosujaca = new Random();
            List<Mrowka> mrowki;
            using (var strumienDanych = File.OpenRead(@"PlikiDanych\irisBezSmieci.data"))
            {
                var parser = new ParserDanych();
                var niestandaryzowaneDane = parser.ParsujDane(strumienDanych);
                var zScore = new StandaryzatorZScore();
                IEnumerable<double[]> wektoryDanych = zScore.Standaryzuj(niestandaryzowaneDane);
                mrowki = wektoryDanych.Select((wektor, indeks) => new Mrowka(indeks, wektor)).ToList();
            }

            var sX = 2;
            var sY = 2;
            var przestrzen = Przestrzen.StworzPrzestrzenDla(mrowki.Count);
            var sasiedztwo = new Sasiedztwo(przestrzen, sX, sY);
            var reprezentacjaId = new ReprezentacjaIdMrowki();
            var wyswietlaczId = new WyswietlaczPrzestrzeni(reprezentacjaId);
            przestrzen.RozmiescMrowki(mrowki, maszynaLosujaca);
            wyswietlaczId.Wyswietl(przestrzen);

            Console.WriteLine();
            Console.WriteLine("Grupowanie...");

            // Przygotowanie całego śmiecia
            var liczbaIteracji = 5000;
            var czas = new Czas(liczbaIteracji);
            var srednieDopasowaniaWCzasie = new SrednieDopasowaniaMrowek();
            var kLambda = 1.0;
            var presja = new PresjaZaleznaOdCzasu(czas, srednieDopasowaniaWCzasie, kLambda);
            var bazowePrawdopAktywacji = 0.1;
            var aktywator = new Aktywator(czas, maszynaLosujaca, bazowePrawdopAktywacji, presja);
            var euklides = new OdlegloscEuklidesowa();
            var odleglosci = new OdleglosciPomiedzyMrowkami(mrowki, euklides);
            var deltaT = 50;
            var kAlfa = 0.5;
            //var srednieOdleglosciOdInnychAgentow = new SrednieOdleglosciOdCzasu(czas, srednieDopasowaniaWCzasie, new KonfiguracjaSredniejOdlOdCzasu()
            //{
            //    IleJednostekCzasuSpogladacWstecz = deltaT,
            //    SposobOkreslaniaWartosciPrzedUaktywnieniem = new NajsredniejszaOdleglosc(mrowki, odleglosci), // str.11, 12 - stałe 0.5coś, 0.4coś? Ale zupełnie nie działają u mnie, średnia odl. dla t=50 ~2.coś, więc wyniki są zawsze ujemne -> wszystko się zawsze rusza?
            //    StopienWplywuRoznicySrednichNaWynikWDanejJednostceCzasu = kAlfa
            //});
            var srednieOdleglosciOdInnychAgentow = new SrednieOdleglosciDlaAgentow(mrowki, odleglosci);
            var slownikOdleglosci = new SlownikOdleglosci(mrowki, odleglosci);
            var oceniacz = new Oceniacz(slownikOdleglosci, srednieOdleglosciOdInnychAgentow, sasiedztwo);
            var stopienZachlannosci = 0.9;
            IPrzemieszczacz przemieszczacz;
            {
                var losowyPrzemieszczacz = new LosowyPrzemieszczacz(przestrzen, sasiedztwo, maszynaLosujaca);
                var zachlannyPrzemieszczacz = new ZachlannyPrzemieszczacz(stopienZachlannosci, losowyPrzemieszczacz,
                    oceniacz, przestrzen, sasiedztwo, maszynaLosujaca);
                przemieszczacz = zachlannyPrzemieszczacz;
            }
            var okreslaczKlas = new OpoznionyOkreslaczKlas(czas, 4995, new OkreslaczKlas(sasiedztwo));

            // Grupowanie właściwe
            while (!czas.CzyUplynal)
            {
                var ocenyPozycji = new Dictionary<(int, int), double>();
                for (int y = 0; y < przestrzen.DlugoscBoku; y++)
                {
                    for (int x = 0; x < przestrzen.DlugoscBoku; x++)
                    {
                        Mrowka aktMrowka = przestrzen[y][x];
                        if (aktMrowka != null)
                        {
                            var pozycja = (x, y);
                            var ocena = oceniacz.Ocen(aktMrowka, pozycja);
                            ocenyPozycji[pozycja] = ocena;
                        }
                    }
                }
                
                var sumaOcenNaTenCzas = ocenyPozycji.Sum(kvp => kvp.Value);
                var sredniaOcenNaTenCzas = sumaOcenNaTenCzas / mrowki.Count;
                srednieDopasowaniaWCzasie[czas.Aktualny] = sredniaOcenNaTenCzas;

                var pozycjeDoZmianyKlasy = new List<(int, int)>();
                var pozycjeDoZmianyPozycji = new List<(int, int)>();
                foreach (var pozycja in ocenyPozycji.Keys)
                {
                    if (aktywator.CzyAktywowac(ocenyPozycji[pozycja]))
                    {
                        pozycjeDoZmianyPozycji.Add(pozycja);
                    }
                    else
                    {
                        pozycjeDoZmianyKlasy.Add(pozycja);
                    }
                }

                foreach (var pozMrowki in pozycjeDoZmianyPozycji)
                {
                    (int x, int y) = pozMrowki;
                    var mrowka = przestrzen[y][x];
                    przemieszczacz.Przemiesc(mrowka, pozMrowki);
                }

                Dictionary<Mrowka, int> noweKlasyMrowek = new Dictionary<Mrowka, int>();
                foreach (var pozMrowki in pozycjeDoZmianyKlasy)
                {
                    (int x, int y) = pozMrowki;
                    var mrowka = przestrzen[y][x];
                    var nowaKlasa = okreslaczKlas.OkreslKlase(mrowka, pozMrowki);
                    noweKlasyMrowek[mrowka] = nowaKlasa;
                }
                foreach (var mrowkaIKlasa in noweKlasyMrowek)
                {
                    var mrowka = mrowkaIKlasa.Key;
                    var klasa = mrowkaIKlasa.Value;
                    mrowka.Klasa = klasa;
                }

                //if (czas.Aktualny > 1900)
                //{
                //    Console.WriteLine();
                //    var reprezentacjaKlasaa = new ReprezentacjaKlasaMrowki();
                //    var wyswietlaczKlasaa = new WyswietlaczPrzestrzeni(reprezentacjaKlasaa);
                //    wyswietlaczKlasaa.Wyswietl(przestrzen);
                //    //wyswietlaczId.Wyswietl(przestrzen);
                //}
                czas.Uplywaj();
            }

            Console.WriteLine();
            Console.WriteLine("Pogrupowano!");

            wyswietlaczId.Wyswietl(przestrzen);
            Console.WriteLine();

            var reprezentacjaKlasa = new ReprezentacjaKlasaMrowki();
            var wyswietlaczKlasa = new WyswietlaczPrzestrzeni(reprezentacjaKlasa);
            wyswietlaczKlasa.Wyswietl(przestrzen);
            Console.WriteLine();

            Func<Mrowka, string> okreslaczKlasyDocelowej = okreslaczKlasyDocelowejIrysow;
            var reprezentacjaKlDocelowa = new ReprezentacjaKlasaDocelowa(okreslaczKlasyDocelowej);
            var wyswietlaczKlDocelowa = new WyswietlaczPrzestrzeni(reprezentacjaKlDocelowa);
            wyswietlaczKlDocelowa.Wyswietl(przestrzen);
            
            Console.Beep();
            Console.ReadKey();
        }

        private static string okreslaczKlasyDocelowejIrysow(Mrowka mrowka)
        {
            switch (mrowka?.Id)
            {
                case null:
                    Console.ResetColor();
                    return String.Empty;
                case int id when id < 50:
                    Console.BackgroundColor = ConsoleColor.Green;
                    return "1";
                case int id when id < 100:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    return "2";
                case int id when id < 150:
                    Console.BackgroundColor = ConsoleColor.Red;
                    return "3";
                default:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    return "?";
            }
        }

        private static string okreslaczKlasyDocelowejWin(Mrowka mrowka)
        {
            switch (mrowka?.Id)
            {
                case null:
                    Console.ResetColor();
                    return String.Empty;
                case int id when id < 59:
                    Console.BackgroundColor = ConsoleColor.Green;
                    return "1";
                case int id when id < 130:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    return "2";
                case int id when id < 178:
                    Console.BackgroundColor = ConsoleColor.Red;
                    return "3";
                default:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    return "?";
            }
        }
    }
}