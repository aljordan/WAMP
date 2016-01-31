using System;
using System.IO;
using System.Windows;
using WhisperingAudioMusicLibrary;
using WhisperingAudioMusicEngine;

namespace WhisperingAudioMusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MusicLibrary selectedLibrary;
        private Playlist currentPlaylist;
        private AudioOutput selectedOutput;

        public MainWindow()
        {
            // In order to log exception
            AppDomain currentDomain = default(AppDomain);
            currentDomain = AppDomain.CurrentDomain;
            // Handler for unhandled exceptions.
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            // Handler for exceptions in threads behind forms.
            System.Windows.Forms.Application.ThreadException += GlobalThreadExceptionHandler;

            // make sure our directory where we will save and load playlists exists
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\wamp"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\wamp");

            //log file


            InitializeComponent();

            Closing += MainWindow_Closing;


            ////Subscribe to PlaylistEditor control PlayPlaylist button event
            //playlistEditorControl.PlayPlaylistButtonPressedEvent += HandlePlayPlaylistButtonPressedEvent;

            //Subscribe to PlaylistEditor control Playlist Changed event
            playlistEditorControl.PlaylistContentsChangedEvent += HandlePlaylistContentsChangedEvent;

            // Subscribe to the selected library changed event in the preferences control.  Note that a name 
            // was given to the control in the main window xaml.
            preferencesControl.SelectedLibraryChangedEvent += HandleSelectedLibraryChangedEvent;            
            playlistEditorControl.SelectedLibrary = preferencesControl.SelectedLibrary;

            //Subscribe to Preferences control SelectedOutputchanged event
            preferencesControl.SelectedOutputChangedEvent += HandleSelectedOutputChangedEvent;
            // and the VolumeEnabled event
            preferencesControl.VolumeEnabledEvent += HandleVolumeEnableEvent;
            // and the AcourateVolumeEnabled event
            preferencesControl.AcourateVolumeEnabledEvent += HandleAcourateVolumeEnableEvent;
            // and the MemoryPlayEnabledEvent
            preferencesControl.MemoryPlayEnabledEvent += HandleMemoryPlayEnableEvent;
            // and the NetworkControlEnabledEvent
            preferencesControl.NetworkControlEnabledEvent += HandleNetworkControlEnableEvent;

            // Try to load the default playlist if it exists
            currentPlaylist = Playlist.OpenPlaylist("default");
            if (currentPlaylist != null)
            {
                playerControl.CurrentPlaylist = currentPlaylist;
                playlistEditorControl.CurrentPlaylist = currentPlaylist;
                Track currentTrack = Properties.Settings.Default.CurrentTrack;
                if (currentTrack != null)
                    playerControl.CurrentTrack = currentTrack;
            }

            if (preferencesControl.SelectedOutput != null)
                playerControl.SelectedOutput = preferencesControl.SelectedOutput;

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new Action(() => { playerControl.InitButtons(); }));
        }


        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            playerControl.Cleanup();
        }


        void HandleSelectedLibraryChangedEvent(object sender, SelectedLibraryChangedEventArgs slcea)
        {
            selectedLibrary = slcea.SelectedLibrary;
            playlistEditorControl.SelectedLibrary = selectedLibrary;
        }


        void HandleSelectedOutputChangedEvent(object sender, SelectedOutputChangedEventArgs socea)
        {
            selectedOutput = socea.SelectedOutput;
            playerControl.SelectedOutput = selectedOutput;
        }

        void HandlePlaylistContentsChangedEvent(object sender, PlaylistEventArgs pea)
        {
            currentPlaylist = pea.SelectedPlaylist;
            playerControl.CurrentPlaylist = currentPlaylist;
        }



        void HandleVolumeEnableEvent(object sender, VolumeEnabledEventArgs veea)
        {
            playerControl.IsVolumeEnabled = veea.IsVolumeEnabled;
        }

        void HandleAcourateVolumeEnableEvent(object sender, VolumeEnabledEventArgs veea)
        {
            playerControl.IsAcourateVolumeEnabled = veea.IsVolumeEnabled;
        }

        void HandleNetworkControlEnableEvent(object sender, NetworkControlEnabledEventArgs nceea)
        {
            playerControl.IsNetworkControlEnabled = nceea.IsNetworkControlEnabled;
        }

        void HandleMemoryPlayEnableEvent(object sender, MemoryPlayEnabledEventArgs mpeea)
        {
            playerControl.IsMemoryPlayEnabled = mpeea.IsMemoryPlayEnabled;
        }

        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            string logFileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\wamp\\exception.log";
            Exception ex = default(Exception);
            ex = (Exception)e.ExceptionObject;
            using (StreamWriter w = File.AppendText(logFileName))
            {
                Log(ex.Message + "\n" + ex.StackTrace, w);
            }
            MusicEngine.Cleanup();
        }

        private static void GlobalThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string logFileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\wamp\\exception.log";
            Exception ex = default(Exception);
            ex = e.Exception;
            using (StreamWriter w = File.AppendText(logFileName))
            {
                Log(ex.Message + "\n" + ex.StackTrace, w);
            }
            MusicEngine.Cleanup();
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
        }


        AudioOutput SelectedOutput
        {
            get { return selectedOutput; }
            set { selectedOutput = value; }
        }

    }
}
