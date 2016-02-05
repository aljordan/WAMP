using System;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.IO; //for stream class

namespace WhisperingAudioMusicPlayer
{
    /// <summary>
    /// The interfaces for the REST API calls that control the player from a web browser (or anything else)
    /// </summary>
    [ServiceContract]
    interface INetworkControlService
    {
        //[OperationContract]
        //string AudioOutputs();

        /// <summary>
        /// Returns the web page that can control the player.
        /// </summary>
        /// <returns>Stream of HTML</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "player/")]
        Stream GetPlayer();


        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "outputs/")]
        string AudioOutputs();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "genres/")]
        string GetGenres();

        /// <summary>
        /// Starts playback on the player if a playlist is loaded.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "play/")]
        string Play();

        /// <summary>
        /// Stops the player
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "stop/")]
        string Stop();

        /// <summary>
        /// Moves to the next song in the playlist
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "next/")]
        string Next();

        /// <summary>
        /// Moves to the previous song in the playlist
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "previous/")]
        string Previous();

        /// <summary>
        /// Gets current playlist
        /// </summary>
        /// <returns>String of JSON</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "currentplaylist/")]
        string CurrentPlaylist();

        /// <summary>
        /// Lowers or raises the volume if enabled in the Player
        /// </summary>
        /// <param name="direction">String that should be "down" or "up"</param>
        /// <returns>String representing the current volume of the player</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "changevolume/{direction}")]
        string ChangeVolume(string direction);

        /// <summary>
        /// Get all artist in a genre
        /// </summary>
        /// <param name="genre">Musical genre</param>
        /// <returns>String of json representing a list of all artist in a genre</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "artistsbygenre/{genre}")]
        string GetArtistByGenre(string genre);

        /// <summary>
        /// Get albums by an artist
        /// </summary>
        /// <param name="genre">Artist name</param>
        /// <returns>String of json representing a list of all albums by an artist</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "albumsbyartist/{artist}")]
        string GetAlbumsByArtist(string artist);

        /// <summary>
        /// Get songs by an album
        /// </summary>
        /// <param name="genre">Album name</param>
        /// <returns>String of json representing a list of all songs by an album</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "songsbyalbum/{album}")]
        string GetSongsByAlbum(string album);

        /// <summary>
        /// Play a specific playlist track
        /// </summary>
        /// <param name="id">string representing the library track id</param>
        /// <returns>JSON string representing current playing track, or empty string if track not found</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "playtrack/{id}")]
        string PlayTrack(string id);


        /// <summary>
        /// Add a track to the current playlist
        /// </summary>
        /// <param name="id">string representing the library track id</param>
        /// <returns>JSON string representing success or failure</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "addtrack/{id}")]
        string AddTrack(string id);


        /// <summary>
        /// Gets current player volume
        /// </summary>
        /// <returns>String</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "volume/")]
        string GetVolume();


        void SetPlayer(ucPlayer master);
    }
}
