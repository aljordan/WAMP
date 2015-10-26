using System;

namespace WhisperingAudioMusicPlayer
{
    public class MemoryPlayEnabledEventArgs : EventArgs
    {
        private bool isMemoryPlayEnabled;

        public MemoryPlayEnabledEventArgs(bool isMemoryPlayEnabled)
        {
            this.isMemoryPlayEnabled = isMemoryPlayEnabled;
        }

        public bool IsMemoryPlayEnabled
        {
            get { return isMemoryPlayEnabled; }
        }

    }
}

