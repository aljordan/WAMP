using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WhisperingAudioMusicLibrary;
using WhisperingAudioMusicEngine;
// Below are for network features
using System.ServiceModel;
using System.ServiceModel.Description;


namespace WhisperingAudioMusicPlayer
{
    // TODO: Under certain conditions, when random play is selected, playlist is playing linearly.
    // Find out why and fix



    /// <summary>
    /// Interaction logic for ucPlayer.xaml
    /// </summary>
    public partial class ucPlayer : UserControl
    {
        private NetworkControlService networkControlService;
        enum ButtonClicked { Play, Pause, Stop };
        private Playlist currentPlaylist;
        private AudioOutput selectedOutput;
        private MusicEngine musicEngine;
        private Track currentTrack;
        private bool isRepeating;
        private bool isRandom;
        private bool isPlaying;  //logic to see if we should move to next song in playlist
        private bool isPaused; // for use in determining if play button should press pause or play
        private System.Windows.Forms.Timer playbackTimer;
        private Shuffler shuffled;
        private bool hasReadFlacAhead;
        private bool isLooping;
        private bool isLoopStarting;
        private int loopStart;
        private int loopEnd;
        private bool isVolumeEnabled;
        private bool isAcourateVolumeEnabled;

        public ucPlayer()
        {
            InitializeComponent();
            musicEngine = new MusicEngine();
            IsRepeating = Properties.Settings.Default.IsRepeating;
            IsRandom = Properties.Settings.Default.IsRandom;
            //musicEngine.PlaybackTimerEvent += HandlePlaybackTimerEvent;
            musicEngine.SongFinishedEvent += HandleSongFinishedEvent;
            playbackTimer = new System.Windows.Forms.Timer {Interval = 50};
            playbackTimer.Tick += HandleLocalPlaybackTimerEvent;
            if (Properties.Settings.Default.IsVolumeEnabled)
            {
                isVolumeEnabled = true;
                double volume = Properties.Settings.Default.CurrentVolume;
                sldrVolume.Value = volume;

                if (Properties.Settings.Default.IsAcourateVolumeEnabled)
                {
                    isAcourateVolumeEnabled = true;
                    musicEngine.IsVolumeEnabled = false;
                    sldrVolume.Focusable = false;
                    sldrVolume.LargeChange = 0;
                }
                else
                    musicEngine.IsVolumeEnabled = true;

                float dbVolume;
                double dB = (volume - 601) / 10;
                if (dB < -60)
                    dbVolume = 0;
                else
                    dbVolume = (float)Math.Pow(10, dB / 20);


                musicEngine.Volume = dbVolume;
                if (sldrVolume.Value == 0)
                    lblVolumeContent.Content = "Muted";
            }
            else
            {
                isVolumeEnabled = false;
                isAcourateVolumeEnabled = false;
                sldrVolume.Value = 601;
                sldrVolume.IsEnabled = false;
                musicEngine.IsVolumeEnabled = false;
            }

            if (Properties.Settings.Default.IsMemoryPlayEnabled)
                musicEngine.MemoryPlay = true;
            else
                musicEngine.MemoryPlay = false;


            /* 
             * Pulling the following out of the constructor and into a separate
             * method that is called from the main window after all children
             * have loaded.  This is because the various (Image)template.FindName("imageShuffle", btnShuffle);
             * calls return null at this point in the initialization workflow.

             *** IsRepeating = Properties.Settings.Default.IsRepeating;
             *** IsRandom = Properties.Settings.Default.IsRandom;
             * */
        }

        internal void InitButtons()
        {
            IsRepeating = Properties.Settings.Default.IsRepeating;
            IsRandom = Properties.Settings.Default.IsRandom;

            networkControlService = new NetworkControlService();
            networkControlService.startNetworkService();
            networkControlService.SetPlayer(this);

        }

