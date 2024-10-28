using System;

namespace MiinaharavaKonsoli 
{
    /// <summary>
    /// Pelin pääluokka
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Pelin alku, joka pistää pelin käyntiin.
        /// </summary>
        static void Main()
        {
            Console.WriteLine("Tervetuloa pelaamaan miinaharavaa! \nLiiku ympäriinsä käyttäen nuolinäppäimiä, ja paina 'C' klikataksesi ruutua tai 'F' laittaaksesi lipun.\n\nPaina Enter aloittaaksesi.");
            Console.ReadLine();

            bool peliJatkuu = true;

            while (peliJatkuu)
            {
                MiinaharavaPeli peli = new MiinaharavaPeli();
                peli.Start();

                Console.WriteLine("Haluatko jatkaa pelaamista? (K/E)");

                while (true)
                {
                    Console.Write(">> ");
                    switch (Console.ReadLine()?.ToLower()) //tämä on nullable varmuuden vuoksi että ei tule virheitä 
                    {
                        case "k":
                            peliJatkuu = true;
                            break;

                        case "e":
                            peliJatkuu = false;
                            break;

                        default:
                            Console.WriteLine("Syötä K tai E.");
                            continue;
                    }
                    break;
                }
            }
        }


    }
}