using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.Dane
{
    public class StandaryzatorZScore
    {
        // https://www.mathsisfun.com/data/standard-deviation-formulas.html
        private double OkreslOdchylenieStandardoweWymiaru(double[] dane)
        {
            var liczbaDanych = dane.Length;
            var srednia = dane.Average();
            var suma = dane.Sum(x_i => (x_i - srednia) * (x_i - srednia));
            return Math.Sqrt(suma / liczbaDanych);
        }

        // http://sebastianraschka.com/Articles/2014_about_feature_scaling.html#about-standardization
        private double OkreslZScoreWartosci(double wartosc, double srednia, double odchylenieStandardowe)
        {
            return (wartosc - srednia) / odchylenieStandardowe;
        }

        private double[] OkreslZScoreWymiaru(double[] dane)
        {
            var srednia = dane.Average();
            var odchylenie = OkreslOdchylenieStandardoweWymiaru(dane);
            return dane.Select(x_i => OkreslZScoreWartosci(x_i, srednia, odchylenie)).ToArray();
        }

        private double[] PobierzWymiar(int indeksWymiaru, IEnumerable<double[]> dane)
        {
            var listaDanych = dane.ToList();
            var wybranyWymiar = new double[listaDanych.Count];
            for (int i = 0; i < listaDanych.Count; i++)
            {
                wybranyWymiar[i] = listaDanych[i][indeksWymiaru];
            }
            return wybranyWymiar;
        }

        private IEnumerable<double[]> PrzeksztalcWymiaryNaObiekty(IEnumerable<double[]> wymiary)
        {
            var listaWymiarow = wymiary.ToList();
            var liczbaWymiarow = listaWymiarow.Count;
            var liczbaObiektow = listaWymiarow.First().Length;
            var obiekty = new List<double[]>(liczbaObiektow);

            for (int i = 0; i < liczbaObiektow; i++)
            {
                obiekty.Add(new double[liczbaWymiarow]);
            }

            for (int i = 0; i < listaWymiarow.Count; i++)
            {
                var wymiar = listaWymiarow[i];
                for (int j = 0; j < liczbaObiektow; j++)
                {
                    obiekty[j][i] = wymiar[j];
                }                
            }

            return obiekty;
        }

        public IEnumerable<double[]> Standaryzuj(IEnumerable<double[]> dane)
        {
            // UWAGA
            // O NIEEEE
            // Spore zmiany w stusunku do wcześniejszej wersji (by nie mutować danych),
            // więc być BARDZO CZUJNYM, czy działa tak samo, jak w F#.
            // (także: kiedyś w F# było też skalowanie min-max, jako opcja zamiast standaryzacji, ale chyba odrzucona? 
            // też opisane w http://sebastianraschka.com)

            var listaDanych = dane.ToList();
            var liczbaWymiarow = listaDanych.First().Length;
            var znormalizowaneWymiary = new List<double[]>(liczbaWymiarow);
            for (int i = 0; i < liczbaWymiarow; i++)
            {
                var nieznormalizowanyWymiar = PobierzWymiar(i, listaDanych);
                var znormalizowanyWymiar = OkreslZScoreWymiaru(nieznormalizowanyWymiar);
                znormalizowaneWymiary.Add(znormalizowanyWymiar);
            }
            return PrzeksztalcWymiaryNaObiekty(znormalizowaneWymiary);
        }
    }
}

