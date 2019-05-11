using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Globalization;

namespace Util
{
    public static class Localization
    {
        private static XElement Dictionary;
        public static CultureInfo DefaultLang;
        private static char[] EscapeChar = { ' ','\t','\r','\n'};

        public static void SetDitionary(XElement dictElement)
        {
            try
            {
                Dictionary = dictElement;
                DefaultLang = CultureInfo.GetCultureInfo(dictElement.Attribute("default").Value);
            }
            catch(Exception ex)
            {
                Dictionary = null;
            }
        }

        public static string GetText(string tag)
        {
            return GetText(tag, CultureInfo.CurrentCulture);
        }
        public static string GetText(string tag, CultureInfo cult)
        {
            var el = Dictionary?.XPathSelectElement("//text[@name='"+ tag + "']");
            if (el != null)
            {
                foreach (var l in el.Elements("lang"))
                    if (l?.Attribute("name")?.Value == cult.Name)
                    {
                        return l.Value.Trim(EscapeChar);
                    }
                if (cult != DefaultLang) return GetText(tag, DefaultLang);
                else return "";
            }
            else
            {
                return "";
            }
        }
    }
}
