using System;
using System.Collections.Generic;
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

        }

        public void stop()
        {
            try
            {
                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=pl_stop");
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
            }
            catch (HttpListenerException)
            {
                Console.WriteLine("Erro no pedido. Volte a tentar");
            }
        }


        public void changeVolume(int op)// op = 0 - mute, -1 - lower, 1 - rise 
        {
            int varVol = 0;
            //para ter o simbolo % no url temos q meter o código %25!!!
            switch (op)
            {
                case 0:
                    varVol = 0;
                    break;
                case 1:
                    varVol = 25;
                    break;
                case -1:
                    varVol = -25;
                    break;
                default:
                    varVol = 400;
                    break;
            }

            try
            {
                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=volume&val=" + varVol); //mute
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
    }
}
