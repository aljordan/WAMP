using System;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using WhisperingAudioMusicEngine;
using System.IO;

namespace WhisperingAudioMusicPlayer
{
    class NetworkControlService : INetworkControlService
    {
        private Uri baseAddress;
        private static ucPlayer player;
        WebServiceHost host;

        public NetworkControlService()
        {
            baseAddress = new Uri("http://localhost:9090/wamp");
        }


        //TODO:
        // Currently the only way to make this work is to run the program as an administrator, or before it is
        // launched, typw the following into an ADMINISTRATIVE Command Prompt:
        // netsh http add urlacl url=http://+:9090/ user="NTAuthority\Authenticated Users" sddl="D:(A;;GX;;;AU)"
        // I need to figure out if the Install Sheild project can do this automatically

        public void startNetworkService()
        {
            host = new WebServiceHost(typeof(NetworkControlService),baseAddress);

            ServiceEndpoint ep = host.AddServiceEndpoint(typeof(INetworkControlService), new WebHttpBinding(), "");

            ServiceDebugBehavior stp = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            stp.HttpHelpPageEnabled = false;

            // Open the ServiceHost to start listening for messages. Since
            // no endpoints are explicitly configured, the runtime will create
            // one endpoint per base address for each service contract implemented
            // by the service.
            host.Open();
        }

        public void stopNetworkService()
        {
            // Close the ServiceHost.
            host.Close();
        }

        public string AudioOutputs()
        {
            string outputs = "";
            foreach (AudioOutput output in WhisperingAudioMusicEngine.AudioOutputs.GetDeviceList())
            {
                outputs += output.DeviceName + " ";
            }
            return outputs;
        }

        public string Play()
        {
            player.Play();
            return "Success";
        }

        public string Stop()
        {
            player.Stop();
            return "Success";
        }

        public string Next()
        {
            player.Next();
            return "Success";
        }
        
        public string Previous()
        {
            player.Previous();
            return "Success";
        }

        public string ChangeVolume(string direction)
        {
            return player.ChangeVolume(direction);
        }

        public void SetPlayer(ucPlayer master)
        {
            player = master;
        }

        public Stream GetPlayer()
        {
            var memoryStream = new MemoryStream();
            //using (var memoryStream = new MemoryStream())
            //{
                StreamWriter writer = new StreamWriter(memoryStream);
                //using (StreamWriter writer = new StreamWriter(memoryStream))
                //{
                    writer.Write(PlayerHTMLBuilder.GetPlayerHtml());
                    writer.Flush();
                    memoryStream.Position = 0;
                    var context = WebOperationContext.Current;

                    if (context != null)
                    {
                        context.OutgoingResponse.ContentType = "text/html";
                    }
                    return memoryStream;
                //}
            //}
        }

    }
}
