using System;
using WhisperingAudioMusicEngine;

namespace WhisperingAudioMusicPlayer
{
    public class SelectedOutputChangedEventArgs : EventArgs
    {
        private AudioOutput output;

        public SelectedOutputChangedEventArgs(AudioOutput audioOutput)
        {
            output = audioOutput;
        }

        public AudioOutput SelectedOutput
        {
            get { return output; }
        }
    }
}
