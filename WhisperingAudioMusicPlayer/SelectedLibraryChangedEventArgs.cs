using System;
using WhisperingAudioMusicLibrary;

namespace WhisperingAudioMusicPlayer
{
    public class SelectedLibraryChangedEventArgs : EventArgs
    {
        private MusicLibrary library;

        public SelectedLibraryChangedEventArgs(MusicLibrary ml)
        {
            library = ml;
        }

        public MusicLibrary SelectedLibrary
        {
            get { return library; } 
        }
    }
}
