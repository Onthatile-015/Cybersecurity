using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace PROG
{
    internal class VoicePlayer
    {
        //Added a method to play the greeting sound when the program starts.
        public static void PlayGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("greeting.wav");
                player.PlaySync();
            }
            catch
            {
                //This is displayed when a program is run without the greeting.wav file in the same directory.
                Console.WriteLine("Voice greeting could not be played.");
            }
        }
    }



}

