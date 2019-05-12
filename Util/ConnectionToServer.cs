using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

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

        public static bool TestConnectToServer()
        {
            bool res = false;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var t1 = client.GetAsync(Settings.Server);
                    t1.Wait();
                    HttpResponseMessage response = t1.Result;
                    response.EnsureSuccessStatusCode();
                    res = true;
                }
                catch (HttpRequestException e)
                {
                    res = false;
                }
            }
            return res;
        }
    }
}
