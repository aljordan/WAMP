using System;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using WhisperingAudioMusicEngine;
using WhisperingAudioMusicLibrary;
using System.IO;
using System.Web.Script.Serialization;



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

        public string GetGenres()
        {
            return new JavaScriptSerializer().Serialize(player.SelectedLibrary.GetGenres());
        }

        public string GetArtists()
        {
            return new JavaScriptSerializer().Serialize(player.SelectedLibrary.GetArtists());
        }

        public string GetAlbums()
        {
            return new JavaScriptSerializer().Serialize(player.SelectedLibrary.GetAlbums());
        }

        public string GetArtistByGenre(string genre)
        {
            return new JavaScriptSerializer().Serialize(player.SelectedLibrary.GetArtistsByGenre(genre));
        }

        public string GetAlbumsByArtist(string artist)
        {
            return new JavaScriptSerializer().Serialize(player.SelectedLibrary.GetAlbumsByArtist(artist));
        }

        public string GetSongsByAlbum(string album)
        {
            return new JavaScriptSerializer().Serialize(player.SelectedLibrary.GetSongsByAlbum(album));
        }


        public string Play()
        {
            return player.Play();
        }

        public string Stop()
        {
            player.Stop();
            return "Success";
        }

        public string Next()
        {
            return player.Next();
        }
        
        public string Previous()
        {
            return player.Previous();
        }

        public string ChangeVolume(string direction)
        {
            return player.ChangeVolume(direction);
        }

        public string PlayTrack(string id)
        {
            return player.PlayPlaylistTrack(Convert.ToInt64(id));
        }

        public string AddTrack(string id)
        {
            Track song = player.SelectedLibrary.GetSongById(Convert.ToInt64(id));
            return player.AddTrackToPlaylist(song);
        }

        public string RemoveTrack(string id)
        {
            Track song = player.SelectedLibrary.GetSongById(Convert.ToInt64(id));
            return player.RemoveTrackFromPlaylist(song);
        }

        public string GetVolume()
        {
            return player.GetVolume();
        }

        public string CurrentPlaylist()
        {
            return player.GetCurrentPlaylistJSON();
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
