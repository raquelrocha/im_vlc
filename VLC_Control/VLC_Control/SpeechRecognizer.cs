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
            while (true)
            {
                Console.ReadLine();
            }
        }


        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("Texto reconhecido: " + e.Result.Text);
            Console.WriteLine("Confiança: " + e.Result.Confidence);
            if (e.Result.Confidence > 0.65)
            {
                foreach (KeyValuePair<string, SemanticValue> r in e.Result.Semantics)
                {
                    Console.WriteLine(r.Key + ":" + r.Value.Value.ToString());

                    switch (r.Value.Value.ToString())
                    {
                        case "reproduzir":
                            Console.WriteLine("XXXx");
                            request.play();
                            break;
                        case "parar":
                            request.stop();
                            break;
                        case "próxima":
                            request.nextFile();
                            break;
                        case "anterior":
                            request.previousFile();
                            break;
                        case "alto":
                            request.changeVolume(1);
                            break;
                        case "baixo":
                            request.changeVolume(-1);
                            break;
                        case "mute":
                            request.changeVolume(0);
                            break;
                        case "pausar":
                            request.pause();
                            break;
                        case "fullscreen":
                            request.fullScreen();
                            break;
                        case "voz feminina":
                            tts.changeGender(Microsoft.Speech.Synthesis.VoiceGender.Female);
                            break;
                        case "voz masculina":
                            tts.changeGender(Microsoft.Speech.Synthesis.VoiceGender.Male);
                            break;
                        default:
                            break;

                    }
                }

            }
            else if (e.Result.Confidence < 0.25)
                tts.Speak("Repita que eu não entendi.");
            else if (e.Result.Confidence < 0.50)
                tts.Speak("Importa-se de repetir?");
            else
                tts.Speak("Não entendi, repita se faz favor.");
        }
    }
}


