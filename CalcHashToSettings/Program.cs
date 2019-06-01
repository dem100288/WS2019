using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace CalcHashToSettings
{
    class Program
    {
        static string GetHash(XDocument x)
        {
            return BitConverter.ToString(MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(x.ToString()))).Replace("-","");
        }

        static bool CheckHash(XDocument x)
        {
            string chash = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(x.ToString()))).Replace("-", "");
            int code = int.Parse(x.Root.Attribute("code")?.Value ?? "0");
            var xhash = x.Root.Attribute("hash").Value;
            return chash.Substring(0, code) == xhash.Substring(0, code);
        }

        static void Main(string[] args)
        {
            var x = XDocument.Load("Settings.xml");
            //var x = XDocument.Load("test.xml");
            var check = CheckHash(x);
            var hsave = x.Root.Attribute("hash").Value;
            int code = int.Parse(x.Root.Attribute("code")?.Value??"0");
            var hcalc = GetHash(x);
            while (hsave.Substring(0, code) != hcalc.Substring(0, code))
            {
                x.Root.SetAttributeValue("hash", hcalc);
                hsave = hcalc;
                hcalc = GetHash(x);
            }
            x.Save("test.xml");
            Console.WriteLine("Find hash: {0}", hsave);
            Console.ReadKey();
        }
    }
}
