using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PROG
{
    internal class ChatBot
    {


        private string userName;
        private string message = " How can i help you today? ";

        public void ShowDivider()
        { 
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");
        Console.ResetColor();


        }
            

        public void StartChat()
        {

            Console.WriteLine("-------------CHAT SSESSION-------------");
            //User input for name
            Console.ForegroundColor = ConsoleColor.Yellow;
            TypingEffect("What is your name? ");
            userName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userName))
                userName = "User";
            Console.ResetColor();


            Console.ForegroundColor = ConsoleColor.Yellow;
            TypingEffect("\nWelcome " + userName + "!" + message);
                Console.ResetColor();
            ChatLoop();
        }





        private void ChatLoop()
        {
            while (true)
            {
                //Added dividers for structure
                ShowDivider();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("\nYou: ");
                string input = Console.ReadLine();
                Console.ResetColor();


                // Check if user wants to exit
                if (input != null && input.ToLower() == "bye")
                {
                    ShowDivider();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    TypingEffect("Bot: Goodbye! Stay safe online");
                    Console.ResetColor();
                    break; // it exits the loop and closes program
                }
                Console.ForegroundColor = ConsoleColor.Blue;
                string response = ResponseHandler.GetResponse(input);
                TypingEffect("Bot: " + response);
                Console.ResetColor();
            }


        
        }
        // Added a method to create a typing effect for the bot's responses, making it feel more interactive and engaging.
        public void TypingEffect(string message)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(30);
            }
            Console.WriteLine();



        }

        }
}
    