using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VLC_Control
{
    class API
    {
        Uri directory; //or file
        public API() {
            //TODO:
            //Uri directory = default path... (Documens/Music or Documents/Videos...lets go see it!)
        }
        public API(Uri dir) {
            directory = dir;
        }
        // http://127.0.0.1:8080/requests/playlist.xml
        
        /*public int getFileId() { }
        public int getPlayListId() { }*/

        public WebRequest stop() {
            return WebRequest.Create("http://127.0.0.1:8080/requests/status.xml?command=pl_stop");
        }

        public WebRequest play() {
            return WebRequest.Create("http://127.0.0.1:8080/requests/status.xml?command=pl_play");
        }
        public WebRequest pause()
        {
            return WebRequest.Create("http://127.0.0.1:8080/requests/status.xml?command = pl_pause");
         }


        // request = WebRequest.Create("http://127.0.0.1:8080/requests/status.xml?command=pl_stop");
        /*
            Abrir ficheiro (música ou filme)
            Abrir pasta (playlist)
            Play
            Pause
            Stop
            Música/Filme Seguinte
            Música/Filme Anterior
            Aumentar e Diminuir volume; Mute
            Ver playlist
            Ecrã Completo
            Avançar
            Recuar
            Repetir playlist
            Aleatoriedade da playlist
            Aumentar e Diminuir velocidade
            Adicionar legendas (ficheiro!)
        *//*
        public WebRequest openFile(String name)
        {
            //search name in directory
            //return null in case it does not exist
            //This will play the one added (add to playlist and plays)
            //?command=in_play&input=<uri>
            //This will add the file to playlist and not play it automatically
            //?command = in_enqueue & input =< uri >
        }
        public WebRequest openPlaylist(String name) { }

        
        
        public WebRequest changeVolume(int op)// op = 0 - mute, -1 - lower, 1 - rise 
        {
            //?command=volume&val=<val>
            //Allowed values are of the form:
            //+< int >, -< int >, < int > or < int >%
        } 
        public WebRequest seePlaylist() { }
        public WebRequest fullScreen() {
            //?command=fullscreen
        }
        public WebRequest nextFile()
        {
            return WebRequest.Create("http://127.0.0.1:8080/requests/status.xml?command=pl_next");
        }


        public WebRequest previousFile()
        {
            return WebRequest.Create("http://127.0.0.1:8080/requests/status.xml?command=pl_previous");
        }
        public WebRequest forward() { }
        public WebRequest backward() { }
        public WebRequest repeatPlaylist() {
            //?command=pl_repeat
        }
        public WebRequest randomPlaylist() {
            //?command=pl_random
        }
        public WebRequest changeSpeed(bool op) { } //0- speed up, 1 - speed down
        public WebRequest addSubtitles()//Select the file respective to the movie (same name)
        {
            //?command = addsubtitle & val =< uri >
            //^uri = directory!!
        }
        public WebRequest addSubtitles(Uri subtitles)//Select the file choosen 
        {
            //?command = addsubtitle & val =< uri >
            //^uri = subtitles
        }*/
    }
}
