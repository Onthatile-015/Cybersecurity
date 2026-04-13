using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG
{
    internal class ResponseHandler
    {

        // This method takes the user's input and generates an appropriate response based on keywords.
        public static string GetResponse(string input)
        {
            // If the input is empty or only contains whitespace, it returns a default message asking the user to rephrase.
            if (string.IsNullOrWhiteSpace(input))
                return "I didn't quite understand that. Could you rephrase?";

            input = input.ToLower();

            if (input.Contains("your name"))
                return " I am the Cybersecurity Awareness Bot.";

            if (input.Contains("how are you"))
                return "I'm functioning perfectly and ready to help you stay safe online!";

            if (input.Contains("purpose"))
                return "My purpose is to teach cybersecurity awareness.";

            if (input.Contains("what can i ask"))
                return "You can ask about password safety, phishing and safe browsing.";

            if (input.Contains("password"))
                return "You must always use strong passwords with symbols, numbers and avoid giving people your passwords.";

            if (input.Contains("phishing"))
                return "Phishing attacks it trickers users to reveal their personal information through fake emails or websites. Is like when someone trying to steal your lunch money but online. So stay safe by verifying sender info";

            if (input.Contains("browsing"))
                return "You must always use secure websites such as https and avoid clicking suspicious links.";

            return "I didn't quite understand that. Could you rephrase?";

            
        }


       
    }











}

