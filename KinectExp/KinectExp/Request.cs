using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace VLC_KinectControl
{
    class Request
    {
        String address = "127.0.0.1:8080";
        HttpWebRequest httpreq;
        Dictionary<string,string> playlistAtual;
        Dictionary<string, Dictionary<string, string>> playlist;
        int quantidade = 0;
        HashSet<string> tipos; //{tipo: {nome:uri}}
        string tipo = "";

        public Request(string address = null)
        {
            if (address != null && address != "")
            {
                this.address = address;
            }

            tipos = new HashSet<string>();
            this.playlist = new Dictionary<string, Dictionary<string, string>>();
            this.playlistAtual = new Dictionary<string, string>();

            createPlayLists();
            updatePlayListAtual();
            
        }
        public void updatePlayListAtual() {
            playlistAtual = new Dictionary<string, string>();
            foreach (Dictionary<string, string> dic in playlist.Values) {
                foreach (KeyValuePair<string, string> musicas in dic)
                {
                    if (!playlistAtual.ContainsKey(musicas.Key))
                        playlistAtual.Add(musicas.Key, musicas.Value);
                    else
                        playlistAtual[musicas.Key] = musicas.Value;
                    
                }

            }
            updateListInVLC();
        }

        private void updateListInVLC() {
            try
            {

                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=pl_empty");
                httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                httpreq.GetResponse();
            }
            catch (HttpListenerException)
            {
               // Console.WriteLine("Erro no pedido. Volte a tentar");
            }
            foreach (KeyValuePair<string,string> musicas in playlistAtual)
            try
            {
                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=in_enqueue&input=" + musicas.Value);
                httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                httpreq.GetResponse();
            }
            catch (HttpListenerException)
            {
                //Console.WriteLine("Erro no pedido. Volte a tentar");
            }
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

        
        public string getFile(string fname)
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> entry in this.playlist)
            {
                foreach (KeyValuePair<string, string> dic in entry.Value)
                {
                    if (dic.Key.Equals(fname, StringComparison.InvariantCultureIgnoreCase))
                        return dic.Value;
                }
            }
            return "";
        }

        public string musicaAtual() {
            string name = "Nenhuma";
            httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml");
            httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
            WebResponse response = httpreq.GetResponse();
            Stream str = response.GetResponseStream();
            XmlDocument doc = new XmlDocument();
            doc.Load(str);
            foreach (XmlNode x in doc.ChildNodes) {
                string xml = x.InnerXml;
                if (xml.Contains("<state>playing</state>")) {
                    int zero = xml.IndexOf("<info name=\"title\">") + 20;
                    name = xml.Substring(zero-1).Split('<')[0];
                }

            }
            Console.WriteLine(name);
            return name;
        }

        private void createPlayLists()
        {
            string dir = System.IO.Directory.GetCurrentDirectory() + "\\musicas";
            
            if (System.IO.Directory.Exists(dir)) {
                foreach (string s in System.IO.Directory.GetFiles(dir))
                {

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

                    foreach (string g in f.Tag.Genres)
                    {
                        if (this.playlist.ContainsKey(g) || (g.Equals("Português") && this.playlist.ContainsKey("Portuguesa")))
                        {
                            quantidade += 1;
                            if (g.Equals("Português"))
                                this.playlist["Portuguesa"][f.Tag.Title] = fi.FullName;
                            else
                                this.playlist[g][f.Tag.Title] = fi.FullName;
                        }
                        else
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic.Add(f.Tag.Title, fi.FullName);

                            if (g.Equals("Português"))
                            {
                                this.playlist.Add("Portuguesa", dic);
                                tipos.Add("Portuguesa");
                            }
                            else {
                                this.playlist.Add(g, dic);
                                tipos.Add(g);
                            }
                            quantidade += 1;

                        }
                    }
                }
                    Console.WriteLine("Músicas adicionadas à playlist...");
                    /*foreach (KeyValuePair<string, Dictionary<string, string>> entry in this.playlist)
                    {
                        foreach (KeyValuePair<string, string> dic in entry.Value)
                            Console.WriteLine(dic.Key + ":\n" + dic.Value + "\n");
                    }*/
                }
        }

        public void playlistByType(string tipo) {
            if (tipo.Equals("TODAS")) { 
                updatePlayListAtual();
            this.tipo = "TODAS";
            }
            else {
                playlistAtual = new Dictionary<string, string>();
                playlistAtual = playlist[tipo];

                this.tipo = tipo;
            }
            
            updateListInVLC();
        }

        public HashSet<string> getPlaylistNames() {
            HashSet<string> aux = new HashSet<string>();
            foreach (KeyValuePair<string, string> music in playlistAtual)
                aux.Add(music.Key);
            return aux;
        }

        public HashSet<string> getTipos() {
            return tipos;
        }

        public int getQuantidade() {
            return quantidade;
        }

        public string tipoAtual() {

            return tipo == "" ? " a inicial " : tipo;
        }

        /* Pedidos HTTP */
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

        public void play(string name = null, string type = null) 
        {
            string mrl = getFile(name);
            try
            {
                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=in_play&input=" + mrl);
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

                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml");
                httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                WebResponse response = httpreq.GetResponse();
                Stream str = response.GetResponseStream();
                XmlDocument doc = new XmlDocument();
                doc.Load(str);
                foreach (XmlNode x in doc.ChildNodes)
                    if (x.InnerXml.Contains("<state>playing</state>"))
                    {
                        httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=pl_pause");
                        httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                        httpreq.GetResponse();
                    }


            }
            catch (HttpListenerException)
            {
                Console.WriteLine("Erro no pedido. Volte a tentar");
            }
        }


        public void changeVolume(int op)//  -2 lower more, -1 - lower, op = 0 - mute,1 - rise, 2 - rise more
        {
            string varVol = "0";
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
                case 2:
                    varVol = "+75";
                    break;
                case -2:
                    varVol = "-75";
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

        public void fullScreen()
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

        public void nextFile()
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


        public void previousFile()
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

        public void repeatPlaylist()
        {
            try
            {
                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=pl_repeat");
                httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                httpreq.GetResponse();
            }
            catch (HttpListenerException)
            {
                Console.WriteLine("Erro no pedido. Volte a tentar");
            }
        }

        public void randomPlaylist()
        {
            try
            {
                httpreq = (HttpWebRequest)WebRequest.Create("http://" + this.address + "/requests/status.xml?command=pl_random");
                httpreq.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
                httpreq.GetResponse();
            }
            catch (HttpListenerException)
            {
                Console.WriteLine("Erro no pedido. Volte a tentar");
            }
        }

    }
}
