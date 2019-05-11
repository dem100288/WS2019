using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Util
{
    public enum MessageStatus { Info = 0, Warning = 2, Error = 3};

    public static class Tools
    {
        public delegate void OnChangeScaleHandle();
        public static event OnChangeScaleHandle OnChangeScale;
        public delegate void OnNewMessageHandle(MessageStatus status, string message);
        public static event OnNewMessageHandle OnNewMessage;

        public static Random rand = new Random();
        private static double scaleX { set; get; } = 1;
        private static double scaleY { set; get; } = 1;
        public static double ScaleX { set { scaleX = value;  OnChangeScale?.Invoke(); } get { return scaleX; } }
        public static double ScaleY { set { scaleY = value; OnChangeScale?.Invoke(); } get { return scaleY; } }

        public static ScaleTransform ScaleObject;

        public static string DescriptinoMessageStatus(MessageStatus status)
        {
            string mes = "";
            switch(status)
            {
                case MessageStatus.Error: { mes = Localization.GetText("Text45"); break; }
                case MessageStatus.Info: { mes = Localization.GetText("Text46"); break; }
                case MessageStatus.Warning: { mes = Localization.GetText("Text47"); break; }
            }
            return mes;
        }

        public static void Message(MessageStatus status, string message, bool sendtoserver = false)
        {
            if (sendtoserver)
                ConnectionToServer.SendDataToServer("send messgae");
            OnNewMessage?.Invoke(status, message);
        }

        static Tools()
        {
            ScaleObject = new ScaleTransform(1, 1);
        }
    }
}
