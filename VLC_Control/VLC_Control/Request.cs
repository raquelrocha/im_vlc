using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VLC_Control
{
    class Request
    {
        String address = "127.0.0.1:8080";
        HttpWebRequest httpreq;
        Dictionary<string, Dictionary<string, string>> playlist;
        //{tipo: {nome:uri}}

        /*
        TODO:
            var to store the Playlist!!!!!
        */

        public Request(string address = null)
        {
            if (address != null && address != "")
            {
                this.address = address;
            }

            this.playlist = new Dictionary<string, Dictionary<string, string>>();
            //createPlayLists();
        }

        string credential = Convert.ToBase64String(Encoding.Default.GetBytes(":123456"));
        public void stop()
        {
            try
            {
                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=pl_stop");
                httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                httpreq.GetResponse();
            }
            catch (HttpListenerException)
            {
                Console.WriteLine("Erro no pedido. Volte a tentar");
            }
        }

        public void play()
        {
            try
            {
                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=pl_play");
                httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                httpreq.GetResponse();
            }
            catch (HttpListenerException)
            {
                Console.WriteLine("Erro no pedido. Volte a tentar");
            }
        }

        public void pause()
        {
            try
            {
                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=pl_pause");
                httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                httpreq.GetResponse();
            }
            catch (HttpListenerException)
            {
                Console.WriteLine("Erro no pedido. Volte a tentar");
            }
        }


        public void changeVolume(int op)// op = 0 - mute, -1 - lower, 1 - rise 
        {
            string varVol = "0";
            //para ter o simbolo % no url temos q meter o código %25!!!
            switch (op)
            {
                case 0:
                    varVol = "0";
                    break;
                case 1:
                    varVol = "+25";
                    break;
                case -1:
                    varVol = "-25";
                    break;
                default:
                    varVol = "400";
                    break;
            }

            try
            {
                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=volume&val=" + varVol); //mute
                httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                httpreq.GetResponse();
            }
            catch (HttpListenerException)
            {
                Console.WriteLine("Erro no pedido. Volte a tentar");
            }


        }

        public void fullScreen() //toggle fullscreen (caso nao esteja fullscreen, fica fullscreen, caso esteja, então desativa.
        {
            try
            {
                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=fullscreen");
                httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                httpreq.GetResponse();
            }
            catch (HttpListenerException)
            {
                Console.WriteLine("Erro no pedido. Volte a tentar");
            }
        }

        public void nextFile()//a bit bugged (problem of VLC and not ours!!!)
        {
            try
            {
                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=pl_next");
                httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                httpreq.GetResponse();
            }
            catch (HttpListenerException)
            {
                Console.WriteLine("Erro no pedido. Volte a tentar");
            }
        }


        public void previousFile()//Not working to 100% -> vai para o inicio do ficheiro.. (problem of VLC and not ours!!!)
        {
            try
            {
                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=pl_previous");

                httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                httpreq.GetResponse();
            }
            catch (HttpListenerException)
            {
                Console.WriteLine("Erro no pedido. Volte a tentar");
            }
        }

        /* TODO:
        Verificar quais são as "tarefas que faltam implementar 
        - incluíndo as que estão em comentário pq não estão feitas!!! */


        /*public WebRequest openFile(String name)
        {
            //search name in directory
            //return null in case it does not exist
            //This will play the one added (add to playlist and plays)
            //?command=in_play&input=<uri>
            //This will add the file to playlist and not play it automatically
            //?command = in_enqueue & input =< uri >
        }*/

        //public WebRequest openPlaylist(String name) { }

        //public WebRequest seePlaylist() { }


        //public WebRequest forward() { }
        //public WebRequest backward() { }
        /*public WebRequest repeatPlaylist() {
            //?command=pl_repeat
        }*/

        /*public WebRequest randomPlaylist() {
            //?command=pl_random
        }*/

        //public WebRequest changeSpeed(bool op) { } //0- speed up, 1 - speed down

        /*public WebRequest addSubtitles()//Select the file respective to the movie (same name)
        {
            //?command = addsubtitle & val =< uri >
            //^uri = directory!!
        }*/

        /*public WebRequest addSubtitles(Uri subtitles)//Select the file choosen 
        {
            //?command = addsubtitle & val =< uri >
            //^uri = subtitles
        }*/

        /* TODO:
            getNames of files?
            função que faça retrieve da playlist atual?
            função que procure a musica/filme/playlist em questão?
            getPlaylists?
            getCurrentMusic?
            select playlist (nsei se ja está em cima)

        */


        /*public string getFile(string fname) {
            foreach (KeyValuePair<string, Dictionary<string, string>> dic in playlist) {
                Dictionary<string, string> aux = dic.Value;
                if (aux.ContainsKey(fname)) {
                    return aux[fname];
                }
            }
            return "";
        }*/

        
        /*private void createPlayLists()
        {
            string dir = System.IO.Directory.GetCurrentDirectory() + "\\musicas";

            if (System.IO.Directory.Exists(dir))
                foreach (string s in System.IO.Directory.GetFiles(dir))
                {
                    Console.WriteLine("----------");
                    System.IO.FileInfo fi = null;
                    try
                    {
                        fi = new System.IO.FileInfo(s);
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    TagLib.File f = TagLib.File.Create(fi.FullName);
                    
                    foreach (string g in f.Tag.Genres) {
                        Console.WriteLine(g);
                        if (this.playlist.ContainsKey(g))
                        {
                                this.playlist[g][f.Tag.Title] = fi.FullName;
                        }
                        else
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic.Add(f.Tag.Title, fi.FullName);
                            this.playlist.Add(g, dic);
                        }    
                    }
                    foreach (KeyValuePair<string, Dictionary<string,string>> entry in this.playlist)
                    {
                        Console.Write(entry.Key + ": \n");
                        foreach (KeyValuePair<string, string> dic in entry.Value)
                            Console.Write(dic.Key + ":\n" + dic.Value + "\n");
                    }


                    Console.WriteLine("----------");
                }

            Console.WriteLine("\n");
        }
        string playlistActual;
        public void playPlaylist(string playlist) {

        }*/
    }
}
