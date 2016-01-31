using System;

namespace WhisperingAudioMusicPlayer
{
    public class NetworkControlEnabledEventArgs : EventArgs
    {
        private bool isNetworkControlEnabled;

        public NetworkControlEnabledEventArgs(bool isNetworkControlEnabled)
        {
            this.isNetworkControlEnabled = isNetworkControlEnabled;
        }

        public bool IsNetworkControlEnabled
        {
            get { return isNetworkControlEnabled; }
        }
    }
}
