using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace DIK_BOT_Console_Time
{

    class Message
    {
        public string chat_id;  //id чата
        public string text;     //текст сообщения

        public Message(string chat_id, string text)
        {
            this.chat_id = chat_id;
            this.text = text;
        }

        public void SetText(string text)
        {
            this.text = text;
        }

        public void SendMessage()
        {

            WebResponse wrs = GetWebResponse("SendMessage");

            Stream ReceiveStream = wrs.GetResponseStream();

            //Encoding encode = Encoding.GetEncoding("UTF-16");

            // Pipe the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(ReceiveStream, Encoding.UTF8);
            Console.WriteLine("\nResponse stream received");
            Char[] read = new Char[256];

            // Read 256 charcters at a time.    
            int count = readStream.Read(read, 0, 256);
            Console.WriteLine("HTML...\r\n");

            string TelegramRes = "";

            while (count > 0)
            {
                // Dump the 256 characters on a string and display the string onto the console.
                String str = new String(read, 0, count);
                //Console.Write(str);
                TelegramRes += str;
                count = readStream.Read(read, 0, 256);
            }

            TelegramRes = Regex.Replace(TelegramRes, @"\\u([0-9A-Fa-f]{4})", m => "" + (char)Convert.ToInt32(m.Groups[1].Value, 16));

            Console.WriteLine(TelegramRes);
            Console.WriteLine("");
            // Release the resources of stream object.
            readStream.Close();

            // Release the resources of response object.
            wrs.Close();

        }

        WebResponse GetWebResponse(string method)
        {
            string token = "";
            string url = "https://api.telegram.org/bot" + token + "/" + method;

            WebProxy webProxy = new WebProxy("", true);

            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Proxy = webProxy;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";

            string JSONMes = JsonConvert.SerializeObject(this);

            byte[] sentData = Encoding.GetEncoding("utf-8").GetBytes(JSONMes);
            webRequest.ContentLength = sentData.Length;

            Stream sendStream = webRequest.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();

            WebResponse webResponse = webRequest.GetResponse();

            return webResponse;
        }


    }
}
