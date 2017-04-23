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
            using (var strumienDanych = File.OpenRead(@"PlikiDanych\daneprostokatow.txt"))//gupieDane.txt"))// irisBezSmieci.data"))
            {
                var parser = new ParserDanych();
                var niestandaryzowaneDane = parser.ParsujDane(strumienDanych);
                var zScore = new StandaryzatorZScore();
                IEnumerable<double[]> wektoryDanych = zScore.Standaryzuj(niestandaryzowaneDane);
                mrowki = wektoryDanych.Select((wektor, indeks) => new Mrowka(indeks, wektor)).ToList();
            }

            var sX = 1;
            var sY = 1;
            var przestrzen = Przestrzen.StworzPrzestrzenDla(mrowki.Count);
            var sasiedztwo = new Sasiedztwo(przestrzen, sX, sY);
            var reprezentacjaId = new ReprezentacjaIdMrowki();
            var wyswietlaczId = new WyswietlaczPrzestrzeni(reprezentacjaId);
            przestrzen.RozmiescMrowki(mrowki, maszynaLosujaca);
            wyswietlaczId.Wyswietl(przestrzen);

            Console.WriteLine();
            Console.WriteLine("Grupowanie...");

            // Przygotowanie całego śmiecia
            var liczbaIteracji = 2000;
            var czas = new Czas(liczbaIteracji);
            var srednieDopasowaniaWCzasie = new SrednieDopasowaniaMrowek();
            var kLambda = 1.0;
            var presja = new StalaPresja(); //new PresjaZaleznaOdCzasu(czas, srednieDopasowaniaWCzasie, kLambda);
            var bazowePrawdopAktywacji = 0.1;
            var aktywator = new Aktywator(czas, maszynaLosujaca, bazowePrawdopAktywacji, presja);
            var euklides = new OdlegloscEuklidesowa();
            var odleglosci = new OdleglosciPomiedzyMrowkami(mrowki, euklides);
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
            var okreslaczKlas = new OkreslaczKlas(sasiedztwo);

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

                foreach (var pozMrowki in pozycjeDoZmianyKlasy)
                {
                    (int x, int y) = pozMrowki;
                    var mrowka = przestrzen[y][x];
                    var nowaKlasa = okreslaczKlas.OkreslKlase(mrowka, pozMrowki);
                    mrowka.Klasa = nowaKlasa;
                }

                czas.Uplywaj();
            }

            Console.WriteLine();
            Console.WriteLine("Pogrupowano!");
            var reprezentacjaKlasa = new ReprezentacjaKlasaMrowki();
            var wyswietlaczKlasa = new WyswietlaczPrzestrzeni(reprezentacjaKlasa);
            wyswietlaczId.Wyswietl(przestrzen);
            Console.WriteLine();
            wyswietlaczKlasa.Wyswietl(przestrzen);

            Console.ReadKey();
        }
    }
}

//let mutable aktPrzestrzen = przestrzen

//    for t=1 to liczbaIteracji do
//        let nastepnaPrzestrzen = Array2D.copy aktPrzestrzen
//        let mutable zmianyKlas = []

//        let polaZMrowkami =
//            PrzestrzenNaSeqPol aktPrzestrzen 
//            |> Seq.filter(PobierzZawartosc aktPrzestrzen >> Option.isSome)
//            |> Seq.toList

//        let ocenySrodowisk =
//            polaZMrowkami
//            |> List.map(fun pole -> PobierzZawartosc aktPrzestrzen pole |> Option.get, pole)
//            |> List.map(fun (mrowka, pole) -> mrowka, funOceny aktPrzestrzen mrowka pole)
//            |> Map.ofList
//        srednieOcenyDlaT.[t] <- ocenySrodowisk |> Map.toSeq |> Seq.sumBy (fun (_, ocena) -> ocena)

//        //////////
//        /// Klasy zaczęły się zmieniać dla mrówek bez sąsiadów!
//        /// Najpierw jest obliczana ocena, potem następuje przemieszczanie, więc czasem mrówka z oceną > 0 zostaje sama.
//        /// Cóż za niefortunne zdarzenie.
//        //////////
//        for pole in polaZMrowkami do
//            let x, y = pole
//            let mrowka = aktPrzestrzen.[x, y] |> Option.get
//            let ocena = Map.find mrowka ocenySrodowisk
//            let pAktywacji = funPrawdopAktywacji ocena t
//            let sasiedztwo = funSasiedztwa pole
//            if los.NextDouble() <= pAktywacji
//            then
//                funPrzemieszczenia nastepnaPrzestrzen sasiedztwo pole
//                //zmianyKlas <- (mrowka, mrowka.Id)::zmianyKlas  // str. 7 - "class label same as original one"
//            else zmianyKlas<- (mrowka, funKlasy aktPrzestrzen sasiedztwo mrowka)::zmianyKlas

//        for (mrowka, klasa) in zmianyKlas do
//            klasyMrowek.[mrowka] <- klasa
//        aktPrzestrzen<- nastepnaPrzestrzen

//        if debug then
//            Console.Clear() |> ignore
//            printfn "Iteracja %i zakończona" t
//            WypiszKlasyWPrzestrzeni klasyMrowek aktPrzestrzen
//            printfn ""
//            System.Threading.Thread.Sleep(25)
//            //Console.ReadKey() |> ignore

//    aktPrzestrzen, klasyMrowek