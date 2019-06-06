using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Util
{
    public static class ConnectionToServer
    {
        public static bool ConnectToServer { get; private set; } = true;

        public static void SendDataToServer(string data)
        {
            Task.Run(() =>
            {

            });
        }

        public static void SendMessageToServer(MessageStatus status, string data)
        {
            if (!ConnectToServer) return;
            int t = 0;
            switch (status)
            {
                case MessageStatus.Info: { t = 1; break; }
                case MessageStatus.Warning: { t = 2; break; }
                case MessageStatus.Error: { t = 3; break; }
                default: { return; }
            }
            JObject json = new JObject();
            json["type"] = t;
            json["message"] = data;
            SendJson(json.ToString(), "Simulation", "Message");
        }

        //public static void SendConteinerCrowded(int id, double fill)
        //{
        //    try
        //    {
        //        if (!ConnectToServer) return;
        //        WebRequest request = WebRequest.Create(string.Format("{0}Things/{1}/Services/{2}?id={3}&fill={4}", Settings.Server, "Simulation", "ConteinerCrowded", id, fill.ToString().Replace(",",".")));
        //        request.Method = "POST";
        //        request.ContentType = "application/json";
        //        request.Headers.Add("AppKey", Settings.AppKey);
        //        request.GetResponse();
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}

        public static void SendSimulationStatus(bool status)
        {
            try
            {
                if (!ConnectToServer) return;
                JObject json = new JObject();
                json["status"] = status;
                SendJson(json.ToString(), "Simulation", "SetStatus");
            }
            catch (Exception e)
            {

            }
        }

        public static void SendAddCar(string car)
        {
            try
            {
                if (!ConnectToServer) return;
                SendJson(car, "Simulation", "AddedCar");
            }
            catch (Exception e)
            {

            }
        }

        public static string GetCarAction(int id)
        {
            try
            {
                if (!ConnectToServer) return null;
                JObject o = new JObject();
                o["id"] = id;
                return SendJson(o.ToString(), "Simulation", "GetAction");
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static void SendChangeCarStatus(string car)
        {
            try
            {
                if (!ConnectToServer) return;
                SendJson(car, "Simulation", "ChangeCarStatus");
            }
            catch (Exception e)
            {

            }
        }

        public static void SendChangeCoins(double coins)
        {
            try
            {
                //if (!ConnectToServer) return;
                //WebRequest request = WebRequest.Create(string.Format("{0}Things/{1}/Services/{2}?coins={3}", Settings.Server, "Simulation", "ChangeCoins", coins.ToString().Replace(",", ".")));
                //request.Method = "POST";
                //request.ContentType = "application/json";
                //request.Timeout = 100;
                //request.Headers.Add("AppKey", Settings.AppKey);
                ////var res = await request.GetResponseAsync();
                //var r = (HttpWebResponse)request.GetResponse();
                JObject json = new JObject();
                json["coins"] = coins;
                SendJson(json.ToString(),"Simulation","ChangeCoins");

                //using (var webClient = new WebClient())
                //{
                //    NameValueCollection pairs = new NameValueCollection();
                //    webClient.Headers.Add("AppKey", Settings.AppKey);
                //    //webClient.Headers.Add("Content-Type", "application/json");
                //    webClient.Headers.Set(HttpRequestHeader.ContentType, "application/json");

                //    webClient.UploadValues(string.Format("{0}Things/{1}/Services/{2}?coins={3}", Settings.Server, "Simulation", "ChangeCoins", coins.ToString().Replace(",", ".")), "POST", pairs);
                //}
            }
            catch (Exception e)
            {

            }
        }

        public static string SendJson(string json, string thing, string service)
        {
            try
            {
                if (!ConnectToServer) return null;
                HttpWebRequest request = WebRequest.CreateHttp(string.Format("{0}Things/{1}/Services/{2}?postParameter=json", Settings.Server, thing, service));
                request.Method = "POST";
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Headers.Add("AppKey", Settings.AppKey);
                request.ContentLength = byteArray.Length;
                using (Stream requestdataStream = request.GetRequestStream())
                {
                    requestdataStream.Write(byteArray, 0, byteArray.Length);
                }

                var response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                response.Close();
                //Util.Tools.Message(MessageStatus.Info, Localization.GetText("Text63"));
                return responseFromServer;

            }
            catch (Exception e)
            {
                return null;
            }
        }


        public static bool TestConnectToServer()
        {
            //try
            //{
            //    WebRequest request = WebRequest.Create(Settings.Server);
            //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //    Console.WriteLine(response.StatusDescription);
            //    if (response.StatusCode == HttpStatusCode.OK) ConnectToServer = true;
            //}
            //catch (Exception ex)
            //{
            //    ConnectToServer = false;
            //}
            //if (ConnectToServer)
            //    Tools.Message(MessageStatus.Info, Util.Localization.GetText("Text60"));
            //else
            //    Tools.Message(MessageStatus.Error, Util.Localization.GetText("Text61"));
            //return ConnectToServer;
            return true;
        }
    }
}
