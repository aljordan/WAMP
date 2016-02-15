using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.IO;

namespace WhisperingAudioMusicPlayer
{
    /// <summary>
    /// Builds all of the html and javascript that becomes the web page that controls the player from a browser
    /// </summary>
    class PlayerHTMLBuilder
    {

        /// <summary>
        /// Get the IP Address of the computer that the player is running on
        /// </summary>
        /// <returns>
        /// String containing an IP Adress in the form of 192.168.0.1 
        /// </returns>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        public static String GetPlayerHtml()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");


            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "HtmlBuilder\\PlayerHeaderTemplate.html"))
            {
                string line;
                using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "HtmlBuilder\\PlayerHeaderTemplate.html"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                    }
                }
            }

            sb.AppendLine("<body>");

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "HtmlBuilder\\PlayerBodyTemplate.html"))
            {
                string line;
                using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "HtmlBuilder\\PlayerBodyTemplate.html"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        //sb.AppendLine(line);
                        sb.AppendLine(line.Replace("localhost", GetLocalIPAddress()));
                    }
                }
            }

            sb.AppendLine("<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/2.2.0/jquery.min.js\"></script>");
            
            sb.AppendLine("<script>");
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "HtmlBuilder\\PlayerScriptTemplate.js"))
            {
                string line;
                using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "HtmlBuilder\\PlayerScriptTemplate.js"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.AppendLine(line.Replace("localhost", GetLocalIPAddress()));
                    }
                }
            }

            sb.AppendLine("</script>");

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return new HtmlString(sb.ToString()).ToString();
        }
    }
}
