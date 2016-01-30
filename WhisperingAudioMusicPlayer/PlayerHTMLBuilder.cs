using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace WhisperingAudioMusicPlayer
{
    class PlayerHTMLBuilder
    {

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
            sb.AppendLine("    <head>");
            sb.AppendLine("        <title>Whispering Audio Music Player</title>");
            sb.AppendLine("        <style>");
            sb.AppendLine("            table.center {");
            sb.AppendLine("                margin-left:auto; ");
            sb.AppendLine("                margin-right:auto;");
            sb.AppendLine("            }");
            sb.AppendLine("            div.outset {");
            sb.AppendLine("                display: inline-block;");
            sb.AppendLine("                border-style: outset;");
            sb.AppendLine("                border-color: blue;");
            sb.AppendLine("                border-width: 4px;");
            sb.AppendLine("            }");
            sb.AppendLine("            span.small {");
            sb.AppendLine("                font-size: small;");
            sb.AppendLine("            }");
            sb.AppendLine("            body {text-align: center;}");
            sb.AppendLine("        </style>");
            sb.AppendLine("    </head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class=\"outset\">");
            sb.AppendLine("        <table class=\"center\">");
            sb.AppendLine("            <tr>");
            sb.AppendLine("                <td colspan=\"3\">Whispering Audo Player Control</td>");
            sb.AppendLine("            </tr>");
            sb.AppendLine("            <tr>");
            sb.AppendLine("                <td><button type=\"button\" onclick=\"previous()\">Prev</button></td>");
            sb.AppendLine("                <td><button type=\"button\" onclick=\"play()\">Play</button></td>");
            sb.AppendLine("                <td><button type=\"button\" onclick=\"next()\">Next</button></td>");
            sb.AppendLine("            </tr>");
            sb.AppendLine("            <tr>");
            sb.AppendLine("                <td></td>");
            sb.AppendLine("                <td><button type=\"button\" onclick=\"stop()\">Stop</button></td>");
            sb.AppendLine("                <td></td>");
            sb.AppendLine("            </tr>");
            sb.AppendLine("            <tr>");
            sb.AppendLine("                <td colspan=\"3\">");
            sb.AppendLine("                    <button type=\"button\" onclick=\"changeVolume('down')\">-</button>");
            sb.AppendLine("                    &nbsp; Volume &nbsp;");
            sb.AppendLine("                    <button type=\"button\" onclick=\"changeVolume('up')\">+</button>");
            sb.AppendLine("                </td>");
            sb.AppendLine("            </tr>");
            sb.AppendLine("            <tr>");
            sb.AppendLine("                <td colspan=\"3\">");
            sb.AppendLine("                    <span class=\"small\" id=\"volumeLabel\">Volume: </span>");
            sb.AppendLine("                </td>");
            sb.AppendLine("            </tr>");
            sb.AppendLine("        </table>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            sb.AppendLine(GetPlayerScript());


            return new HtmlString(sb.ToString()).ToString();
        }



        private static String GetPlayerScript()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script>");

            sb.Append(GetSimpleScriptBlock("play"));
            sb.Append(GetSimpleScriptBlock("stop"));
            sb.Append(GetSimpleScriptBlock("next"));
            sb.Append(GetSimpleScriptBlock("previous"));
            sb.Append(GetScriptBlockWithParameter("changeVolume","direction", true,"volumeLabel", "Volume: ", "ChangeVolumeResult"));

            sb.AppendLine("</script>");

            return sb.ToString();
        }

        private static String GetSimpleScriptBlock(string command)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("function " + command + "() {");
            sb.AppendLine("var xhttp = new XMLHttpRequest();");
            sb.AppendLine("xhttp.open(\"GET\", \"http://" + GetLocalIPAddress() + ":9090/wamp/" + command + "\", true);");
            sb.AppendLine("xhttp.send();");
            sb.AppendLine("}");
            sb.AppendLine("");

            return sb.ToString();
        }

        private static String GetScriptBlockWithParameter(string command, string parameterName, 
            bool updateLabel, string labelId, string labelText, string jsonVariableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("function " + command + "(" + parameterName + ") {");
            sb.AppendLine("var url = \"http://" + GetLocalIPAddress() + ":9090/wamp/" + command + "/\" + " + parameterName);
            sb.AppendLine("var xhttp = new XMLHttpRequest();");

            sb.AppendLine("xhttp.onreadystatechange = function() {");
            if (updateLabel)
            {
                sb.AppendLine("if (xhttp.readyState == 4 && xhttp.status == 200) {");
                sb.AppendLine("var Data = JSON.parse(xhttp.responseText);");
                sb.AppendLine("document.getElementById(\"" + labelId + "\").innerHTML = \"" + labelText + "\" + Data." + jsonVariableName + ";");
                sb.AppendLine("}");
                sb.AppendLine("};");
            }

            sb.AppendLine("xhttp.open(\"GET\", url, true);");
            sb.AppendLine("xhttp.send();");
            sb.AppendLine("}");
            sb.AppendLine("");

            return sb.ToString();
        }


    }
}
