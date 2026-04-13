using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PROG
{
    internal class Program
    {
        static void Main(string[] args)
        {

                Console.Title = "Cybersecurity Awareness Bot";

                // Calling the voice greeting
                VoicePlayer.PlayGreeting();

                // Showing ASCII logo
                AsciiArt.DisplayLogo();


                // Starting chatbot
                ChatBot bot = new ChatBot();
                bot.StartChat();
            }

            static void TypingEffect(string message)
            {
                foreach (char c in message)
                {
                    Console.Write(c);
                    Thread.Sleep(40);
                }
                Console.WriteLine();





















        }
    }
}
