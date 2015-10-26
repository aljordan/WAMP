using System;

namespace WhisperingAudioMusicEngine
{
    public class PlaybackStatusEventArgs : EventArgs
    {
        private PlaybackStatus status;

        public PlaybackStatusEventArgs()
        {
        }

        public PlaybackStatusEventArgs(PlaybackStatus status)
        {
            this.status = status;
        }

        public PlaybackStatus Status
        {
            get { return status; }
        }
    }
}
