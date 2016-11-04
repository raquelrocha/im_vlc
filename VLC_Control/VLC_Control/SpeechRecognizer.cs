using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VLC_Control
    /* TODO
        Acção "Quero ouvir todas as músicas" -> voltar à playlist original    
*/
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
                return;
            }
            //Gramática dos ficheiros (nomes)
            Grammar names = createFilesGrammar();
            if (names != null)
            {
                names.Enabled = true;
                sre.LoadGrammar(names);
                Console.WriteLine("Gramática de nomes carregada...");
            }
            else
            {
                Console.WriteLine("Não foi possível carregar a gramática de nomes.");
                tts.Speak("Não foi possível carregar a gramática de nomes.");
            }

            //Gramática das categorias (Rock, Pop, Comédia, etc.)
            Grammar type = createTypeGrammar();
            if (type != null)
            {
                type.Enabled = true;
                sre.LoadGrammar(type);
                Console.WriteLine("Gramática das categorias carregada...");
            }
            else
            {
                Console.WriteLine("Não foi possível carregar a gramática das categorias.");
                tts.Speak("Não foi possível carregar a gramática das categorias.");
            }

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
                if (e.Result.Semantics.ContainsKey("reproduzir") && e.Result.Confidence > 0.4)
                {
                    KeyValuePair<string, SemanticValue>[] sem = e.Result.Semantics.ToArray();
                    string query = e.Result.Text;
                    string remove = sem[0].Value.Value.ToString();
                    query = query.Replace(remove, "").Trim();
                    Console.WriteLine("A procurar por: " + query);
                    tts.Speak("A procurar por: " + query);
                    bool notfound = request.getFile(query).Equals(""); //Retorna "" se não encontrar
                    if (notfound)
                        tts.Speak("Não encontro a música " + query);
                    else
                    {
                        Console.WriteLine("Música " + query + "encontrada.");
                        request.play(query);
                    }
                }
                else if (e.Result.Semantics.ContainsKey("tipos") && e.Result.Confidence > 0.4)
                {
                    KeyValuePair<string, SemanticValue>[] sem = e.Result.Semantics.ToArray();
                    string query = e.Result.Text;
                    string remove = sem[0].Value.Value.ToString();
                    query = query.Replace(remove, "").Trim();
                    Console.WriteLine("A procurar por: " + query);
                    tts.Speak("A procurar por: " + query);
                    bool found = request.getTipos().Contains(query);
                    if (!found)
                        tts.Speak("Não existem músicas de " + query);
                    else
                    {
                        Console.WriteLine("Existem músicas de " + query);
                        request.stop();
                        request.playlistByType(query);
                        request.nextFile();
                    }
                        
                }
                else {
                    foreach (KeyValuePair<string, SemanticValue> r in e.Result.Semantics)
                    {
                        Console.WriteLine(r.Key + ":" + r.Value.Value.ToString());

                        switch (r.Value.Value.ToString())
                        {
                            case "reproduzir":
                                request.play();
                                break;
                            case "parar":
                                request.stop();
                                break;
                            case "seguinte":
                                request.nextFile();
                                break;
                            case "anterior":
                                request.previousFile();
                                break;
                            case "alto":
                                request.changeVolume(1);
                                break;
                            case "maisalto":
                                request.changeVolume(2);
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
                            case "notfullscreen":
                                request.fullScreen();
                                break;
                            case "voz feminina":
                                tts.changeGender(Microsoft.Speech.Synthesis.VoiceGender.Female);
                                tts.Speak("Já mudei de voz");
                                break;
                            case "voz masculina":
                                tts.changeGender(Microsoft.Speech.Synthesis.VoiceGender.Male);
                                tts.Speak("Já mudei de voz");
                                break;
                            case "Bleed it out":
                                request.play("Bleed it out");
                                break;
                            case "Rock":
                                //request.getMusicByType("Rock");
                                break;
                            default:
                                break;

                        }
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

        private Grammar createFilesGrammar() {
            /*
            TODO:
            Separar os nomes dos ficheiros para 
            Filmes [nomes]  e Musicas[nomes]
            Depois nesta função criar frases para filmes a frases para musicas
            Feito agora: Apenas para musicas!
            */
            string[] frases = { "Quero ouvir a música" };
            Choices frase = new Choices(frases);
            GrammarBuilder elementoFrase = new GrammarBuilder(frase, 1, 1);
            SemanticResultKey acaoSRK = new SemanticResultKey("reproduzir", elementoFrase);

            HashSet<string> names = request.getPlaylistNames();
            if (names == null || names.Count == 0)
                return null;
            Choices namesChoices = new Choices(names.ToArray());
            GrammarBuilder namesGB = new GrammarBuilder(namesChoices, 1, 1);

            GrammarBuilder total = acaoSRK.ToGrammarBuilder();
            total.Append(namesGB);

            total.Culture = new System.Globalization.CultureInfo("pt-PT");
            Grammar names_gr = new Grammar(total);
            names_gr.Name = "Files Grammar";
            
            return names_gr;
        }

        private Grammar createTypeGrammar() {

            string[] frases = { "Quero ouvir músicas de", "Quero ouvir " };
            Choices frase = new Choices(frases);
            GrammarBuilder elementoFrase = new GrammarBuilder(frase, 1, 1);
            SemanticResultKey acaoSRK = new SemanticResultKey("tipos", elementoFrase);

            HashSet<string> tipos = request.getTipos();
            if (tipos == null || tipos.Count == 0)
                return null;
            Choices tiposChoices = new Choices(tipos.ToArray());
            GrammarBuilder tiposGB = new GrammarBuilder(tiposChoices, 1, 1);

            GrammarBuilder total = acaoSRK.ToGrammarBuilder();
            total.Append(tiposGB);

            total.Culture = new System.Globalization.CultureInfo("pt-PT");
            Grammar tipos_gr = new Grammar(total);
            tipos_gr.Name = "Files Grammar";

            
            return tipos_gr;
        }
    }
}


