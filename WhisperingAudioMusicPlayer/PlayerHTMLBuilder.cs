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
                        sb.AppendLine(line);
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



        ///// <summary>
        ///// Builds all of the javascript used by the browser based player controller
        ///// </summary>
        ///// <returns>string of valid javascript</returns>
        //private static String GetPlayerScript()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("<script>");

        //    sb.AppendLine(GetSimpleScriptBlock("play", false, null, null, null));
        //    sb.AppendLine(GetSimpleScriptBlock("stop", false, null, null, null));
        //    sb.AppendLine(GetSimpleScriptBlock("next", false, null, null, null));
        //    sb.AppendLine(GetSimpleScriptBlock("previous", false, null, null, null));
        //    sb.AppendLine(GetScriptBlockWithParameter("changeVolume","direction", true,"volumeLabel", "Volume: ", "ChangeVolumeResult"));

        //    sb.AppendLine("</script>");

        //    return sb.ToString();
        //}

        ///// <summary>
        ///// Returns a javascript method that will call the restful service API hosted by the music player.
        ///// </summary>
        ///// <param name="command">String representing the route of the rest service to call</param>
        ///// <param name="updateLabel">Bool that specifies if the result of the rest call should update an HTML element in the web page.</param>
        ///// <param name="labelId">The id of the HTML element to update.</param>
        ///// <param name="labelText">Any text that should always be at the beginning of the HTML element that is updated.</param>
        ///// <param name="jsonVariableName">The variable name in the JSON that is returned from the REST call that will contain the text that will be placed in the above HTML element.</param>
        ///// <returns>String representing a javascript method</returns>
        //private static String GetSimpleScriptBlock(string command, bool updateLabel, string labelId, string labelText, string jsonVariableName)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("function " + command + "() {");
        //    sb.AppendLine(" var xhttp = new XMLHttpRequest();");

        //    if (updateLabel)
        //    {
        //        sb.AppendLine(" xhttp.onreadystatechange = function() {");
        //        sb.AppendLine("     if (xhttp.readyState == 4 && xhttp.status == 200) {");
        //        sb.AppendLine("         var Data = JSON.parse(xhttp.responseText);");
        //        sb.AppendLine("         document.getElementById(\"" + labelId + "\").innerHTML = \"" + labelText + "\" + Data." + jsonVariableName + ";");
        //        sb.AppendLine("     }");
        //        sb.AppendLine(" };");
        //    }
            
        //    sb.AppendLine(" xhttp.open(\"GET\", \"http://" + GetLocalIPAddress() + ":9090/wamp/" + command + "\", true);");
        //    sb.AppendLine(" xhttp.send();");
        //    sb.AppendLine("}");
        //    sb.AppendLine("");

        //    return sb.ToString();
        //}

        ///// <summary>
        ///// Returns a javascript method with a single parameter, that will call the restful 
        ///// service API hosted by the music player.
        ///// </summary>
        ///// <param name="command">String representing the route of the rest service to call</param>
        ///// <param name="parameterName">The name of the parameter that will be sent to the Javascript function</param>
        ///// <param name="updateLabel">Bool that specifies if the result of the rest call should update an HTML element in the web page.</param>
        ///// <param name="labelId">The id of the HTML element to update.</param>
        ///// <param name="labelText">Any text that should always be at the beginning of the HTML element that is updated.</param>
        ///// <param name="jsonVariableName">The variable name in the JSON that is returned from the REST call that will contain the text that will be placed in the above HTML element.</param>
        ///// <returns>String representing a JavaScript method</returns>
        //private static String GetScriptBlockWithParameter(string command, string parameterName, 
        //    bool updateLabel, string labelId, string labelText, string jsonVariableName)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("function " + command + "(" + parameterName + ") {");
        //    sb.AppendLine(" var url = \"http://" + GetLocalIPAddress() + ":9090/wamp/" + command + "/\" + " + parameterName);
        //    sb.AppendLine(" var xhttp = new XMLHttpRequest();");

        //    if (updateLabel)
        //     {
        //        sb.AppendLine(" xhttp.onreadystatechange = function() {");
        //        sb.AppendLine("     if (xhttp.readyState == 4 && xhttp.status == 200) {");
        //        sb.AppendLine("         var Data = JSON.parse(xhttp.responseText);");
        //        sb.AppendLine("         document.getElementById(\"" + labelId + "\").innerHTML = \"" + labelText + "\" + Data." + jsonVariableName + ";");
        //        sb.AppendLine("     }");
        //        sb.AppendLine(" };");
        //    }

        //    sb.AppendLine(" xhttp.open(\"GET\", url, true);");
        //    sb.AppendLine(" xhttp.send();");
        //    sb.AppendLine("}");
        //    sb.AppendLine("");

        //    return sb.ToString();
        //}
    }
}
