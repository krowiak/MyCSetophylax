using MyCSetophylax.Aktywacja;
using MyCSetophylax.Dane;
using MyCSetophylax.Dopasowanie;
using MyCSetophylax.Klasy;
using MyCSetophylax.KonkretneOdleglosci;
using MyCSetophylax.Przemieszczenie;
using MyCSetophylax.PrzestrzenZyciowa;
using MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie;
using MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie.Obraz;
using MyCSetophylax.SrednieOdleglosci;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            Dictionary<int, int> slownikKlasDocelowych = new Dictionary<int, int>();
            string sciezkaDoDanych =
                //@"PlikiDanych\smiecdane300p.txt";
                //@"PlikiDanych\smiecdane150p.txt";
                //@"PlikiDanych\smiecdane150mikro.txt";
                //@"PlikiDanych\kddcup.data_10_percent.txt";
                //@"PlikiDanych\kdd400.txt";
                //@"PlikiDanych\kdd1000.txt";
                //@"E:\Pobrane\adult.data";
                //@"PlikiDanych\soybean-small.data";
                //@"PlikiDanych\iris.data";
                @"PlikiDanych\wine-etykiety.data";
            using (var strumienDanych = File.OpenRead(sciezkaDoDanych))
            {
                var parser = new ParserDanych() { DaneZawierajaEtykiety = true };
                var niestandaryzowaneDane = parser.ParsujDane(strumienDanych);
                var zScore = new StandaryzatorZScore();
                IEnumerable<double[]> wektoryDanych = zScore.Standaryzuj(niestandaryzowaneDane);
                mrowki = wektoryDanych.Select((wektor, indeks) => new Mrowka(indeks, wektor)).ToList();
                slownikKlasDocelowych = parser.OdczytaneEtykiety;
            }

            var ladnePedzelki = new List<Brush>()
            {
                Brushes.AliceBlue, Brushes.Beige, Brushes.BurlyWood,
                Brushes.Chocolate, Brushes.Coral, Brushes.Crimson,
                Brushes.DarkGoldenrod, Brushes.DarkKhaki, Brushes.DeepPink,
                Brushes.FloralWhite, Brushes.Gainsboro, Brushes.GreenYellow,
                Brushes.Lavender, Brushes.LemonChiffon, Brushes.LightSeaGreen,
                Brushes.MediumSpringGreen, Brushes.Orange, Brushes.PapayaWhip,
                Brushes.PeachPuff, Brushes.Plum, Brushes.Salmon,
                Brushes.Turquoise, Brushes.Wheat, Brushes.Yellow
            };

#region wyswietlanie
            Func<Mrowka, string> okreslaczKlasyDocelowej = KolorujOkreslaczKlasyDocelowej(TworzOkreslaczKlasyDocelowej(slownikKlasDocelowych));

            var reprezentacjaId = new ReprezentacjaIdMrowki();
            var reprezentacjaKlDocelowa = new ReprezentacjaKlasaDocelowa(okreslaczKlasyDocelowej);
            var reprezentacjaKlasa = new ReprezentacjaKlasaMrowki();

            var wylaczoneWyswietlanie = new WylaczoneWyswietlanie();

            long ticks = DateTime.Now.Ticks;
            Directory.CreateDirectory("wyniki");
            var obrazPrzestrzeniIdPrzed = wylaczoneWyswietlanie; //new ObrazPrzestrzeni(ladnePedzelki, reprezentacjaId, $"{ticks}-id-przed.bmp");
            var obrazPrzestrzeniId = new ObrazPrzestrzeni(ladnePedzelki, reprezentacjaId, $"wyniki\\{ticks}-id.bmp");
            var obrazPrzestrzeniKlDocelowa = new ObrazPrzestrzeni(ladnePedzelki, reprezentacjaKlDocelowa, 
                new SlownikowyOkreslaczPedzla(slownikKlasDocelowych, (mrowka) => mrowka.Id, 0), $"wyniki\\{ticks}-klasa-docelowa.bmp");
            var obrazPrzestrzeniKlWynikowa = new ObrazPrzestrzeni(ladnePedzelki, reprezentacjaKlasa, $"wyniki\\{ticks}-klasa-wynikowa.bmp");

            var wyswietlaczIdPrzed = wylaczoneWyswietlanie; //new WyswietlaczPrzestrzeni(reprezentacjaId);
            var wyswietlaczId = new WyswietlaczPrzestrzeni(reprezentacjaId);
            var wyswietlaczKlasa = new WyswietlaczPrzestrzeni(reprezentacjaKlasa);
            var wyswietlaczKlDocelowa = new WyswietlaczPrzestrzeni(reprezentacjaKlDocelowa);
