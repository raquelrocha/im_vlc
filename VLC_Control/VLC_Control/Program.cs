using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VLC_Control
{
    class Program
    {
        static void Main()
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create("http://127.0.0.1:8080/requests/status.xml?command=pl_stop");
            var credential = Convert.ToBase64String(Encoding.Default.GetBytes(":123456"));
            request.Headers[HttpRequestHeader.Authorization] = "Basic " + credential;
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);
            // Clean up the streams and the response.
            reader.Close();
            response.Close();

            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
        
    }
}
