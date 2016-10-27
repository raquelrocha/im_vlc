using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VLC_Control
{
    class SpeechRecognizer
    {
        // Create a new SpeechRecognitionEngine instance.

        private SpeechRecognitionEngine sre;
        private API ops = new API();

        public SpeechRecognizer()
        {
            sre = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pt-PT"));
            sre.SetInputToDefaultAudioDevice();


            Choices operations = new Choices();
            operations.Add(new string[] { "parar música","parar","música seguinte","seguinte", "música anterior", "tocar", "pausar" });
            
            
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(operations);

            gb.Culture = new System.Globalization.CultureInfo("pt-PT");
            // Create the Grammar instance.
            Grammar g = new Grammar(gb);

            sre.LoadGrammar(g);

            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);

            sre.RecognizeAsync(RecognizeMode.Multiple);

            Console.WriteLine("Starting asynchronous speech recognition... ");

            // Keep the console window open.
            while (true)
            {
                Console.ReadLine();
            }
        }

        void doOperations(WebRequest operation) {
            // Create a request for the URL. 
            WebRequest request = operation;
            var credential = Convert.ToBase64String(Encoding.Default.GetBytes(":123456"));
            request.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            //Console.WriteLine(responseFromServer);
            // Clean up the streams and the response.
            reader.Close();
            response.Close();
        }

        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine(e.Result.Text);
            switch (e.Result.Text) {
                case "parar":
                case "parar música":
                    doOperations(ops.stop());
                    break;
                case "tocar":
                    doOperations(ops.play());
                    break;

            }
            /*
            Console.WriteLine("---------------------------------------------");

            Console.WriteLine("Recognition result summary:");
            Console.WriteLine(
              "  Recognized phrase: {0}\n" +
              "  Confidence score {1}\n" +
              "  Grammar used: {2}\n",
              e.Result.Text, e.Result.Confidence, e.Result.Grammar.Name);


            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("  Alternate phrase collection:");
            foreach (RecognizedPhrase phrase in e.Result.Alternates)
            {
                Console.WriteLine("    Phrase: " + phrase.Text);
                Console.WriteLine("    Confidence score: " + phrase.Confidence);
            }

            Console.WriteLine("---------------------------------------------");
            */
        }
    }
}
