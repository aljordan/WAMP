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

        void SetPlayer(ucPlayer master);
    }
}
