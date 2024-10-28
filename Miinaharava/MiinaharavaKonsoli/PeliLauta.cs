using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MiinaharavaKonsoli
{
    /// <summary>
    /// Miinaharavapelin pelilaudan logiikka
    /// </summary>
    internal class PeliLauta
    {
        /// <summary> Merkki, joka esittää tyhjää ruutua pelilaudalla. </summary>
        private const char TYHJA = '░';

        /// <summary> Merkki, joka esittää miinaa pelilaudalla. </summary>
        private const char MIINA = '¤';

        /// <summary> Merkki, joka esittää lippua pelilaudalla. </summary>
        private const char LIPPU = 'F';

        /// <summary> Pelilaudan matriisi.</summary>
        private char[,] pelilautaMatriisi; 

        /// <summary> Matriisi miinojen sijoittelulle. </summary>
        private bool[,] miinat;

        /// <summary> Pelilaudan koko, eli leveys ja korkeus. Pelilauta on neliö </summary>
        private int koko;

        ///<summary> Pelin etenemistä kuvaava luku. Kun arvo on 0, peli päättyy </summary>
        private int avaamattomatRuudutLkm;

        /// <summary>
        /// Luo uuden PeliLauta-olion ja alustaa pelilaudan.
        /// </summary>
        /// <param name="koko"> Pelilaudan leveys ja korkeus. </param>
        /// <param name="miinojenMaara"> Pelilaudalle asetettavien miinojen määrä. </param>
        public PeliLauta(int koko, int miinojenMaara)
        {
            this.koko = koko;
            pelilautaMatriisi = new char[koko, koko];
            miinat = new bool[koko, koko];
            avaamattomatRuudutLkm = koko * koko;

            AlustaPelilauta(miinojenMaara);
        }


        /// <summary>
        /// Alustaa pelilaudan, ja luo myös miinat
        /// </summary>
        /// <param name="miinojenMaara"> Asetettavien miinojen määrä. </param>
        private void AlustaPelilauta(int miinojenMaara)
        {
            //alustaa pelilaudan
            for (int y = 0; y < koko; y++)
            {
                for (int x = 0; x < koko; x++)
                {
                    pelilautaMatriisi[y, x] = TYHJA;
                }
            }

            //Asettaa miinat
            Random rnd = new Random();

            for (; miinojenMaara > 0; miinojenMaara--)
            {
                int randomN1 = rnd.Next(0, koko);
                int randomN2 = rnd.Next(0, koko);

                //varmistaa, että oikea määrä miinoja menee kentälle ilman päällekkäisyyksiä
                while (miinat[randomN1, randomN2] == true)
                {
                    randomN1 = rnd.Next(0, koko);
                    randomN2 = rnd.Next(0, koko);
                }

                miinat[randomN1, randomN2] = true;
            }
        }


        /// <summary>
        /// Tarkistaa, onko miina, jos ei ole niin näyttää naapuri miinojen määrän.
        /// </summary>
        /// <param name="input">Lista, jossa on kordinaatit sekä C (tarkistaa kohdan) tai F (laittaa lipun)</param>
        /// <returns>
        /// Kaksi bool arvoa:
        ///     valmis (bool): Onko peli päättynyt, vai ei.
        ///     voitto (bool?): Onko peli voitettu, vai ei. Palauttaa null, jos ei ratkennut.
        /// </returns>
        public (bool valmis, bool? voitto) TarkistaMiina(string[] input)
        {
            int inputY = int.Parse(input[0]);
            int inputX = int.Parse(input[1]);
            string inputTyyppi = input[2].ToLower();

            switch (inputTyyppi)
            {
                case "c":
                    if (miinat[inputY, inputX] == true && pelilautaMatriisi[inputY, inputX] != LIPPU) 
                    {
                        PaljastaMiinat();
                        return (true, false); //palauttaa että peli on hävitty, koska kohdassa oli miina
                    }
                    else if (pelilautaMatriisi[inputY, inputX] != LIPPU)
                    {
                        PaljastaNaapurit(inputX, inputY);
                    }
                    break;

                case "f":
                    if (miinat[inputY, inputX] == true && pelilautaMatriisi[inputY, inputX] == TYHJA)
                    {
                        avaamattomatRuudutLkm -= 1; // jos asetetaan lippu oikeaan kohtaan, pelin valmius etenee, eli muuttuja avaamattomien ruutujen lkm vähenee
                    }
                    else if (miinat[inputY, inputX] == true && pelilautaMatriisi[inputY, inputX] == LIPPU)
                    {
                        avaamattomatRuudutLkm += 1; // jos jostakin syystä käyttäjä ottaa lipun pois oikeasta kohdasta, avaamattomien lkm kasvaa, ja muuttujaan lisätään 1
                    }

                    //asettaa lipun paikalle tai poistaa lipun tarpeen mukaan, avattuihin kohtiin ei voi asettaa lippua
                    if (pelilautaMatriisi[inputY, inputX] == TYHJA || pelilautaMatriisi[inputY, inputX] == LIPPU)
                    {
                        pelilautaMatriisi[inputY, inputX] = (pelilautaMatriisi[inputY, inputX] == TYHJA) ? LIPPU : TYHJA;
                    }

                    break;
            }

            if (avaamattomatRuudutLkm <= 0)
            {
                return (true, true); //palauttaa että peli on voitettu, koska kaikki ruudut on käyty läpi
            }

            return (false, null); //peli jatkuu, eikä ole vielä voitettu
        }


        /// <summary>
        /// Piirtää pelilaudan, joka on jo alustettu.
        /// </summary>
        /// <param name="kursoriX">Kursorin X kordinaatti</param>
        /// <param name="kursoriY">Kursorin y kordinaatti</param>
        public void PiirraPelilauta(int kursoriX, int kursoriY)
        {
            try
            {
                Console.Write("      ");

                // Piirtää x-akselin kordinaatit ylälaitaan
                // Lisää vähemmän välilyöntejä, jos koordinaatti on suurempi kuin 10, että tulostuisi tasaisesti
                for (int xAkseliKohta = 0; xAkseliKohta < koko; xAkseliKohta++)
                {
                    if (xAkseliKohta < 10) 
                    {
                        Console.Write("   " + xAkseliKohta + " ");
                    }
                    else 
                    { 
                        Console.Write("  " + xAkseliKohta + " ");
                    }
                }

                Console.WriteLine("\n\n");

                for (int yAkseliKohta = 0; yAkseliKohta < koko; yAkseliKohta++)
                {
                    // piirtää Y-akselin kordinaatit vasemmalle, välilyöntien määrässä sama idea kun x-akselilla
                    if (yAkseliKohta < 10)
                    {
                        Console.Write(yAkseliKohta.ToString() + "     ");
                    }
                    else 
                    { 
                        Console.Write(yAkseliKohta.ToString() + "    ");
                    }

                    //piirtää ruudut, ja antaa niille myös niiden mukaisen värin
                    for (int xAkseliKohta = 0; xAkseliKohta < koko; xAkseliKohta++)
                    {
                        Console.Write("   ");

                        switch (pelilautaMatriisi[yAkseliKohta, xAkseliKohta])
                        {
                            case '0':
                                Console.ForegroundColor = ConsoleColor.Black;
                                break;

                            case '1':
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;

                            case '2':
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;

                            case '3':
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;

                            case '4':
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                break;

                            case '5':
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                break;

                            case '6':
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                break;

                            case '7':
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                break;

                            case '8':
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                break;

                            case MIINA:
                                Console.BackgroundColor = ConsoleColor.Red;
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;

                            case LIPPU:
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.ForegroundColor = ConsoleColor.Black;
                                break;

                            default:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                        }

                        if (yAkseliKohta == kursoriY && xAkseliKohta == kursoriX)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkYellow;
                        }

                        Console.Write(pelilautaMatriisi[yAkseliKohta, xAkseliKohta]);
                        
                        //palauttaa värit normaaliksi, jotta ei väritä ruutujen välejä
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                    }

                    Console.WriteLine("\n");
                }
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Pelilautaa ei ole luotu");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        /// <summary>
        /// Näyttää annetun kohdan kaikki naapurit matriisissa
        /// </summary>
        /// <param name="xKohta">X-kohta matriisissa.</param>
        /// <param name="yKohta">Y-kohta matriisissa.</param>
        private void PaljastaNaapurit(int xKohta, int yKohta)
        {
            //jos kohtaa ei ole paljastettu, peli etenee
            if (pelilautaMatriisi[yKohta, xKohta] == TYHJA)
            {
                avaamattomatRuudutLkm -= 1;
            }

            char naapurit = Char.Parse(LaskeNaapuritMatriisissa(xKohta, yKohta).ToString()); // muunnetaan chariksi, jotta voidaan verrata ja asettaa pelilautaan
            pelilautaMatriisi[yKohta, xKohta] = naapurit;

            //Tarkistaa naapureiden naapurit, jos tämä kohta on 0
            if (naapurit == '0')
            {
                for (int y = Math.Max(0, yKohta - 1); y <= Math.Min(yKohta + 1, koko - 1); y++)
                {
                    for (int x = Math.Max(0, xKohta - 1); x <= Math.Min(xKohta + 1, koko - 1); x++)
                    {
                        //Jos naapuri on 0, tarkistaa senkin naapurit
                        if (miinat[y, x] == false && pelilautaMatriisi[y, x] != naapurit)
                        {
                            PaljastaNaapurit(x, y);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Laskee annetun kohdan naapurit matriisissa
        /// </summary>
        /// <param name="xKohta">X-kohta matriisissa.</param>
        /// <param name="yKohta">Y-kohta matriisissa.</param>
        /// <returns>Naapurien lukumäärä annetussa kohdassa.</returns>
        private int LaskeNaapuritMatriisissa(int xKohta, int yKohta)
        {
            int naapurit = 0;

            for (int y = Math.Max(0, yKohta - 1); y <= Math.Min(yKohta + 1, koko - 1); y++)
            {
                for (int x = Math.Max(0, xKohta - 1); x <= Math.Min(xKohta + 1, koko - 1); x++)
                {
                    if (miinat[y, x] == true && (y != yKohta || x != xKohta))
                    {
                        naapurit++;
                    }
                }
            }

            return naapurit;
        }


        /// <summary>
        /// Laittaa miinat näkyviin pelilaudalle.
        /// </summary>
        private void PaljastaMiinat()
        {
            for (int y = 0; y < koko; y++)
            {
                for (int x = 0; x < koko; x++)
                {
                    pelilautaMatriisi[y, x] = (miinat[y, x] == true) ? MIINA : pelilautaMatriisi[y, x];
                }
            }
        }


    }
}