#endregion

            var sX = 2;
            var sY = 2;
            var przestrzen = Przestrzen.StworzPrzestrzenDla(mrowki.Count);
            var sasiedztwo = new Sasiedztwo(przestrzen, sX, sY);
            przestrzen.RozmiescMrowki(mrowki, maszynaLosujaca);
            wyswietlaczIdPrzed.Wyswietl(przestrzen);
            obrazPrzestrzeniIdPrzed.Wyswietl(przestrzen);

            Console.WriteLine();
            Console.WriteLine("Grupowanie...");

            // Przygotowanie całego śmiecia
            var liczbaIteracji = 1000;
            var czas = new Czas(liczbaIteracji);
            var srednieDopasowaniaWCzasie = new SrednieDopasowaniaMrowek();
            var kLambda = 1.0;
            var presja = new StalaPresja(2);//*/new PresjaZaleznaOdCzasu(czas, srednieDopasowaniaWCzasie, kLambda);
            var bazowePrawdopAktywacji = 0.1;
            var aktywator = new AltAktywator(maszynaLosujaca, presja);//*/new Aktywator(maszynaLosujaca, bazowePrawdopAktywacji, presja);
            var deltaT = 50;
            var kAlfa = 0.5;
            var euklides = new MrowkowaOdleglosc(new OdlegloscEuklidesowa());
            var miaraOdleglosci = /*euklides;//*/new SlownikOdleglosci(mrowki, euklides);
            //var srednieOdleglosciOdInnychAgentow = new SrednieOdleglosciOdCzasu(czas, srednieDopasowaniaWCzasie, new KonfiguracjaSredniejOdlOdCzasu()
            //{
            //    IleJednostekCzasuSpogladacWstecz = deltaT,
            //    SposobOkreslaniaWartosciPrzedUaktywnieniem = new NajsredniejszaOdleglosc(mrowki, miaraOdleglosci), // str.11, 12 - stałe 0.5coś, 0.4coś? Ale zupełnie nie działają u mnie, średnia odl. dla t=50 ~2.coś, więc wyniki są zawsze ujemne -> wszystko się zawsze rusza?
            //    StopienWplywuRoznicySrednichNaWynikWDanejJednostceCzasu = kAlfa
            //});
            var srednieOdleglosciOdInnychAgentow = new SrednieOdleglosciDlaAgentow(mrowki, miaraOdleglosci);
            //var srednieOdleglosciOdInnychAgentow = new NajsredniejszaOdleglosc(mrowki, miaraOdleglosci);
            //var srednieOdleglosciOdInnychAgentow = new SredniaPodzbioru(mrowki, miaraOdleglosci, 0.05, maszynaLosujaca);
            //var srednieOdleglosciOdInnychAgentow = new StalaUdajacaSrednia(5.7070232700742309);
            var oceniacz = /*new AltOceniacz(miaraOdleglosci, srednieOdleglosciOdInnychAgentow, sasiedztwo);//*/new Oceniacz(miaraOdleglosci, srednieOdleglosciOdInnychAgentow, sasiedztwo);
            var stopienZachlannosci = 0.9;
            IPrzemieszczacz przemieszczacz;
            {
                var losowyPrzemieszczacz = new LosowyPrzemieszczacz(przestrzen, sasiedztwo, maszynaLosujaca);
                var zachlannyPrzemieszczacz = new ZachlannyPrzemieszczacz(stopienZachlannosci, losowyPrzemieszczacz,
                    oceniacz, przestrzen, sasiedztwo, maszynaLosujaca);
                przemieszczacz = zachlannyPrzemieszczacz;
            }
            var okreslaczKlas = 
                new OkreslaczKlas(sasiedztwo);
                //new OpoznionyOkreslaczKlas(czas, 4900, new GlobalnyPodobienstwowyOkreslaczKlas(odleglosci, sasiedztwo, czas, true) { MinProgLicznosci = 20 });
                //new OpoznionyOkreslaczKlas(czas, 4000, new OkreslaczKlas(sasiedztwo));
            AktywatorUwzglPodobienstwo sprawdzaczNiepodobienstwa = null;//*/ new AktywatorUwzglPodobienstwo(sasiedztwo, odleglosci, 15, 0.5);

            // Grupowanie właściwe
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
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
                    var (x, y) = pozycja;
                    var mrowka = przestrzen[y][x];
                    var czyNiepodobneIstnieja = sprawdzaczNiepodobienstwa?.CzyAktywowac(mrowka, pozycja) ?? false;
                    if (czyNiepodobneIstnieja || aktywator.CzyAktywowac(ocenyPozycji[pozycja]))
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
                
                if (czas.Aktualny % 500 == 0)
                {
                    Console.WriteLine($"Koniec iteracji {czas.Aktualny}.");
                }
                czas.Uplywaj();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);
            }
            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine($"Pogrupowano! {stopwatch.ElapsedMilliseconds}ms");

            obrazPrzestrzeniId.Wyswietl(przestrzen);
            wyswietlaczId.Wyswietl(przestrzen);
            Console.WriteLine();

            obrazPrzestrzeniKlWynikowa.Wyswietl(przestrzen);
            wyswietlaczKlasa.Wyswietl(przestrzen);
            Console.WriteLine();

            obrazPrzestrzeniKlDocelowa.Wyswietl(przestrzen);
            wyswietlaczKlDocelowa.Wyswietl(przestrzen);

            Console.Beep();
            Console.ReadKey();

            var liczbaMrowekWKazdejKlasieDocelowej = slownikKlasDocelowych.Values
                .GroupBy(klasa => klasa)
                .Select(grupa => new { Klucz = grupa.Key, Liczba = grupa.Count() })
                .ToDictionary(liczbaKlasy => liczbaKlasy.Klucz, liczbaKlasy => liczbaKlasy.Liczba);
            var grupyMrowek = mrowki.GroupBy(mrowka => mrowka.Klasa).ToList();
            foreach (var grupa in grupyMrowek)
            {
                var liczbaMrowekWGrupie = grupa.Count();
                var faktyczneKlasy = grupa.GroupBy(mrowka => slownikKlasDocelowych[mrowka.Id]).ToList();
                Console.WriteLine($"Klasa wynikowa {grupa.Key}: {liczbaMrowekWGrupie} mrówek z {faktyczneKlasy.Count} klas docelowych:");

                foreach (var fakKlasa in faktyczneKlasy)
                {
                    var liczbaMrowekFakKlasy = fakKlasa.Count();
                    Console.WriteLine($"\tKlasa {fakKlasa.Key}: {fakKlasa.Count()} mrówek: ");
                    Console.WriteLine($"\t\t({(liczbaMrowekFakKlasy / (double)liczbaMrowekWKazdejKlasieDocelowej[fakKlasa.Key]) * 100}% mrówek klasy docelowej,");
                    Console.WriteLine($"\t\t{(liczbaMrowekFakKlasy / (double)liczbaMrowekWGrupie) * 100}% mrówek klasy wynikowej,");
                    Console.WriteLine($"\t\t{(liczbaMrowekFakKlasy / (double)mrowki.Count) * 100}% wszystkich mrówek.");
                }

                Console.WriteLine();
            }
            Console.WriteLine($"Czas: {stopwatch.ElapsedMilliseconds}ms");
            Console.ReadKey();
        }

        private static Func<Mrowka, string> TworzOkreslaczKlasyDocelowej(Dictionary<int, int> slownikKlas)
        {
            return (Mrowka mrowka) =>
            {
                if (mrowka == null)
                {
                    return String.Empty;
                }
                else
                {
                    return slownikKlas.TryGetValue(mrowka.Id, out int klasa) ? klasa.ToString() : "?";
                }
            };
        }

        private static Func<Mrowka, string> KolorujOkreslaczKlasyDocelowej(Func<Mrowka, string> okreslacz)
        {
            return (Mrowka mrowka) =>
            {
                var wartosc = okreslacz(mrowka);

                if (int.TryParse(wartosc, out int wartoscNum))
                {
                    switch (wartoscNum % 4)
                    {
                        case 0:
                            Console.BackgroundColor = ConsoleColor.Green;
                            break;
                        case 1:
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            break;
                        case 2:
                            Console.BackgroundColor = ConsoleColor.Red;
                            break;
                        case 3:
                            Console.BackgroundColor = ConsoleColor.Blue;
                            break;
                    }
                }
                else
                {
                    if (wartosc == String.Empty)
                    {
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                }

                return wartosc;
            };
         }

        private static string OkreslaczKlasyDocelowejIrysow(Mrowka mrowka)
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
                    Console.BackgroundColor = ConsoleColor.White;
                    return "?";
            }
        }

        private static string OkreslaczKlasyDocelowejSmiecdanych400(Mrowka mrowka)
        {
            switch (mrowka?.Id)
            {
                case null:
                    Console.ResetColor();
                    return String.Empty;
                case int id when id < 100:
                    Console.BackgroundColor = ConsoleColor.Green;
                    return "1";
                case int id when id < 200:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    return "2";
                case int id when id < 300:
                    Console.BackgroundColor = ConsoleColor.Red;
                    return "3";
                case int id when id < 400:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    return "4";
                default:
                    Console.BackgroundColor = ConsoleColor.White;
                    return "?";
            }
        }

        private static string OkreslaczKlasyDocelowejWin(Mrowka mrowka)
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