using System;
using System.Text;

namespace WhisperingAudioMusicPlayer
{
    class SongInfo
    {
        private int songLength = 0;
        private int songProgress = 0;
        private string songTitle;
        private string artist;
        private string album;

        public SongInfo() { }

        public SongInfo(string title, string artist, string album)
        {
            this.songTitle = title;
            this.album = album;
            this.artist = artist;
        }

        public SongInfo(string title, string artist, string album, int length)
            : this(title, artist, album)
        {
            this.songLength = length;
        }

        public SongInfo(string title, string artist, string album, int length, int progress)
            : this(title, artist, album, length)
        {
            this.songProgress = progress;
        }

        public string SongTitle
        {
            get { return songTitle; }
            set { songTitle = value;}
        }

        public string Artist
        {
            get { return artist; }
            set { artist = value; }
        }

        public string Album
        {
            get { return album; }
            set { album = value; }
        }

        public int SongLength
        {
            get { return songLength; }
            set { songLength = value; }
        }

        public int SongProgress
        {
            get { return songProgress; }
            set { songProgress = value; }
        }
    }
}
