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
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax
{
    class Program
    {
        #region konfiguracje
        private static KonfiguracjaGrupowania Konfiguracja_A4C_2004(Random maszynaLosujaca, int liczIteracji, int zasiegWzroku,
            IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> miaraOdleglosci)
        {
            var deltaT = 50;
            var kAlfa = 0.5;
            var kLambda = 1.0;

            var przestrzen = Przestrzen.StworzPrzestrzenDla(mrowki.Count());
            var sasiedztwo = new Sasiedztwo(przestrzen, zasiegWzroku, zasiegWzroku);
            var czas = new Czas(liczIteracji);
            var srednieDopasowania = new SrednieDopasowaniaMrowek();
            var presja = new PresjaZaleznaOdCzasu(czas, srednieDopasowania, kLambda);
            var srednieOdleglosci = new SrednieOdleglosciOdCzasu(czas, srednieDopasowania, new KonfiguracjaSredniejOdlOdCzasu()
            {
                IleJednostekCzasuSpogladacWstecz = deltaT,
                SposobOkreslaniaWartosciPrzedUaktywnieniem = new StalaUdajacaSrednia(0.4483),
                StopienWplywuRoznicySrednichNaWynikWDanejJednostceCzasu = kAlfa
            });

            return new KonfiguracjaGrupowania()
            {
                MaszynaLosujaca = maszynaLosujaca,
                SrednieDopasowania = srednieDopasowania,
                Aktywator = new AltAktywator(maszynaLosujaca, presja),
                LiczbaIteracji = liczIteracji,
                Przestrzen = przestrzen,
                Sasiedztwo = sasiedztwo,
                SrednieOdleglosci = srednieOdleglosci,
                Oceniacz = new AltOceniacz(miaraOdleglosci, srednieOdleglosci, sasiedztwo),
                Czas = czas
            };
        }

        private static KonfiguracjaGrupowania Konfiguracja_A4C_2007(Random maszynaLosujaca, int liczIteracji, int zasiegWzroku,
            IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> miaraOdleglosci)
        {
            var deltaT = 50;
            var kAlfa = 0.5;
            var kLambda = 1.0;
            var bazowePrawdopAktywacji = 0.1;

            var przestrzen = Przestrzen.StworzPrzestrzenDla(mrowki.Count());
            var sasiedztwo = new Sasiedztwo(przestrzen, zasiegWzroku, zasiegWzroku);
            var czas = new Czas(liczIteracji);
            var srednieDopasowania = new SrednieDopasowaniaMrowek();
            var presja = new PresjaZaleznaOdCzasu(czas, srednieDopasowania, kLambda);
            var srednieOdleglosci = new SrednieOdleglosciOdCzasu(czas, srednieDopasowania, new KonfiguracjaSredniejOdlOdCzasu()
            {
                IleJednostekCzasuSpogladacWstecz = deltaT,
                SposobOkreslaniaWartosciPrzedUaktywnieniem = new NajsredniejszaOdleglosc(mrowki, miaraOdleglosci),
                StopienWplywuRoznicySrednichNaWynikWDanejJednostceCzasu = kAlfa
            });

            return new KonfiguracjaGrupowania()
            {
                MaszynaLosujaca = maszynaLosujaca,
                SrednieDopasowania = srednieDopasowania,
                Aktywator = new Aktywator(maszynaLosujaca, bazowePrawdopAktywacji, presja),
                LiczbaIteracji = liczIteracji,
                Przestrzen = przestrzen,
                Sasiedztwo = sasiedztwo,
                SrednieOdleglosci = srednieOdleglosci,
                Oceniacz = new Oceniacz(miaraOdleglosci, srednieOdleglosci, sasiedztwo),
                Czas = czas
            };
        }

        private static KonfiguracjaGrupowania Konfiguracja_SA4C_2004(Random maszynaLosujaca, int liczIteracji, int zasiegWzroku,
            IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> miaraOdleglosci)
        {
            var przestrzen = Przestrzen.StworzPrzestrzenDla(mrowki.Count());
            var sasiedztwo = new Sasiedztwo(przestrzen, zasiegWzroku, zasiegWzroku);
            var czas = new Czas(liczIteracji);
            var srednieDopasowania = new SrednieDopasowaniaMrowek();
            var presja = new StalaPresja(2);
            var srednieOdleglosci = new StalaUdajacaSrednia(0.3);

            return new KonfiguracjaGrupowania()
            {
                MaszynaLosujaca = maszynaLosujaca,
                SrednieDopasowania = srednieDopasowania,
                Aktywator = new AltAktywator(maszynaLosujaca, presja),
                LiczbaIteracji = liczIteracji,
                Przestrzen = przestrzen,
                Sasiedztwo = sasiedztwo,
                SrednieOdleglosci = srednieOdleglosci,
                Oceniacz = new AltOceniacz(miaraOdleglosci, srednieOdleglosci, sasiedztwo),
                Czas = czas
            };
        }

        private static KonfiguracjaGrupowania Konfiguracja_SA4C_2007(Random maszynaLosujaca, int liczIteracji, int zasiegWzroku,
            IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> miaraOdleglosci)
        {
            var bazowePrawdopAktywacji = 0.1;

            var przestrzen = Przestrzen.StworzPrzestrzenDla(mrowki.Count());
            var sasiedztwo = new Sasiedztwo(przestrzen, zasiegWzroku, zasiegWzroku);
            var czas = new Czas(liczIteracji);
            var srednieDopasowania = new SrednieDopasowaniaMrowek();
            var presja = new StalaPresja(2);
            var srednieOdleglosci = new NajsredniejszaOdleglosc(mrowki, miaraOdleglosci);

            return new KonfiguracjaGrupowania()
            {
                MaszynaLosujaca = maszynaLosujaca,
                SrednieDopasowania = srednieDopasowania,
                Aktywator = new Aktywator(maszynaLosujaca, bazowePrawdopAktywacji, presja),
                LiczbaIteracji = liczIteracji,
                Przestrzen = przestrzen,
                Sasiedztwo = sasiedztwo,
                SrednieOdleglosci = srednieOdleglosci,
                Oceniacz = new Oceniacz(miaraOdleglosci, srednieOdleglosci, sasiedztwo),
                Czas = czas
            };
        }

        private static KonfiguracjaGrupowania Konfiguracja_SA4CPrim_2004(Random maszynaLosujaca, int liczIteracji, int zasiegWzroku,
            IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> miaraOdleglosci)
        {
            var przestrzen = Przestrzen.StworzPrzestrzenDla(mrowki.Count());
            var sasiedztwo = new Sasiedztwo(przestrzen, zasiegWzroku, zasiegWzroku);
            var czas = new Czas(liczIteracji);
            var srednieDopasowania = new SrednieDopasowaniaMrowek();
            var presja = new StalaPresja(2);
            var srednieOdleglosci = new SrednieOdleglosciDlaAgentow(mrowki, miaraOdleglosci);

            return new KonfiguracjaGrupowania()
            {
                MaszynaLosujaca = maszynaLosujaca,
                SrednieDopasowania = srednieDopasowania,
                Aktywator = new AltAktywator(maszynaLosujaca, presja),
                LiczbaIteracji = liczIteracji,
                Przestrzen = przestrzen,
                Sasiedztwo = sasiedztwo,
                SrednieOdleglosci = srednieOdleglosci,
                Oceniacz = new AltOceniacz(miaraOdleglosci, srednieOdleglosci, sasiedztwo),
                Czas = czas
            };
        }

        private static KonfiguracjaGrupowania Konfiguracja_SA4CPrim_2007(Random maszynaLosujaca, int liczIteracji, int zasiegWzroku,
            IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> miaraOdleglosci)
        {
            var bazowePrawdopAktywacji = 0.1;

            var przestrzen = Przestrzen.StworzPrzestrzenDla(mrowki.Count());
            var sasiedztwo = new Sasiedztwo(przestrzen, zasiegWzroku, zasiegWzroku);
            var czas = new Czas(liczIteracji);
            var srednieDopasowania = new SrednieDopasowaniaMrowek();
            var presja = new StalaPresja(2);
            var srednieOdleglosci = new SrednieOdleglosciDlaAgentow(mrowki, miaraOdleglosci);

            return new KonfiguracjaGrupowania()
            {
                MaszynaLosujaca = maszynaLosujaca,
                SrednieDopasowania = srednieDopasowania,
                Aktywator = new Aktywator(maszynaLosujaca, bazowePrawdopAktywacji, presja),
                LiczbaIteracji = liczIteracji,
                Przestrzen = przestrzen,
                Sasiedztwo = sasiedztwo,
                SrednieOdleglosci = srednieOdleglosci,
                Oceniacz = new Oceniacz(miaraOdleglosci, srednieOdleglosci, sasiedztwo),
                Czas = czas
            };
        }
        #endregion

        private static void Powtarzaj(Func<KonfiguracjaGrupowania> konfiguruj, long msNaStworzenieMiaryOdleglosci,
            List<Mrowka> mrowki, Dictionary<int, int> slownikKlasDocelowych,
            string sciezkaWynikow, int ileRazy)
        {
            for (int i = 0; i < ileRazy; i++)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var konfiguracja = konfiguruj();
                stopwatch.Stop();
                var msNaStworzenieKonfiguracji = stopwatch.ElapsedMilliseconds;

                var msNaPrzygotowania = msNaStworzenieKonfiguracji + msNaStworzenieMiaryOdleglosci;
                Wykonuj(sciezkaWynikow, i, true, false, konfiguracja, mrowki, slownikKlasDocelowych, msNaPrzygotowania);

                foreach (var mrowka in mrowki)
                {
                    mrowka.Klasa = mrowka.Id;
                }
            }
        }

        static void Main(string[] args)
        {
            string katalogDocelowy = DateTime.Now.ToString("yyy-MM-dd--hh-mm-ss");
            string sciezkaWynikow = $"wyniki\\{katalogDocelowy}\\";
            int liczbaIteracji = 5000;
            int liczbaPowtorzen = 1;
            int zasiegWzroku = 1;

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
                @"PlikiDanych\iris.data";
                //@"PlikiDanych\wine-etykiety.data";
            using (var strumienDanych = File.OpenRead(sciezkaDoDanych))
            {
                var parser = new ParserDanych() { DaneZawierajaEtykiety = true };
                var niestandaryzowaneDane = parser.ParsujDane(strumienDanych);
                var zScore = new StandaryzatorZScore();
                IEnumerable<double[]> wektoryDanych = zScore.Standaryzuj(niestandaryzowaneDane);
                mrowki = wektoryDanych.Select((wektor, indeks) => new Mrowka(indeks, wektor)).ToList();
                slownikKlasDocelowych = parser.OdczytaneEtykiety;
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var euklides = new MrowkowaOdleglosc(new OdlegloscEuklidesowa());
            var miaraOdleglosci = /*euklides;//*/new SlownikOdleglosci(mrowki, euklides);
            stopwatch.Stop();
            var msNaStworzenieOdleglosci = stopwatch.ElapsedMilliseconds;

            Func<KonfiguracjaGrupowania> konfig_asm2004_a4c = () => Konfiguracja_A4C_2004(maszynaLosujaca, liczbaIteracji, zasiegWzroku, mrowki, miaraOdleglosci);
            Func<KonfiguracjaGrupowania> konfig_asm2007_a4c = () => Konfiguracja_A4C_2007(maszynaLosujaca, liczbaIteracji, zasiegWzroku, mrowki, miaraOdleglosci);
            Func<KonfiguracjaGrupowania> konfig_asm2004_sa4c = () => Konfiguracja_SA4C_2004(maszynaLosujaca, liczbaIteracji, zasiegWzroku, mrowki, miaraOdleglosci);
            Func<KonfiguracjaGrupowania> konfig_asm2007_sa4c = () => Konfiguracja_SA4C_2007(maszynaLosujaca, liczbaIteracji, zasiegWzroku, mrowki, miaraOdleglosci);
            Func<KonfiguracjaGrupowania> konfig_asm2004_sa4cprim = () => Konfiguracja_SA4CPrim_2004(maszynaLosujaca, liczbaIteracji, zasiegWzroku, mrowki, miaraOdleglosci);
            Func<KonfiguracjaGrupowania> konfig_asm2007_sa4cprim = () => Konfiguracja_SA4CPrim_2007(maszynaLosujaca, liczbaIteracji, zasiegWzroku, mrowki, miaraOdleglosci);

            Powtarzaj(konfig_asm2004_a4c, msNaStworzenieOdleglosci, mrowki, slownikKlasDocelowych, sciezkaWynikow + "ASM2004_A4C", liczbaPowtorzen);
            Powtarzaj(konfig_asm2007_a4c, msNaStworzenieOdleglosci, mrowki, slownikKlasDocelowych, sciezkaWynikow + "ASM2007_A4C", liczbaPowtorzen);
            Powtarzaj(konfig_asm2004_sa4c, msNaStworzenieOdleglosci, mrowki, slownikKlasDocelowych, sciezkaWynikow + "ASM2004_SA4C", liczbaPowtorzen);
            Powtarzaj(konfig_asm2007_sa4c, msNaStworzenieOdleglosci, mrowki, slownikKlasDocelowych, sciezkaWynikow + "ASM2007_SA4C", liczbaPowtorzen);
            Powtarzaj(konfig_asm2004_sa4cprim, msNaStworzenieOdleglosci, mrowki, slownikKlasDocelowych, sciezkaWynikow + "ASM2004_SA4CPrim", liczbaPowtorzen);
            Powtarzaj(konfig_asm2007_sa4cprim, msNaStworzenieOdleglosci, mrowki, slownikKlasDocelowych, sciezkaWynikow + "ASM2007_SA4CPrim", liczbaPowtorzen);


            //    string katalogDocelowy = DateTime.Now.ToString("yyy-MM-dd--hh-mm-ss") + "ASM2007_A4C_zadlugo";
            //    string sciezkaWynikow = $"wyniki\\{katalogDocelowy}";
            //    int liczbaPowtorzen = 100;
            //    bool trybCichyWSensieStuprocentowoDoslownym = true;
            //    bool niePowaznieNawetBezBeepaNaSamiutenkiKoniec = false;
            //    bool czekajNaReadLine = false;
            //    int zasiegWzroku = 1;

            //    for (int iPowtorzenia = 0; iPowtorzenia < liczbaPowtorzen; iPowtorzenia++)
            //    {
            //        var maszynaLosujaca = new Random();
            //        List<Mrowka> mrowki;
            //        Dictionary<int, int> slownikKlasDocelowych = new Dictionary<int, int>();
            //        string sciezkaDoDanych =
            //            //@"PlikiDanych\smiecdane300p.txt";
            //            //@"PlikiDanych\smiecdane150p.txt";
            //            //@"PlikiDanych\smiecdane150mikro.txt";
            //            //@"PlikiDanych\kddcup.data_10_percent.txt";
            //            //@"PlikiDanych\kdd400.txt";
            //            //@"PlikiDanych\kdd1000.txt";
            //            //@"E:\Pobrane\adult.data";
            //            //@"PlikiDanych\soybean-small.data";
            //            @"PlikiDanych\iris.data";
            //            //@"PlikiDanych\wine-etykiety.data";
            //        using (var strumienDanych = File.OpenRead (sciezkaDoDanych))
            //        {
            //            var parser = new ParserDanych() { DaneZawierajaEtykiety = true };
            //            var niestandaryzowaneDane = parser.ParsujDane(strumienDanych);
            //            var zScore = new StandaryzatorZScore();
            //            IEnumerable<double[]> wektoryDanych = zScore.Standaryzuj(niestandaryzowaneDane);
            //            mrowki = wektoryDanych.Select((wektor, indeks) => new Mrowka(indeks, wektor)).ToList();
            //            slownikKlasDocelowych = parser.OdczytaneEtykiety;
            //        }

            //        var ladnePedzelki = new List<Brush>()
            //        {
            //            Brushes.AliceBlue, Brushes.Beige, Brushes.BurlyWood,
            //            Brushes.Chocolate, Brushes.Coral, Brushes.Crimson,
            //            Brushes.DarkGoldenrod, Brushes.DarkKhaki, Brushes.DeepPink,
            //            Brushes.FloralWhite, Brushes.Gainsboro, Brushes.GreenYellow,
            //            Brushes.Lavender, Brushes.LemonChiffon, Brushes.LightSeaGreen,
            //            Brushes.MediumSpringGreen, Brushes.Orange, Brushes.PapayaWhip,
            //            Brushes.PeachPuff, Brushes.Plum, Brushes.Salmon,
            //            Brushes.Turquoise, Brushes.Wheat, Brushes.Yellow
            //        };

            //        #region wyswietlanie
            //        Func<Mrowka, string> okreslaczKlasyDocelowej = KolorujOkreslaczKlasyDocelowej(TworzOkreslaczKlasyDocelowej(slownikKlasDocelowych));

            //        var reprezentacjaId = new ReprezentacjaIdMrowki();
            //        var reprezentacjaKlDocelowa = new ReprezentacjaKlasaDocelowa(okreslaczKlasyDocelowej);
            //        var reprezentacjaKlasa = new ReprezentacjaKlasaMrowki();

            //        var wylaczoneWyswietlanie = new WylaczoneWyswietlanie();

            //        Directory.CreateDirectory(sciezkaWynikow);
            //        var obrazPrzestrzeniIdPrzed = wylaczoneWyswietlanie; //new ObrazPrzestrzeni(ladnePedzelki, reprezentacjaId, $"{ticks}-id-przed.bmp");
            //        var obrazPrzestrzeniId = new ObrazPrzestrzeni(ladnePedzelki, reprezentacjaId, $"{sciezkaWynikow}\\{iPowtorzenia}-id.bmp");
            //        var obrazPrzestrzeniKlDocelowa = new ObrazPrzestrzeni(ladnePedzelki, reprezentacjaKlDocelowa,
            //            new SlownikowyOkreslaczPedzla(slownikKlasDocelowych, (mrowka) => mrowka.Id, 0), $"{sciezkaWynikow}\\{iPowtorzenia}-klasa-docelowa.bmp");
            //        var obrazPrzestrzeniKlWynikowa = new ObrazPrzestrzeni(ladnePedzelki, reprezentacjaKlasa, $"{sciezkaWynikow}\\{iPowtorzenia}-klasa-wynikowa.bmp");

            //        var wyswietlaczIdPrzed = wylaczoneWyswietlanie; //new WyswietlaczPrzestrzeni(reprezentacjaId);
            //        var wyswietlaczId = new WyswietlaczPrzestrzeni(reprezentacjaId);
            //        var wyswietlaczKlasa = new WyswietlaczPrzestrzeni(reprezentacjaKlasa);
            //        var wyswietlaczKlDocelowa = new WyswietlaczPrzestrzeni(reprezentacjaKlDocelowa);
            //        #endregion

            //        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            //        var sX = zasiegWzroku;
            //        var sY = zasiegWzroku;
            //        var przestrzen = Przestrzen.StworzPrzestrzenDla(mrowki.Count);
            //        var sasiedztwo = new Sasiedztwo(przestrzen, sX, sY);
            //        przestrzen.RozmiescMrowki(mrowki, maszynaLosujaca);

            //        stopwatch.Stop();
            //        wyswietlaczIdPrzed.Wyswietl(przestrzen);
            //        obrazPrzestrzeniIdPrzed.Wyswietl(przestrzen);

            //        Console.WriteLine();
            //        Console.WriteLine($"Grupowanie - {iPowtorzenia + 1}...");
            //        stopwatch.Start();

            //        // Przygotowanie całego śmiecia
            //        var liczbaIteracji = 5000;
            //        var czas = new Czas(liczbaIteracji);
            //        var srednieDopasowaniaWCzasie = new SrednieDopasowaniaMrowek();
            //        var kLambda = 1.0;
            //        var presja = /*new StalaPresja(2);//*/new PresjaZaleznaOdCzasu(czas, srednieDopasowaniaWCzasie, kLambda);
            //        var bazowePrawdopAktywacji = 0.1;
            //        var aktywator = /*new AltAktywator(maszynaLosujaca, presja);//*/new Aktywator(maszynaLosujaca, bazowePrawdopAktywacji, presja);
            //        var deltaT = 50;
            //        var kAlfa = 0.5;
            //        var euklides = new MrowkowaOdleglosc(new OdlegloscEuklidesowa());
            //        var miaraOdleglosci = /*euklides;//*/new SlownikOdleglosci(mrowki, euklides);
            //        var srednieOdleglosciOdInnychAgentow = new SrednieOdleglosciOdCzasu(czas, srednieDopasowaniaWCzasie, new KonfiguracjaSredniejOdlOdCzasu()
            //        {
            //            IleJednostekCzasuSpogladacWstecz = deltaT,
            //            SposobOkreslaniaWartosciPrzedUaktywnieniem = /*new StalaUdajacaSrednia(0.4483),//*/new NajsredniejszaOdleglosc(mrowki, miaraOdleglosci), // str.11, 12 - stałe 0.5coś, 0.4coś? Ale zupełnie nie działają u mnie, średnia odl. dla t=50 ~2.coś, więc wyniki są zawsze ujemne -> wszystko się zawsze rusza?
            //            StopienWplywuRoznicySrednichNaWynikWDanejJednostceCzasu = kAlfa
            //        });
            //        //var srednieOdleglosciOdInnychAgentow = new SrednieOdleglosciDlaAgentow(mrowki, miaraOdleglosci);
            //        //var srednieOdleglosciOdInnychAgentow = new NajsredniejszaOdleglosc(mrowki, miaraOdleglosci);
            //        //var srednieOdleglosciOdInnychAgentow = new SredniaPodzbioru(mrowki, miaraOdleglosci, 0.05, maszynaLosujaca);
            //        //var srednieOdleglosciOdInnychAgentow = new StalaUdajacaSrednia(0.3);
            //        var oceniacz = new AltOceniacz(miaraOdleglosci, srednieOdleglosciOdInnychAgentow, sasiedztwo);//*/new Oceniacz(miaraOdleglosci, srednieOdleglosciOdInnychAgentow, sasiedztwo);
            //        var stopienZachlannosci = 0.9;
            //        IPrzemieszczacz przemieszczacz;
            //        {
            //            var losowyPrzemieszczacz = new LosowyPrzemieszczacz(przestrzen, sasiedztwo, maszynaLosujaca);
            //            var zachlannyPrzemieszczacz = new ZachlannyPrzemieszczacz(stopienZachlannosci, losowyPrzemieszczacz,
            //                oceniacz, przestrzen, sasiedztwo, maszynaLosujaca);
            //            przemieszczacz = zachlannyPrzemieszczacz;
            //        }
            //        var okreslaczKlas =
            //            new OkreslaczKlas(sasiedztwo);
            //        //new OpoznionyOkreslaczKlas(czas, 4900, new GlobalnyPodobienstwowyOkreslaczKlas(odleglosci, sasiedztwo, czas, true) { MinProgLicznosci = 20 });
            //        //new OpoznionyOkreslaczKlas(czas, 4000, new OkreslaczKlas(sasiedztwo));
            //        AktywatorUwzglPodobienstwo sprawdzaczNiepodobienstwa = null;//*/ new AktywatorUwzglPodobienstwo(sasiedztwo, odleglosci, 15, 0.5);

            //        // Grupowanie właściwe
            //        while (!czas.CzyUplynal)
            //        {
            //            var ocenyPozycji = new Dictionary<(int, int), double>();
            //            for (int y = 0; y < przestrzen.DlugoscBoku; y++)
            //            {
            //                for (int x = 0; x < przestrzen.DlugoscBoku; x++)
            //                {
            //                    Mrowka aktMrowka = przestrzen[y][x];
            //                    if (aktMrowka != null)
            //                    {
            //                        var pozycja = (x, y);
            //                        var ocena = oceniacz.Ocen(aktMrowka, pozycja);
            //                        ocenyPozycji[pozycja] = ocena;
            //                    }
            //                }
            //            }

            //            var sumaOcenNaTenCzas = ocenyPozycji.Sum(kvp => kvp.Value);
            //            var sredniaOcenNaTenCzas = sumaOcenNaTenCzas / mrowki.Count;
            //            srednieDopasowaniaWCzasie[czas.Aktualny] = sredniaOcenNaTenCzas;

            //            var pozycjeDoZmianyKlasy = new List<(int, int)>();
            //            var pozycjeDoZmianyPozycji = new List<(int, int)>();
            //            foreach (var pozycja in ocenyPozycji.Keys)
            //            {
            //                var (x, y) = pozycja;
            //                var mrowka = przestrzen[y][x];
            //                var czyNiepodobneIstnieja = sprawdzaczNiepodobienstwa?.CzyAktywowac(mrowka, pozycja) ?? false;
            //                if (czyNiepodobneIstnieja || aktywator.CzyAktywowac(ocenyPozycji[pozycja]))
            //                {
            //                    pozycjeDoZmianyPozycji.Add(pozycja);
            //                }
            //                else
            //                {
            //                    pozycjeDoZmianyKlasy.Add(pozycja);
            //                }
            //            }

            //            foreach (var pozMrowki in pozycjeDoZmianyPozycji)
            //            {
            //                (int x, int y) = pozMrowki;
            //                var mrowka = przestrzen[y][x];
            //                przemieszczacz.Przemiesc(mrowka, pozMrowki);
            //            }

            //            Dictionary<Mrowka, int> noweKlasyMrowek = new Dictionary<Mrowka, int>();
            //            foreach (var pozMrowki in pozycjeDoZmianyKlasy)
            //            {
            //                (int x, int y) = pozMrowki;
            //                var mrowka = przestrzen[y][x];
            //                var nowaKlasa = okreslaczKlas.OkreslKlase(mrowka, pozMrowki);
            //                noweKlasyMrowek[mrowka] = nowaKlasa;
            //            }
            //            foreach (var mrowkaIKlasa in noweKlasyMrowek)
            //            {
            //                var mrowka = mrowkaIKlasa.Key;
            //                var klasa = mrowkaIKlasa.Value;
            //                mrowka.Klasa = klasa;
            //            }

            //            if (czas.Aktualny % 500 == 0)
            //            {
            //                Console.WriteLine($"Koniec iteracji {czas.Aktualny}.");
            //            }
            //            czas.Uplywaj();
            //        }
            //        stopwatch.Stop();

            //        Console.WriteLine();
            //        Console.WriteLine($"Pogrupowano! {stopwatch.ElapsedMilliseconds}ms");

            //        obrazPrzestrzeniId.Wyswietl(przestrzen);
            //        wyswietlaczId.Wyswietl(przestrzen);
            //        Console.WriteLine();

            //        obrazPrzestrzeniKlWynikowa.Wyswietl(przestrzen);
            //        wyswietlaczKlasa.Wyswietl(przestrzen);
            //        Console.WriteLine();

            //        obrazPrzestrzeniKlDocelowa.Wyswietl(przestrzen);
            //        wyswietlaczKlDocelowa.Wyswietl(przestrzen);

            //        if (!trybCichyWSensieStuprocentowoDoslownym)
            //        {
            //            Console.Beep();
            //        }
            //        if (czekajNaReadLine)
            //        {
            //            Console.ReadKey();
            //        }

            //        var liczbaMrowekWKazdejKlasieDocelowej = slownikKlasDocelowych.Values
            //            .GroupBy(klasa => klasa)
            //            .Select(grupa => new { Klucz = grupa.Key, Liczba = grupa.Count() })
            //            .ToDictionary(liczbaKlasy => liczbaKlasy.Klucz, liczbaKlasy => liczbaKlasy.Liczba);
            //        var grupyMrowek = mrowki.GroupBy(mrowka => mrowka.Klasa).ToList();

            //        using (var strumienWynikow = File.Create($"{sciezkaWynikow}\\{iPowtorzenia}-podsumowanie.txt"))
            //        using (var pisarz = new StreamWriter(strumienWynikow))
            //        {
            //            foreach (var grupa in grupyMrowek)
            //            {
            //                var liczbaMrowekWGrupie = grupa.Count();
            //                var faktyczneKlasy = grupa.GroupBy(mrowka => slownikKlasDocelowych[mrowka.Id]).ToList();
            //                var naglowek = $"Klasa wynikowa {grupa.Key}: {liczbaMrowekWGrupie} mrówek z {faktyczneKlasy.Count} klas docelowych:";
            //                Console.WriteLine(naglowek);
            //                pisarz.WriteLine(naglowek);

            //                foreach (var fakKlasa in faktyczneKlasy)
            //                {
            //                    var liczbaMrowekFakKlasy = fakKlasa.Count();
            //                    string klasaDocelowa = $"\tKlasa {fakKlasa.Key}: {fakKlasa.Count()} mrówek: ";
            //                    string procentDocelowej = $"\t\t({(liczbaMrowekFakKlasy / (double)liczbaMrowekWKazdejKlasieDocelowej[fakKlasa.Key]) * 100}% mrówek klasy docelowej,";
            //                    string procentWynikowej = $"\t\t{(liczbaMrowekFakKlasy / (double)liczbaMrowekWGrupie) * 100}% mrówek klasy wynikowej,";
            //                    string procentWszystkich = $"\t\t{(liczbaMrowekFakKlasy / (double)mrowki.Count) * 100}% wszystkich mrówek.";
            //                    Console.WriteLine(klasaDocelowa);
            //                    Console.WriteLine(procentDocelowej);
            //                    Console.WriteLine(procentWynikowej);
            //                    Console.WriteLine(procentWszystkich);
            //                    pisarz.WriteLine(klasaDocelowa);
            //                    pisarz.WriteLine(procentDocelowej);
            //                    pisarz.WriteLine(procentWynikowej);
            //                    pisarz.WriteLine(procentWszystkich);
            //                }

            //                pisarz.WriteLine();
            //                Console.WriteLine();
            //            }
            //            string ileToWszystkoTrwalo = $"Czas: {stopwatch.ElapsedMilliseconds}ms";
            //            Console.WriteLine(ileToWszystkoTrwalo);
            //            pisarz.WriteLine(ileToWszystkoTrwalo);
            //            pisarz.Flush();
            //        }

            //        using (var strumien = File.Create($"{sciezkaWynikow}\\{iPowtorzenia}-wyniki.a4c"))
            //        {
            //            var wyniki = new WynikAlgorytmu()
            //            {
            //                Mrowki = mrowki,
            //                Przestrzen = przestrzen,
            //                SlownikKlasDocelowych = slownikKlasDocelowych,
            //                S_x = sasiedztwo.ZasiegX,
            //                S_y = sasiedztwo.ZasiegY,
            //                CzasTrwaniaMs = stopwatch.ElapsedMilliseconds
            //            };
            //            var formater = new BinaryFormatter();
            //            formater.Serialize(strumien, wyniki);
            //        }

            //        if (czekajNaReadLine)
            //        {
            //            Console.ReadKey();
            //        }
            //    }

            //    if (!niePowaznieNawetBezBeepaNaSamiutenkiKoniec)
            //    {
            //        Console.Beep();
            //    }
        }

        private static void Wykonuj(string sciezkaWynikow, int idDoWyswietlaniaIWypisywaniaItd,
            bool trybCichy, bool czekajNaReadLine,
            KonfiguracjaGrupowania konfiguracja, List<Mrowka> mrowki, Dictionary<int, int> slownikKlasDocelowych,
            long czasMsNaPrzygotowania)
        {
                var maszynaLosujaca = konfiguracja.MaszynaLosujaca;

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

                Directory.CreateDirectory(sciezkaWynikow);
                var obrazPrzestrzeniIdPrzed = wylaczoneWyswietlanie; //new ObrazPrzestrzeni(ladnePedzelki, reprezentacjaId, $"{ticks}-id-przed.bmp");
                var obrazPrzestrzeniId = new ObrazPrzestrzeni(ladnePedzelki, reprezentacjaId, $"{sciezkaWynikow}\\{idDoWyswietlaniaIWypisywaniaItd}-id.bmp");
                var obrazPrzestrzeniKlDocelowa = new ObrazPrzestrzeni(ladnePedzelki, reprezentacjaKlDocelowa,
                    new SlownikowyOkreslaczPedzla(slownikKlasDocelowych, (mrowka) => mrowka.Id, 0), $"{sciezkaWynikow}\\{idDoWyswietlaniaIWypisywaniaItd}-klasa-docelowa.bmp");
                var obrazPrzestrzeniKlWynikowa = new ObrazPrzestrzeni(ladnePedzelki, reprezentacjaKlasa, $"{sciezkaWynikow}\\{idDoWyswietlaniaIWypisywaniaItd}-klasa-wynikowa.bmp");

                var wyswietlaczIdPrzed = wylaczoneWyswietlanie; //new WyswietlaczPrzestrzeni(reprezentacjaId);
                var wyswietlaczId = new WyswietlaczPrzestrzeni(reprezentacjaId);
                var wyswietlaczKlasa = new WyswietlaczPrzestrzeni(reprezentacjaKlasa);
                var wyswietlaczKlDocelowa = new WyswietlaczPrzestrzeni(reprezentacjaKlDocelowa);
                #endregion

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var przestrzen = konfiguracja.Przestrzen;
                var sasiedztwo = konfiguracja.Sasiedztwo;
                przestrzen.RozmiescMrowki(mrowki, maszynaLosujaca);

                stopwatch.Stop();
                wyswietlaczIdPrzed.Wyswietl(przestrzen);
                obrazPrzestrzeniIdPrzed.Wyswietl(przestrzen);

                Console.WriteLine();
                Console.WriteLine($"Grupowanie - {idDoWyswietlaniaIWypisywaniaItd + 1}...");
                stopwatch.Start();
                
                var czas = konfiguracja.Czas;
                var srednieDopasowaniaWCzasie = konfiguracja.SrednieDopasowania;
                var aktywator = konfiguracja.Aktywator;
                var srednieOdleglosciOdInnychAgentow = konfiguracja.SrednieOdleglosci;
                var oceniacz = konfiguracja.Oceniacz;
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

                if (!trybCichy)
                {
                    Console.Beep();
                }
                if (czekajNaReadLine)
                {
                    Console.ReadKey();
                }

                var liczbaMrowekWKazdejKlasieDocelowej = slownikKlasDocelowych.Values
                    .GroupBy(klasa => klasa)
                    .Select(grupa => new { Klucz = grupa.Key, Liczba = grupa.Count() })
                    .ToDictionary(liczbaKlasy => liczbaKlasy.Klucz, liczbaKlasy => liczbaKlasy.Liczba);
                var grupyMrowek = mrowki.GroupBy(mrowka => mrowka.Klasa).ToList();

                using (var strumienWynikow = File.Create($"{sciezkaWynikow}\\{idDoWyswietlaniaIWypisywaniaItd}-podsumowanie.txt"))
                using (var pisarz = new StreamWriter(strumienWynikow))
                {
                    foreach (var grupa in grupyMrowek)
                    {
                        var liczbaMrowekWGrupie = grupa.Count();
                        var faktyczneKlasy = grupa.GroupBy(mrowka => slownikKlasDocelowych[mrowka.Id]).ToList();
                        var naglowek = $"Klasa wynikowa {grupa.Key}: {liczbaMrowekWGrupie} mrówek z {faktyczneKlasy.Count} klas docelowych:";
                        Console.WriteLine(naglowek);
                        pisarz.WriteLine(naglowek);

                        foreach (var fakKlasa in faktyczneKlasy)
                        {
                            var liczbaMrowekFakKlasy = fakKlasa.Count();
                            string klasaDocelowa = $"\tKlasa {fakKlasa.Key}: {fakKlasa.Count()} mrówek: ";
                            string procentDocelowej = $"\t\t({(liczbaMrowekFakKlasy / (double)liczbaMrowekWKazdejKlasieDocelowej[fakKlasa.Key]) * 100}% mrówek klasy docelowej,";
                            string procentWynikowej = $"\t\t{(liczbaMrowekFakKlasy / (double)liczbaMrowekWGrupie) * 100}% mrówek klasy wynikowej,";
                            string procentWszystkich = $"\t\t{(liczbaMrowekFakKlasy / (double)mrowki.Count) * 100}% wszystkich mrówek.";
                            Console.WriteLine(klasaDocelowa);
                            Console.WriteLine(procentDocelowej);
                            Console.WriteLine(procentWynikowej);
                            Console.WriteLine(procentWszystkich);
                            pisarz.WriteLine(klasaDocelowa);
                            pisarz.WriteLine(procentDocelowej);
                            pisarz.WriteLine(procentWynikowej);
                            pisarz.WriteLine(procentWszystkich);
                        }

                        pisarz.WriteLine();
                        Console.WriteLine();
                    }
                    string ileToWszystkoTrwalo = $"Czas: {stopwatch.ElapsedMilliseconds + czasMsNaPrzygotowania}ms";
                    Console.WriteLine(ileToWszystkoTrwalo);
                    pisarz.WriteLine(ileToWszystkoTrwalo);
                    pisarz.Flush();
                }

                using (var strumien = File.Create($"{sciezkaWynikow}\\{idDoWyswietlaniaIWypisywaniaItd}-wyniki.a4c"))
                {
                    var wyniki = new WynikAlgorytmu()
                    {
                        Mrowki = mrowki,
                        Przestrzen = przestrzen,
                        SlownikKlasDocelowych = slownikKlasDocelowych,
                        S_x = sasiedztwo.ZasiegX,
                        S_y = sasiedztwo.ZasiegY,
                        CzasTrwaniaMs = stopwatch.ElapsedMilliseconds + czasMsNaPrzygotowania
                    };
                    var formater = new BinaryFormatter();
                    formater.Serialize(strumien, wyniki);
                }

                if (czekajNaReadLine)
                {
                    Console.ReadKey();
                }

                foreach (var mrowka in mrowki)
                {
                    mrowka.Klasa = mrowka.Id;
                }
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