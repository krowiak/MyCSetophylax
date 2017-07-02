using MyCSetophylax.KonkretneOdleglosci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.SrednieOdleglosci
{
    public class SredniaPodzbioru : ISrednieOdleglosci
    {
        private readonly NajsredniejszaOdleglosc srednia;

        public SredniaPodzbioru(IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> odleglosci, double czescMrowek, Random maszynaLosujaca)
        {
            srednia = TworzSrednia(mrowki, odleglosci, czescMrowek, maszynaLosujaca);
        }

        private NajsredniejszaOdleglosc TworzSrednia(IEnumerable<Mrowka> mrowki, IOdleglosc<Mrowka> odleglosci,
            double czescMrowek, Random maszynaLosujaca)
        {
            var listaMrowek = mrowki.ToList();
            var liczbaMrowek = listaMrowek.Count;
            var liczbaDoWylosowania = Math.Round(czescMrowek * liczbaMrowek);
            
            List<Mrowka> wylosowaneMrowki = new List<Mrowka>();
            for (int i = 0; i < liczbaDoWylosowania; i++)
            {
                var indeks = maszynaLosujaca.Next(liczbaMrowek);
                var mrowka = listaMrowek[indeks];
                if (!wylosowaneMrowki.Contains(mrowka))
                {
                    wylosowaneMrowki.Add(mrowka);
                }
                else
                {
                    i--;
                }
            }

            return new NajsredniejszaOdleglosc(wylosowaneMrowki, odleglosci);
        }


        public double OkreslSredniaOdleglosc(Mrowka sprawdzanaMrowka)
        {
            return srednia.OkreslSredniaOdleglosc(sprawdzanaMrowka);
        }
    }
}
