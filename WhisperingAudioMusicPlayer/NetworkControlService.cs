using System;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using WhisperingAudioMusicEngine;
using WhisperingAudioMusicLibrary;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


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

        // The above TODO has been completed as install events in the installer package.

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

        public void TogglePlay()
        {
            player.TogglePlayPause();
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

        public void MoveToInSong(string percentage)
        {
            player.MoveToInSong(Convert.ToDecimal(percentage));
        }

        public string AddTrack(string id)
        {
            Track song = player.SelectedLibrary.GetSongById(Convert.ToInt64(id));
            return player.AddTrackToPlaylist(song);
        }

        public string AddAlbum(string album)
        {
            List<Track> songs = player.SelectedLibrary.GetSongsByAlbum(album);
            return player.AddTracksToPlaylist(songs);
        }

        public string AddArtist(string artist)
        {
            List<Track> songs = player.SelectedLibrary.GetSongsByArtist(artist);
            return player.AddTracksToPlaylist(songs);
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

        public string GetRepeat()
        {
            return player.IsRepeating.ToString();
        }

        public void SetRepeat(string repeat)
        {
            player.IsRepeating = Convert.ToBoolean(repeat);
        }

        public string GetRandom()
        {
            return player.IsRandom.ToString();
        }

        public void SetRandom(string random)
        {
            player.IsRandom = Convert.ToBoolean(random);
        }

        public string GetCurrentSongInfo()
        {
            return player.GetCurrentSongInfo();
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


        public Stream GetImage(string imageName)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "HtmlBuilder\\images\\" + imageName;
            if (File.Exists(filePath))
            {
                FileStream fs = File.OpenRead(filePath);
                WebOperationContext.Current.OutgoingRequest.ContentType = "image/png";
                return fs;
            }
            else
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(" Requested Image does not exist :(");
                MemoryStream strm = new MemoryStream(byteArray);
                return strm;
            }
        }

        public Stream GetAlbumArtByAlbumTitle(string albumTitle)
        {
            Image img;
            string path = player.SelectedLibrary.GetAlbumArtPathByAlbumName(albumTitle);
            if (path != "")
            {
                img = Image.FromFile(path);
                img = ResizeImage(img, new System.Drawing.Size(100, 100));
            }
            else
            {
                img = new Bitmap(100, 100);
            }

            MemoryStream memStream = new MemoryStream();
            img.Save(memStream, ImageFormat.Jpeg);

            memStream.Position = 0;
            WebOperationContext.Current.OutgoingRequest.ContentType = "image/jpeg";
            return memStream;
        }

        public Stream GetAlbumArtBySongId(string id)
        {
            Image img;
            string path = player.SelectedLibrary.GetAlbumArtPathBySongID(Convert.ToInt64(id));
            if (path != "")
            {
                img = Image.FromFile(path);
                img = ResizeImage(img, new System.Drawing.Size(100, 100));
            }
            else
            {
                img = new Bitmap(100, 100);
            }

            MemoryStream memStream = new MemoryStream();
            img.Save(memStream, ImageFormat.Jpeg);

            memStream.Position = 0;
            WebOperationContext.Current.OutgoingRequest.ContentType = "image/jpeg";
            return memStream;
        }


        private static Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

    }
}
