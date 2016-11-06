using System;

namespace VLC_Control
{
    class Program
    {
        static void Main()
        {
            Request req = new Request();
            new SpeechRecognizer("textgrammar.grxml", req);

            Console.ReadLine();
        }

    }
}
