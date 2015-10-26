using WhisperingAudioMusicLibrary;

namespace WhisperingAudioMusicPlayer
{
    public class PlaylistEventArgs
    {
        private Playlist pl;
        
        public PlaylistEventArgs(Playlist playlist)
        {
            pl = playlist;
        }

        public Playlist SelectedPlaylist
        {
            get { return pl; }
        }

    }
}
