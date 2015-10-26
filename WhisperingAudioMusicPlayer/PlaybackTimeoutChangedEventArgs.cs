using System;

namespace WhisperingAudioMusicPlayer
{
    public class PlaybackTimeoutChangedEventArgs : EventArgs
    {
        private int playbackTimeout = 0;

        public PlaybackTimeoutChangedEventArgs() { }

        public PlaybackTimeoutChangedEventArgs(int playbackTimeout)
        { 
            this.playbackTimeout = playbackTimeout; 
        }

        public int PlaybackTimeout
        {
            get { return playbackTimeout; }
        }
    }
}
