using System;

namespace WhisperingAudioMusicPlayer
{
    public class VolumeEnabledEventArgs : EventArgs
    {
        private bool isVolumeEnabled;

        public VolumeEnabledEventArgs(bool isVolumeEnabled)
        {
            this.isVolumeEnabled = isVolumeEnabled;
        }

        public bool IsVolumeEnabled
        {
            get { return isVolumeEnabled; }
        }

    }
}
