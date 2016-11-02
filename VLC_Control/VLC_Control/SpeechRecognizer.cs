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
        private SpeechRecognitionEngine sre; //SpeechRecognitionEngine
        private Request request; //VLC Request (HTTP)
        private Grammar g; //Grammar
        private string gender_tts = "";
        private Synthesizer tts; //Text to Speech Synthesizer

        public SpeechRecognizer(string grammar, Request request)
        {
            Console.WriteLine("Espere enquanto a aplicação multimodal carrega...");
            this.request = request;
            tts = new Synthesizer(request);
            gender_tts = tts.getGender();
            sre = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pt-PT"));
            sre.SetInputToDefaultAudioDevice();

            Console.WriteLine(grammar);


            if (System.IO.File.Exists(grammar))
            {
                Console.WriteLine("aqui");
                g = new Grammar(grammar);
                g.Enabled = true;
                Console.WriteLine("Gramática geral carregada...");
                sre.LoadGrammar(g);
            }
            else
            {
                Console.WriteLine("Não foi possível carregar a gramática geral.");
                tts.Speak("Não foi possível carregar a gramática geral.");
            }

            //Load of the dinamic grammar! (playlist/music names)
            /*Grammar playlist_gr = createPlGrammar();
            Grammar files_names_gr = createFnamesGrammar();

            if (playlist_gr == null)
            {
                tts.Speak("Não foi possível carregar a gramática das listas");
                Console.WriteLine("Não foi possível carregar a gramática das listas");
            }
            else
            {
                playlist_gr.Enabled = true;
                sre.LoadGrammar(playlist_gr);
                Console.WriteLine("Gramática das listas carregada");
            }

            if (files_names_gr == null)
            {
                tts.Speak("Não foi possível carregar a gramática dos nomes");
                Console.WriteLine("Não foi possível carregar a gramática dos nomes");
            }
            else
            {
                files_names_gr.Enabled = true;
                sre.LoadGrammar(files_names_gr);
                Console.WriteLine("Gramática dos nomes carregada");
            }
            */

            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
            sre.RecognizeAsync(RecognizeMode.Multiple);

            if (gender_tts == "female")
            {
                tts.Speak("Pronta a receber pedidos");
                Console.WriteLine("Pronta a receber ordens");
            }
            else
            {
                tts.Speak("Pronto a receber pedidos");
                Console.WriteLine("Pronto a receber ordens");
            }
            
            // Keep the console window open.
            /*while (true)
            {
                Console.ReadLine();
            }*/
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
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("Texto reconhecido: " + e.Result.Text);
            Console.WriteLine("Confiança: " + e.Result.Confidence);
            Console.WriteLine("---------------Semantics----------------");
            Console.WriteLine(e.Result.Semantics);
            Console.WriteLine(e.Result.Semantics.Value);
        }
    }
}
