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
        private long id;
        private bool isPaused;


        public SongInfo() { }

        public SongInfo(string title, string artist, string album, long id)
        {
            this.songTitle = title;
            this.album = album;
            this.artist = artist;
            this.id = id;
        }

        public SongInfo(string title, string artist, string album, long id, int length)
            : this(title, artist, album, id)
        {
            this.songLength = length;
        }

        public SongInfo(string title, string artist, string album, long id, int length, int progress, bool isPaused)
            : this(title, artist, album, id, length)
        {
            this.songProgress = progress;
            this.isPaused = isPaused;
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


        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }
    }
}
