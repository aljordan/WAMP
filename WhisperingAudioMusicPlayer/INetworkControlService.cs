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

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "artists/")]
        string GetArtists();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "albums/")]
        string GetAlbums();

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
        /// Toggles Play / Pause.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "toggleplay/")]
        void TogglePlay();

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
        /// Searches the music library by artist, album, or song.
        /// </summary>
        /// <param name="searchText">The text to search for</param>
        /// <param name="searchType">should be "artist", "album", or "song"</param>
        /// <returns>String representation of json representing a list of Tracks for song search,
        /// a list of artist names for artist search, or a list of album names for album search.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "search/{searchtext}/{searchtype}")]
        string Search(string searchText, string searchType);


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
        /// Moves to a place in the currently playing song
        /// </summary>
        /// <param name="percentage>
        /// A string representation of a decimal between 0 and 1 describing the percentage of the song to move to
        /// </param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "movesongto/{percentage}")]
        void MoveToInSong(string percentage);


        /// <summary>
        /// Add a track to the current playlist
        /// </summary>
        /// <param name="id">string representing the library track id</param>
        /// <returns>JSON string representing current playlist</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "addtrack/{id}")]
        string AddTrack(string id);

        /// <summary>
        /// Add an album to the current playlist
        /// </summary>
        /// <param name="album">string representing the album title</param>
        /// <returns>JSON string representing current playlist</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "addalbum/{album}")]
        string AddAlbum(string album);


        /// <summary>
        /// Add all tracks from an artist to the current playlist
        /// </summary>
        /// <param name="album">string representing the artist name</param>
        /// <returns>JSON string representing current playlist</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "addartist/{artist}")]
        string AddArtist(string artist);


        /// <summary>
        /// Remove a track from the current playlist
        /// </summary>
        /// <param name="id">string representing the library track id</param>
        /// <returns>JSON string representing current playlist</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "removetrack/{id}")]
        string RemoveTrack(string id);


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

        /// <summary>
        /// Set repeat
        /// </summary>
        /// <param name="repeat">string that should be "true" or "false"</param>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "setrepeat/{repeat}")]
        void SetRepeat(string repeat);

        /// <summary>
        /// Get repeat
        /// </summary>
        /// <returns>bool</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "getrepeat/")]
        string GetRepeat();

        /// <summary>
        /// Set random
        /// </summary>
        /// <param name="repeat">string that should be "true" or "false"</param>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "setrandom/{random}")]
        void SetRandom(string random);

        /// <summary>
        /// Get Random
        /// </summary>
        /// <returns>bool</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "getrandom/")]
        string GetRandom();

        
        /// <summary>
        /// Gets current playing song info in JSON. Blank string if nothing playing
        /// </summary>
        /// <returns>String</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "songinfo/")]
        string GetCurrentSongInfo();

        /// <summary>
        /// Get image for html page
        /// </summary>
        /// <param name="imageName">image file name</param>
        /// <returns>Stream of image file</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "getimage/{imageName}")]
        Stream GetImage(string imageName);

        /// <summary>
        /// Get album art for album
        /// </summary>
        /// <param name="albumTitle">title of album</param>
        /// <returns>Stream of image file</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "albumartbyalbumtitle/{albumTitle}")]
        Stream GetAlbumArtByAlbumTitle(string albumTitle);

        /// <summary>
        /// Get album art for song
        /// </summary>
        /// <param name="albumTitle">title of album</param>
        /// <returns>Stream of image file</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "albumartbysongid/{id}")]
        Stream GetAlbumArtBySongId(string id);

        void SetPlayer(ucPlayer master);
    }
}
