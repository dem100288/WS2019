using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Util
{
    public static class ConnectionToServer
    {
        public static void SendDataToServer(string data)
        {
            Task.Run(() =>
            {

            });
        }

        public static void SendJson(string json, string thing, string service)
        {
            try
            {
                WebRequest request = WebRequest.Create(string.Format("{0}Things/{1}/Services/{2}?postParameter=json", Settings.Server, thing, service));
                request.Method = "POST";
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);
                request.ContentType = "application/json";
                request.Headers.Add("AppKey", Settings.AppKey);
                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                request.GetResponseAsync();
            }
            catch (Exception e)
            {

            }
        }


        //public static bool TestConnectToServer()
        //{
        //    bool res = false;
        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
                    
        //            var t1 = client.GetAsync(Settings.Server);
        //            t1.Wait();
        //            HttpResponseMessage response = t1.Result;
        //            response.EnsureSuccessStatusCode();
        //            res = true;
        //        }
        //        catch (HttpRequestException e)
        //        {
        //            res = false;
        //        }
        //    }
        //    return res;
        //}
    }
}
