using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCSetophylax.PrzestrzenZyciowa.Wyswietlanie.Obraz
{
    public class ObrazPrzestrzeni : IWyswietlaczPrzestrzeni
    {
        private readonly Dictionary<int, Brush> pedzleKlas;
        private readonly List<Brush> dostepnePedzle;
        private readonly string sciezkaDoZapisu;
        private readonly IReprezentacjaPola reprezentacjaPolaMrowki;
        private readonly IOkreslaczPedzla okreslaczPedzla;

        public ObrazPrzestrzeni(IEnumerable<Brush> dostepnePedzle, 
            IReprezentacjaPola reprezentacjaPolaMrowki, 
            IOkreslaczPedzla okreslaczPedzla,
            string sciezkaDoZapisu)
        {
            pedzleKlas = new Dictionary<int, Brush>();
            this.dostepnePedzle = dostepnePedzle.ToList();
            this.okreslaczPedzla = okreslaczPedzla;
            this.sciezkaDoZapisu = sciezkaDoZapisu;
            this.reprezentacjaPolaMrowki = reprezentacjaPolaMrowki;
        }

        public ObrazPrzestrzeni(IEnumerable<Brush> dostepnePedzle,
            IReprezentacjaPola reprezentacjaPolaMrowki,
            string sciezkaDoZapisu)
            : this(dostepnePedzle, reprezentacjaPolaMrowki, new KlasowyOkreslaczPedzla(0), sciezkaDoZapisu)
        {
        }

        public int DlugoscBokuPola
        {
            get;
            set;
        } = 20;

        public Brush PedzelTekstu
        {
            get;
            set;
        } = Brushes.Black;

        public Font CzcionkaTekstu
        {
            get;
            set;
        } = SystemFonts.DefaultFont;

        public void Wyswietl(Przestrzen przestrzen)
        {
            var dlugoscBokuWPikselach = przestrzen.DlugoscBoku * DlugoscBokuPola;
            using (Bitmap bitmapa = new Bitmap(dlugoscBokuWPikselach, dlugoscBokuWPikselach))
            using (Graphics rysownik = Graphics.FromImage(bitmapa))
            {
                rysownik.Clear(Color.White);

                for (int y = 0; y < przestrzen.DlugoscBoku; y++)
                {
                    for (int x = 0; x < przestrzen.DlugoscBoku; x++)
                    {
                        if (przestrzen[y][x] is Mrowka mrowka)
                        {
                            int posX = x * DlugoscBokuPola;
                            int posY = y * DlugoscBokuPola;
                            rysownik.FillRectangle(OkreslKolor(mrowka), posX, posY, DlugoscBokuPola, DlugoscBokuPola);
                            rysownik.DrawString(reprezentacjaPolaMrowki.Okresl(mrowka), CzcionkaTekstu, PedzelTekstu, posX, posY);
                        }
                    }
                }

                for (int y = 0; y < przestrzen.DlugoscBoku; y++)
                {
                    rysownik.DrawLine(Pens.Black,
                        new Point(0, y * DlugoscBokuPola),
                        new Point(przestrzen.DlugoscBoku * DlugoscBokuPola, y * DlugoscBokuPola));
                }
                for (int x = 0; x < przestrzen.DlugoscBoku; x++)
                {
                    rysownik.DrawLine(Pens.Black,
                        new Point(x * DlugoscBokuPola, 0),
                        new Point(x * DlugoscBokuPola, przestrzen.DlugoscBoku * DlugoscBokuPola));
                }

                rysownik.Flush();
                bitmapa.Save(sciezkaDoZapisu);
            }
        }

        private Brush OkreslKolor(Mrowka mrowka)
        {
            int bazaIdPedzla = okreslaczPedzla.ZwrocBazeDoOkresleniaPedzla(mrowka);
            if (pedzleKlas.TryGetValue(bazaIdPedzla, out var znanyKolor))
            {
                return znanyKolor;
            }

            var idKoloru = bazaIdPedzla % dostepnePedzle.Count;
            var nowyKolor = dostepnePedzle[idKoloru];
            if (pedzleKlas.ContainsValue(nowyKolor))
            {
                if (pedzleKlas.Count >= dostepnePedzle.Count)
                {
                    return znanyKolor;
                }
                else
                {
                    nowyKolor = dostepnePedzle.First(kolor => !pedzleKlas.Values.Contains(kolor));
                    pedzleKlas[bazaIdPedzla] = nowyKolor;
                }
            }
            else
            {
                pedzleKlas[bazaIdPedzla] = nowyKolor;
            }

            return nowyKolor;
        }
    }
}
