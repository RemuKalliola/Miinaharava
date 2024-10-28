using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiinaharavaKonsoli
{
    /// @author rekallio
    /// @version 20.11.2023
    /// <summary>
    ///     Vanha kunnon miinaharava. 
    ///     Pelin tavoitteena on löytää kaikki miinat ja asettaa niiden kohdalle lippu, sekä kääntää kaikki kohdat joissa ei miinaa ole.
    ///     Kun ruudun kääntää, se näyttää naapuroivien miinojen määrän.
    /// </summary>
    internal class MiinaharavaPeli
    {
        /// <summary> Pelilaudan X ja Y koko </summary>
        int pelilautaKoko;

        /// <summary> Miinojen määrä laudalla </summary>
        int miinojenMaara;

        /// <summary>
        /// Konstruktori kysyy vaikeustason ja asettaa pelilaudan koon ja miinojen määrän.
        /// </summary>
        public MiinaharavaPeli() 
        {
            (pelilautaKoko, miinojenMaara) = VaikeustasoValikko();
        }

        /// <summary>
        /// Aloittaa Miinaharava pelin
        /// </summary>
        public void Start()
        {
            PeliLauta peliLauta = new PeliLauta(pelilautaKoko, miinojenMaara); 

            bool valmis = false; //false, niin pitkään kun peli jatkuu
            bool? voitto = null; //onko peli voitettu, kun se päättyy, defaultisti null

            int cursorX = 0; 
            int cursorY = 0;

            Stopwatch peliAjanotto = new Stopwatch();
            peliAjanotto.Start();

            //gameplay silmukka
            while (!valmis)
            {
                Console.Clear();
                peliLauta.PiirraPelilauta(cursorX,cursorY);

                Console.WriteLine("X Y >> " + cursorX.ToString() +", "+ cursorY.ToString()); // Tulostaa kordinaatit sivun alalaitaan

                ConsoleKeyInfo keyInfo = Console.ReadKey(); //tarkistaa, mitä nappia käyttäjä painaa
                ConsoleKey key = keyInfo.Key;

                //tarkistaa kohdan, jossa kursori liikkuu, sekä painaako käyttäjä c vai f, ja muuttaa pelilaudan tietoja sen mukaan
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (cursorY > 0)
                            cursorY--;
                        break;

                    case ConsoleKey.DownArrow:
                        if (cursorY < pelilautaKoko - 1)
                            cursorY++;
                        break;

                    case ConsoleKey.LeftArrow:
                        if (cursorX > 0)
                            cursorX--;
                        break;

                    case ConsoleKey.RightArrow:
                        if (cursorX < pelilautaKoko - 1)
                            cursorX++;
                        break;

                    case ConsoleKey.C:
                        (valmis, voitto) = peliLauta.TarkistaMiina(new string[] { cursorY.ToString(), cursorX.ToString(), "C" });
                        break;

                    case ConsoleKey.F:
                        (valmis, voitto) = peliLauta.TarkistaMiina(new string[] { cursorY.ToString(), cursorX.ToString(), "F" });
                        break;

                    case ConsoleKey.Escape: //lopettaa pelin, jos käyttäjä painaa esc
                        valmis = true;
                        break;
                }
            }

            peliAjanotto.Stop();
            Console.Clear();
            peliLauta.PiirraPelilauta(-1, -1); //piirtää pelilaudan ilman kursoria lopuksi, asettamalla kursorin kohta pois pelilaudalta

            switch (voitto)
            {
                case true:
                    Console.WriteLine("Voitit pelin!");
                    Console.Write("Sinulla kului aikaa: " + Math.Round(peliAjanotto.Elapsed.TotalSeconds).ToString() + " sekuntia.\n\n");
                    break;

                case false:
                    Console.WriteLine("Hävisit pelin. :(\n");
                    Console.Write("Sinulla kului aikaa: " + Math.Round(peliAjanotto.Elapsed.TotalSeconds).ToString() + " sekuntia.\n\n");
                    break;

                case null:
                    Console.WriteLine("Peli keskeytetty");
                    break;
            }
        }


        /// <summary>
        /// Pyytää pelaajaa asettamaan vaikeustason, ja pistää sen mukaan miinojen määrän ja pelilaudan koon
        /// </summary>
        /// <returns>
        /// Kaksi int arvoa:
        ///     pelilaudanKoko: Pelilaudan koko annetun vaikeustason perusteella, 
        ///     miinojenMaara: Pelilaudalle asetettavien miinojen määrä 
        /// </returns>
        private (int pelilaudanKoko, int miinojenMaara) VaikeustasoValikko() 
        {
            string? vaikeusTaso = ""; 
            while (vaikeusTaso != "1" && vaikeusTaso != "2" && vaikeusTaso != "3")
            {
                Console.Clear();
                Console.WriteLine("Minkä vaikeustason haluat: Helppo(1), Keskivaikea(2), Vaikea(3)");
                Console.Write("\nSyötä 1, 2 tai 3 ja paina ENTER >> ");

                vaikeusTaso = Console.ReadLine();
            }

            switch (vaikeusTaso) 
            {
                case "1":
                    return (9, 9);

                case "2":
                    return (11, 15);

                case "3":
                    return (12, 25);

                default:
                    throw new Exception("Jokin meni pieleen vaikeustasoa asettaessa.");
            }
        }


    }
}