        public void Cleanup()
        {
            musicEngine.Stop();
            Thread.Sleep(500);
            musicEngine.Dispose();
            List<Track> list = new List<Track>();
            foreach (Track t in lstNowPlaying.Items)
            {
                list.Add(t);
            }
            Playlist p = new Playlist("default", list);
            p.SavePlaylist();

            networkControlService.stopNetworkService();
        }

        public Track CurrentTrack
        {
            get { return currentTrack; }
            set 
            { 
                currentTrack = value;
                foreach (Track t in lstNowPlaying.Items)
                    if (t.Equals(currentTrack))
                    {
                        lstNowPlaying.SelectedItem = t;
                        lstNowPlaying.ScrollIntoView(lstNowPlaying.SelectedItem);
                        break;
                    }
            }
        }

        public void Play()
        {
            btnPlay.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            isPaused = false;
        }

        public void Stop()
        {
            btnStop.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));

        }

        public void Next()
        {
            btnNext.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));

        }

        public void Previous()
        {
            btnPrevious.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));

        }

        public string ChangeVolume(string direction)
        {
            Console.WriteLine(direction);
            if (!isVolumeEnabled)
                return "Volume not enabled in player.";

            double volume = sldrVolume.Value;
            switch (direction.ToLower())
            {
                case "up":
                    volume += 10;
                    if (volume > 601.0)
                        volume = 601;
                    break;
                case "down":
                    volume -= 10;
                    if (volume < 0)
                        volume = 0;
                    break;
                default:
                    return "Failure: Bad direction command";
            }

            sldrVolume.Value = volume;

            float dbVolume;
            double dB = (volume - 601) / 10;
            if (dB < -60)
                dbVolume = 0;
            else
                dbVolume = (float)Math.Pow(10, dB / 20);

            musicEngine.Volume = dbVolume;
            if (sldrVolume.Value == 0)
                lblVolumeContent.Content = "Muted";

            return lblVolumeContent.Content.ToString();
        }

        public bool IsRandom
        {
            get { return isRandom; }
            set 
            { 
                isRandom = value;
                if (isRandom)
                {
                    var template = btnShuffle.Template;
                    var imageControl = (Image)template.FindName("imageShuffle", btnShuffle);
                    var uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnRandomMouseOverSmall.png", UriKind.Relative);
                    if (imageControl != null)
                        imageControl.Source = new BitmapImage(uriSource);
                    if (currentPlaylist != null)
                        shuffled = new Shuffler(currentPlaylist);
                }
                else
                {
                    var template = btnShuffle.Template;
                    var imageControl = (Image)template.FindName("imageShuffle", btnShuffle);
                    var uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnRandomSmall.png", UriKind.Relative);
                    if (imageControl != null)
                        imageControl.Source = new BitmapImage(uriSource);
                    shuffled = null;
                }
            }
        }

        public bool IsRepeating
        {
            get { return isRepeating; }
            set
            {
                isRepeating = value;
                if (isRepeating)
                {
                    var template = btnRepeat.Template;
                    var imageControl = (Image)template.FindName("imageRepeat", btnRepeat);
                    var uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnRepeatMouseOverSmall.png", UriKind.Relative);
                    if (imageControl != null)
                        imageControl.Source = new BitmapImage(uriSource);
                }
                else
                {
                    var template = btnRepeat.Template;
                    var imageControl = (Image)template.FindName("imageRepeat", btnRepeat);
                    var uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnRepeatSmall.png", UriKind.Relative);
                    if (imageControl != null)
                        imageControl.Source = new BitmapImage(uriSource);
                }
            }
        }

        public Playlist CurrentPlaylist
        {
            get { return currentPlaylist; }
            set
            {
                int selectIndex = 0;  //track to select in new playlist
                bool found = false;
                currentPlaylist = value;
                lstNowPlaying.Items.Clear();
                foreach (Track song in currentPlaylist)
                {
                    lstNowPlaying.Items.Add(song);
                    if (currentTrack != null  && !found)
                    {
                        if (song.Equals(currentTrack))
                        {
                            selectIndex = lstNowPlaying.Items.Count - 1;
                            found = true;
                        }
                    }
                }
                
                lstNowPlaying.SelectedIndex = selectIndex;
                currentTrack = (Track)lstNowPlaying.SelectedItem;

                if (isRandom)
                {
                    shuffled = new Shuffler(currentPlaylist);
                }

                if (lstNowPlaying.Items.Count > 0 && !isPlaying && selectedOutput != null)
                    Play();
            }
        }

        public bool IsVolumeEnabled
        {
            set
            {
                isVolumeEnabled = value;
                musicEngine.IsVolumeEnabled = value;
                if (value == true)
                {
                    sldrVolume.IsEnabled = true;
                    sldrVolume.Value = Properties.Settings.Default.CurrentVolume;
                    musicEngine.Volume = (float)(Properties.Settings.Default.CurrentVolume / 601);
                }
                else
                {
                    sldrVolume.Value = 601;
                    sldrVolume.IsEnabled = false;
                }
            }
        }

        public bool IsAcourateVolumeEnabled
        {
            set
            {
                isAcourateVolumeEnabled = value;
                if (value == true)
                {
                    musicEngine.IsVolumeEnabled = false;
                    sldrVolume.IsEnabled = true;
                    sldrVolume.Value = Properties.Settings.Default.CurrentVolume;
                    sldrVolume.LargeChange = 0;
                    sldrVolume.Focusable = false;
                    //musicEngine.Volume = (float)(Properties.Settings.Default.CurrentVolume / 601);  
                }
                else
                {
                    if (isVolumeEnabled)
                    {
                        sldrVolume.LargeChange = 10;
                        sldrVolume.Focusable = true;
                        musicEngine.IsVolumeEnabled = true;
                    }
                    else
                    {
                        musicEngine.IsVolumeEnabled = false;
                        sldrVolume.Value = 601;
                        sldrVolume.IsEnabled = false;
                    }
                }
            }
        }

        public bool IsMemoryPlayEnabled
        {
            set
            {
                musicEngine.MemoryPlay = value;
            }
        }

        public AudioOutput SelectedOutput
        {
            get { return selectedOutput; }
            set { selectedOutput = value; }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (selectedOutput == null)
            {
                MessageBox.Show("You must select an audio output in the preferences tab before playing.");
                return;
            }
            if (lstNowPlaying.Items.Count == 0)
            {
                MessageBox.Show("Please create a playlist before playing.");
                return;
            }

            if (isPaused)
            {
                //timeoutValue = 0;
                btnPause.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                return;
            }

            if (isLooping || isLoopStarting)
                resetLooping();

            if (lstNowPlaying.SelectedIndex == -1)
            {
                if (isRandom)
                {
                    shuffled = new Shuffler(currentPlaylist);
                    CurrentTrack = shuffled.MoveToNextTrack();
                }
                else
                    lstNowPlaying.SelectedIndex = 0;
            }
            else
            {
                if (!isPlaying && isRandom)
                {
                    shuffled = new Shuffler(currentPlaylist,(Track)lstNowPlaying.SelectedItem);
                }
            }
            PlayTrack((Track)lstNowPlaying.SelectedItem);
            isPlaying = true;
            playbackTimer.Enabled = true;
            //timeoutValue = 0;
            hasReadFlacAhead = false;
            isPaused = false;
            lstNowPlaying.ScrollIntoView(lstNowPlaying.SelectedItem);
        }


        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            bool ispaused = musicEngine.Pause();
            if (ispaused)
            {
                playbackTimer.Enabled = false;
                isPaused = true;
            }
            else
            {
                playbackTimer.Enabled = true;
                isPaused = false;
            }
            SetButtons(ButtonClicked.Pause);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            playbackTimer.Enabled = false;
            isPlaying = false;
            musicEngine.Stop();
            //timeoutValue = 0;
            isPaused = false;
            prgBarElapsedTime.Value = 0;
            lblCurrentTime.Content = "";
            lblTotalTime.Content = "";
            resetLooping();
            SetButtons(ButtonClicked.Stop);
        }

        private void ListBoxNowPlayingItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstNowPlaying.SelectedIndex == -1)
                return;
            //PlayTrack((Track)lstNowPlaying.SelectedItem);
            isPlaying = true; //setting this so random play doesn't choose something else
            btnPlay.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
        }

        // When shuffler reaches the end of the playlist and next called, it won't return any more songs.
        // the player should check for end of playlist and create a new shuffler.
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            isPaused = false;
            if (lstNowPlaying.SelectedIndex == -1)
                return;

            if (!isRandom)
            {
                if (lstNowPlaying.SelectedIndex == lstNowPlaying.Items.Count - 1)
                {
                    if (isRepeating != true)
                    {
                        //btnStop.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                        return;
                    }
                    else
                        lstNowPlaying.SelectedIndex = 0;
                }
                else
                    lstNowPlaying.SelectedIndex = lstNowPlaying.SelectedIndex + 1;
            }
            else //random play
            {
                if (isRepeating != true)
                {
                    Track nextTrack = shuffled.MoveToNextTrack();
                    if (nextTrack == null)
                    {
                        //btnStop.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                        return;
                    }
                    CurrentTrack = nextTrack;
                }
                else
                {
                    CurrentTrack = shuffled.MoveToNextTrackRepeat();
                }
            }

            //PlayTrack((Track)lstNowPlaying.SelectedItem);
            btnPlay.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            isPaused = false;
            if (lstNowPlaying.SelectedIndex == -1)
                return;

            int currentSelectedIndex = lstNowPlaying.SelectedIndex;

            if (!isRandom)
            {
                if (lstNowPlaying.SelectedIndex == 0)
                {
                    if (isRepeating != true)
                    {
                        //btnStop.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                        return;
                    }
                    else
                        lstNowPlaying.SelectedIndex = lstNowPlaying.Items.Count - 1;
                }
                else
                    lstNowPlaying.SelectedIndex = lstNowPlaying.SelectedIndex - 1;
            }
            else //random play
            {
                Track previousTrack = shuffled.MoveToPreviousTrack();
                if (previousTrack == null)
                {
                    //btnStop.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    return;
                }
                else
                {
                    CurrentTrack = previousTrack;
                }
            }

            //PlayTrack((Track)lstNowPlaying.SelectedItem);
            if (currentSelectedIndex != lstNowPlaying.SelectedIndex)
                btnPlay.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
        }


        private void btnRepeat_Click(object sender, RoutedEventArgs e)
        {
            if (isRepeating)
                IsRepeating = false;
            else
                IsRepeating = true;

            Properties.Settings.Default.IsRepeating = isRepeating;
            Properties.Settings.Default.Save();
        }

        private void PlayTrack(Track t)
        {
            if (isPlaying)
                musicEngine.Stop();

            currentTrack = t;

            //this.Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        musicEngine.Play(((Track)lstNowPlaying.SelectedItem).FilePath, selectedOutput);
            //    }));

            try
            {
                musicEngine.Play(((Track)lstNowPlaying.SelectedItem).FilePath, selectedOutput);
            }
            catch (InvalidOutputDeviceException e)
            {
                string messageBoxText = "The selected output device: \n" 
                    + e.Output.DeviceName + 
                    "\nis not working properly. Make\nsure the device is turned on.";

                string caption = "Output Device Problem";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(messageBoxText, caption, button, icon);
                return;
            }

            setLabels(t);
            SetButtons(ButtonClicked.Play);
            Properties.Settings.Default.CurrentTrack = currentTrack;
            Properties.Settings.Default.Save();
        }



        private void SetButtons(ButtonClicked button)
        {
            switch (button)
            {
                case ButtonClicked.Play:
                    var template = btnPlay.Template;
                    var imageControl = (Image)template.FindName("imagePlay", btnPlay);
                    var uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnPlayMouseOverSmall.png", UriKind.Relative);
                    imageControl.Source = new BitmapImage(uriSource); 
                    //playImage.Source = new BitmapImage(uriSource);

                    template = btnStop.Template;
                    imageControl = (Image)template.FindName("imageStop", btnStop);
                    uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnStopSmall.png", UriKind.Relative);
                    imageControl.Source = new BitmapImage(uriSource);

                    template = btnPause.Template;
                    imageControl = (Image)template.FindName("imagePause", btnPause);
                    uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnPauseSmall.png", UriKind.Relative);
                    imageControl.Source = new BitmapImage(uriSource);

                    lstNowPlaying.Focus();
                    break;

                case ButtonClicked.Stop:
                    template = btnPlay.Template;
                    imageControl = (Image)template.FindName("imagePlay", btnPlay);
                    uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnPlaySmall.png", UriKind.Relative);
                    imageControl.Source = new BitmapImage(uriSource); 
                    //playImage.Source = new BitmapImage(uriSource);

                    template = btnStop.Template;
                    imageControl = (Image)template.FindName("imageStop", btnStop);
                    uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnStopMouseOverSmall.png", UriKind.Relative);
                    imageControl.Source = new BitmapImage(uriSource);

                    template = btnPause.Template;
                    imageControl = (Image)template.FindName("imagePause", btnPause);
                    uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnPauseSmall.png", UriKind.Relative);
                    imageControl.Source = new BitmapImage(uriSource);

                    lstNowPlaying.Focus();
                    break;

                case ButtonClicked.Pause:
                    if (isPaused)
                    {
                        template = btnPlay.Template;
                        imageControl = (Image)template.FindName("imagePlay", btnPlay);
                        uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnPlaySmall.png", UriKind.Relative);
                        imageControl.Source = new BitmapImage(uriSource);
                        //                        playImage.Source = new BitmapImage(uriSource);

                        template = btnStop.Template;
                        imageControl = (Image)template.FindName("imageStop", btnStop);
                        uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnStopSmall.png", UriKind.Relative);
                        imageControl.Source = new BitmapImage(uriSource);

                        template = btnPause.Template;
                        imageControl = (Image)template.FindName("imagePause", btnPause);
                        uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnPauseMouseOverSmall.png", UriKind.Relative);
                        imageControl.Source = new BitmapImage(uriSource);
                    }
                    else
                    {
                        template = btnPlay.Template;
                        imageControl = (Image)template.FindName("imagePlay", btnPlay);
                        uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnPlayMouseOverSmall.png", UriKind.Relative);
                        imageControl.Source = new BitmapImage(uriSource);

                        template = btnStop.Template;
                        imageControl = (Image)template.FindName("imageStop", btnStop);
                        uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnStopSmall.png", UriKind.Relative);
                        imageControl.Source = new BitmapImage(uriSource);

                        template = btnPause.Template;
                        imageControl = (Image)template.FindName("imagePause", btnPause);
                        uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnPauseSmall.png", UriKind.Relative);
                        imageControl.Source = new BitmapImage(uriSource);

                        lstNowPlaying.Focus();
                    }
                    break;
            }
        }

        private void setLabels(Track t)
        {
            lblTitleContent.Content = t.Title;
            lblArtistContent.Content = t.Artist;
            lblAlbumContent.Content = t.Album;
            if (t.Year != 0)
                lblYearContent.Content = + t.Year;
            else
                lblYearContent.Content = "";
            lblGenreContent.Content = t.Genre;
            //lblBitDepthContent.Content = musicEngine.BitDepth;
            lblSampleRateContent.Content = musicEngine.SampleRate;

            //folder image
            try
            {
                string folderPath = t.FilePath.Substring(0, t.FilePath.LastIndexOf('\\'));
                string imageFile = folderPath + "\\folder.jpg";
                if (System.IO.File.Exists(imageFile))
                {
                    albumImage.StretchDirection = StretchDirection.DownOnly;
                    albumImage.Source = new BitmapImage(new Uri(imageFile));
                    //ImageBrush background = new ImageBrush();
                    //background.ImageSource = new BitmapImage( new Uri(imageFile));
                    //playerMainGrid.Background = background;
                }
                else
                {
                    string[] dirs = System.IO.Directory.GetFiles(folderPath, "*.jpg");
                    if (dirs.Count() > 0)
                    {
                        albumImage.StretchDirection = StretchDirection.DownOnly;
                        albumImage.Source = new BitmapImage(new Uri(dirs[0]));
                    }
                    else
                    {
                        albumImage.Source = null;
                    }
                }
            }
            catch (Exception)
            {
                albumImage.Source = null;
                Console.WriteLine("Trapped exception loading album art");
            }


        }

        private void SetVolumeLabel(double newValue)
        {
            if (newValue == 0)
            {
                //lblVolumeContent.Content = "-" + "\u221E";
                lblVolumeContent.Content = "volume muted";
            }
            else
            {
                double result = (newValue - 601) / 10;
                //lblVolumeContent.Content = result + " dB";
                lblVolumeContent.Content = String.Format("{0:F1} dB", result);
            }
        }


        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            if (isRandom)
            {
                IsRandom = false;
            }
            else
            {
                IsRandom = true;
            }

            Properties.Settings.Default.IsRandom = isRandom;
            Properties.Settings.Default.Save();
        }

        //private void HandlePlaybackTimerEvent(object sender, PlaybackTimerEventArgs ptea)
        //{
        //    //Another thread is updating our UI so have to use Dispatcher BeginInvoke
        //    this.Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        lblCurrentTime.Content = ptea.CurrentTimeInSeconds;
        //        lblTotalTime.Content = ptea.TotalTimeInSeconds;
        //        prgBarElapsedTime.Minimum = 0;
        //        prgBarElapsedTime.Maximum = ptea.TotalTimeInSeconds;
        //        prgBarElapsedTime.Value = ptea.CurrentTimeInSeconds;
        //    }));
        //}

        private void HandleLocalPlaybackTimerEvent(object sender, EventArgs ea)
        {
            int totalTime = musicEngine.GetTotalTrackTimeInSeconds();
            int currentTime = musicEngine.GetCurrentTrackTimeInSeconds();

            if (!hasReadFlacAhead && totalTime > 0 && currentTime > 0 && (currentTime > (totalTime - 90)))
                CheckAndProcessReadFlacAhead();

            // looping
            if (isLooping && currentTime > loopEnd)
            {
                musicEngine.MoveToPositionInSeconds(loopStart);
                return;
            }

            lblCurrentTime.Content = musicEngine.GetCurrentTrackTime();
            lblTotalTime.Content = musicEngine.GetTotalTrackTime();

            prgBarElapsedTime.Minimum = 0;
            if (totalTime > 0)
            {
                prgBarElapsedTime.Maximum = totalTime;
                prgBarElapsedTime.Value = currentTime;
            }
            else
            {
                prgBarElapsedTime.Maximum = 100;
                prgBarElapsedTime.Value = 0;
            }

        }


        private void HandleSongFinishedEvent(object sender, EventArgs ea)
        {
            //Another thread is going after local ui info so have to use Dispatcher BeginInvoke
            Dispatcher.BeginInvoke(DispatcherPriority.Send ,new Action(() =>
            {
                if (isPlaying && !IsLastTrackInPlaylist())
                {
                    isPlaying = false;  // this is to keep from stopping the music engine because we want gapless
                    playbackTimer.Enabled = false;
                    btnNext.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                }
                else
                {
                    btnStop.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    playbackTimer.Enabled = false;
                }
            }));
        }

        private void prgBarElapsedTime_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //double MousePosition = e.GetPosition(prgBarElapsedTime).X;
            decimal pos = ((decimal)e.GetPosition(prgBarElapsedTime).X / (decimal)prgBarElapsedTime.ActualWidth);
            decimal moveTo = pos * musicEngine.GetTotalTrackTimeInSeconds();
            int moveToRounded = (int)Math.Round(moveTo);
            if (moveToRounded <=  musicEngine.GetTotalTrackTimeInSeconds())
                musicEngine.MoveToPositionInSeconds(moveToRounded);
        }

        private bool IsLastTrackInPlaylist()
        {
            if (isRepeating)
                return false;
            else
            {
                if (isRandom)
                    return (shuffled.SeeNextTrack() == null);
                else
                    return (lstNowPlaying.SelectedIndex == lstNowPlaying.Items.Count - 1);
            }
        }

        private void CheckAndProcessReadFlacAhead()
        {
            hasReadFlacAhead = true;
            string nextFileName = null;
            if (!IsLastTrackInPlaylist())
            {
                if (!isRandom)
                {
                    if (lstNowPlaying.SelectedIndex == lstNowPlaying.Items.Count - 1)
                    {
                        if (isRepeating != true)
                        {
                            return;
                        }
                        else
                            nextFileName = ((Track)lstNowPlaying.Items.GetItemAt(0)).FilePath;
                    }
                    else
                        nextFileName = ((Track)lstNowPlaying.Items.GetItemAt(lstNowPlaying.SelectedIndex + 1)).FilePath;
                }
                else //random play
                {
                    if (isRepeating != true)
                    {
                        if (shuffled.SeeNextTrack() == null)
                        {
                            return;
                        }
                        nextFileName = shuffled.SeeNextTrack().FilePath;
                    }
                    else
                    {
                        nextFileName = shuffled.SeeNextTrackRepeat().FilePath;
                    }
                }
            }
            if (nextFileName != null)
            {
                if (nextFileName.ToLower().EndsWith(".flac"))
                {
                    musicEngine.DecodeFlacAhead(nextFileName);
                }
            }
        }

        private async void SendAcourateVolumeDownSmall(double numberOfKeysToSend)
        {
            await Task.Run(() =>
            {
                for (double counter = 0; counter < numberOfKeysToSend; counter++)
                {
                    try
                    {
                        System.Windows.Forms.SendKeys.SendWait("%{DOWN}");
                    }
                    catch (Exception) { }
                }
            });
        }

        private async void SendAcourateVolumeDownLarge(double numberOfKeysToSend)
        {
            await Task.Run(() =>
            {
                for (double counter = 0; counter < numberOfKeysToSend; counter++)
                {
                    try
                    {
                        System.Windows.Forms.SendKeys.SendWait("^%{DOWN}");
                    }
                    catch (Exception) { }
                }
            });
        }

        private async void SendAcourateVolumeUpSmall(double numberOfKeysToSend)
        {
            await Task.Run(() =>
            {
                for (double counter = 0; counter < numberOfKeysToSend; counter++)
                {
                    try
                    {
                        System.Windows.Forms.SendKeys.SendWait("%{UP}");
                    }
                    catch (Exception) { }
                }
            });
        }

        private async void SendAcourateVolumeUpLarge(double numberOfKeysToSend)
        {
            await Task.Run(() =>
            {

                for (double counter = 0; counter < numberOfKeysToSend; counter++)
                {
                    try
                    {
                        System.Windows.Forms.SendKeys.SendWait("^%{UP}");
                    }
                    catch (Exception) { }
                }
            });
        }


        private void btnLoop_Click(object sender, RoutedEventArgs e)
        {
            if (!isPlaying)
                return;
            if (!isLooping && !isLoopStarting) 
            {
                loopStart = musicEngine.GetCurrentTrackTimeInSeconds();
                var template = btnLoop.Template;
                var imageControl = (Image)template.FindName("imageLoop", btnLoop);
                var uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnSetLoopSmall.png", UriKind.Relative);
                imageControl.Source = new BitmapImage(uriSource);
                isLoopStarting = true;
            } 
            else if (isLoopStarting) 
            { //this button press means we end looping here
                loopEnd = musicEngine.GetCurrentTrackTimeInSeconds();
                var template = btnLoop.Template;
                var imageControl = (Image)template.FindName("imageLoop", btnLoop);
                var uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnLoopingSmall.png", UriKind.Relative);
                imageControl.Source = new BitmapImage(uriSource);
                musicEngine.MoveToPositionInSeconds(loopStart);
                isLooping = true;
                isLoopStarting = false;
            }
            else 
            { //stop looping
                resetLooping();
            }
        }

        public void resetLooping() {
            loopStart = 0;
            loopEnd = 0;
            isLooping = false;    
            isLoopStarting = false;
            var template = btnLoop.Template;
            var imageControl = (Image)template.FindName("imageLoop", btnLoop);
            var uriSource = new Uri(@"/WhisperingAudioMusicPlayer;component/images/btnLoopSmall.png", UriKind.Relative);
            imageControl.Source = new BitmapImage(uriSource);
        }

        private void sldrVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (musicEngine != null && isAcourateVolumeEnabled == false)
            {
                float volume;
                double dB = (e.NewValue - 601) / 10;
                if (dB < -60)
                    volume = 0;
                else
                    volume = (float)Math.Pow(10, dB / 20);
                musicEngine.Volume = volume;
                //musicEngine.Volume = (float)Math.Pow(10, dB / 20);
                SetVolumeLabel(e.NewValue);
            }
            else
            {
                SetVolumeLabel(e.NewValue);
            }

            //SetVolume();
        }

        private void sldrVolume_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            double volumeValue = Properties.Settings.Default.CurrentVolume;
            double changeValue = sldrVolume.Value;

            if (isAcourateVolumeEnabled)
            {
                if (changeValue > volumeValue)
                {
                    int difference = (int)(changeValue - volumeValue);
                    if (difference < 10)
                        SendAcourateVolumeUpSmall(difference);
                    else
                    {
                        int largeChange = difference / 10;
                        int smallchange = difference % 10;
                        SendAcourateVolumeUpLarge(largeChange);
                        SendAcourateVolumeUpSmall(smallchange);
                    }
                }
                else
                {
                    int difference = (int)(volumeValue - changeValue);
                    if (difference < 10)
                        SendAcourateVolumeDownSmall(difference);
                    else
                    {
                        int largeChange = difference / 10;
                        int smallchange = difference % 10;
                        SendAcourateVolumeDownLarge(largeChange);
                        SendAcourateVolumeDownSmall(smallchange);
                    }
                }
            }

            Properties.Settings.Default.CurrentVolume = changeValue;
            Properties.Settings.Default.Save();
        }

        //private void SetVolume()
        //{
        //    double volumeValue = Properties.Settings.Default.CurrentVolume;
        //    double changeValue = sldrVolume.Value;

        //    if (musicEngine != null && isAcourateVolumeEnabled == false)
        //    {
        //        float volume;
        //        double dB = (changeValue - 601) / 10;
        //        if (dB < -60)
        //            volume = 0;
        //        else
        //            volume = (float)Math.Pow(10, dB / 20);
        //        musicEngine.Volume = volume;
        //    }

        //    else if (isAcourateVolumeEnabled)
        //    {
        //        if (changeValue > volumeValue)
        //        {
        //            int difference = (int)(changeValue - volumeValue);
        //            if (difference < 10)
        //                SendAcourateVolumeUpSmall(difference);
        //            else
        //            {
        //                int largeChange = difference / 10;
        //                int smallchange = difference % 10;
        //                SendAcourateVolumeUpLarge(largeChange);
        //                SendAcourateVolumeUpSmall(smallchange);
        //            }
        //        }
        //        else
        //        {
        //            int difference = (int)(volumeValue - changeValue);
        //            if (difference < 10)
        //                SendAcourateVolumeDownSmall(difference);
        //            else
        //            {
        //                int largeChange = difference / 10;
        //                int smallchange = difference % 10;
        //                SendAcourateVolumeDownLarge(largeChange);
        //                SendAcourateVolumeDownSmall(smallchange);
        //            }
        //        }
        //    }

        //    Properties.Settings.Default.CurrentVolume = changeValue;
        //    SetVolumeLabel(changeValue);
        //}
    }
}
