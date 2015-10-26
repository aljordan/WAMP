using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WhisperingAudioMusicEngine;
using WhisperingAudioMusicLibrary;

namespace WhisperingAudioMusicPlayer
{
    public delegate void SelectedOutputChangedEventHandler(object sender, SelectedOutputChangedEventArgs eArgs);
    public delegate void PlaybackTimeoutChangedEventHandler(object sender, PlaybackTimeoutChangedEventArgs eArgs);
    public delegate void SelectedLibraryChangedEventHandler(object sender, SelectedLibraryChangedEventArgs e);
    public delegate void VolumeEnabledEventHandler(object sender, VolumeEnabledEventArgs e);
    public delegate void AcourateVolumeEnabledEventHandler(object sender, VolumeEnabledEventArgs e);
    public delegate void MemoryPlayEnabledEventHandler(object sender, MemoryPlayEnabledEventArgs e);

    /// <summary>
    /// Interaction logic for ucPreferences.xaml
    /// </summary>
    public partial class ucPreferences : UserControl
    {
        public event SelectedOutputChangedEventHandler SelectedOutputChangedEvent;
        public event PlaybackTimeoutChangedEventHandler PlaybackTimeoutChangedEvent;
        private AudioOutput currentOutput;
        public event SelectedLibraryChangedEventHandler SelectedLibraryChangedEvent;
        public event VolumeEnabledEventHandler VolumeEnabledEvent;
        public event AcourateVolumeEnabledEventHandler AcourateVolumeEnabledEvent;
        public event MemoryPlayEnabledEventHandler MemoryPlayEnabledEvent;
        private List<MusicLibrary> libraries;
        private MusicLibrary selectedLibrary;


        public ucPreferences()
        {
            InitializeComponent();
            sldrDeviceLatency.IsEnabled = false;
            PopulateDeviceList();
            try
            {
                AudioOutput savedOutput = Properties.Settings.Default.SelectedOutput;
                foreach (AudioOutput o in lstAudioDevices.Items)
                {
                    if (o.DeviceName == savedOutput.DeviceName && o.DeviceType == savedOutput.DeviceType)
                    {
                        o.ChosenLatency = savedOutput.ChosenLatency;
                        lstAudioDevices.SelectedItem = o;
                    }
                }
            }
            catch (Exception)
            {
                lstAudioDevices.SelectedIndex = -1;
            }

            //try
            //{
            //    int playbackTimeout = Properties.Settings.Default.PlaybackTimeout;
            //    sldrSwitchTracks.Value = playbackTimeout;
            //    if (playbackTimeout == 0)
            //        lblSwitchTracksContent.Content = "0 milliseconds";
            //}
            //catch (Exception)
            //{
            //    sldrSwitchTracks.Value = 0;
            //    lblSwitchTracksContent.Content = "0 milliseconds";
            //}

            libraries = MusicLibrary.GetAvailableLibraries();
            lstLibraries.ItemsSource = libraries;
            try
            {
                lstLibraries.SelectedItem = libraries.Find(x => x.LibraryName == Properties.Settings.Default.SelectedLibraryName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not load selected library: " + e.Message);
            }

            try
            {
                chkVolumeEnabled.IsChecked = Properties.Settings.Default.IsVolumeEnabled;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not get volume enabled state: " + e.Message);
            }

            //AcourateAsio volume
            try
            {
                if (IsAcourateAsioVolumeAllowed())
                {
                    chkAcourateAsioVolume.IsEnabled = true;
                    chkAcourateAsioVolume.IsChecked = Properties.Settings.Default.IsAcourateVolumeEnabled;
                }
                else
                {
                    chkAcourateAsioVolume.IsChecked = false;
                    chkAcourateAsioVolume.IsEnabled = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not get AcourateAsio volume enabled state: " + e.Message);
            }


            try
            {
                chkMemoryPlayEnabled.IsChecked = Properties.Settings.Default.IsMemoryPlayEnabled;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not get memory play enabled state: " + e.Message);
            }
        }

        public AudioOutput SelectedOutput
        {
            get { return currentOutput; }
        }

        private void PopulateDeviceList()
        {
            foreach (AudioOutput output in AudioOutputs.GetDeviceList())
            {
                lstAudioDevices.Items.Add(output);
            }

        }

        private bool IsAcourateAsioVolumeAllowed()
        {
            if (lstAudioDevices.SelectedIndex != -1)
            {
                AudioOutput selectedOutput = (AudioOutput)lstAudioDevices.Items.GetItemAt(lstAudioDevices.SelectedIndex);
                if (selectedOutput.DeviceName.ToLower().Contains("acourateasio"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        
        private void lstAudioDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstAudioDevices.SelectedIndex == -1)
            {
                chkAcourateAsioVolume.IsChecked = false;
                chkAcourateAsioVolume.IsEnabled = false;
                sldrDeviceLatency.IsEnabled = false;
                return;
            }

            currentOutput = (AudioOutput)lstAudioDevices.SelectedItem;

            if (currentOutput.DeviceType == AudioOutputs.AudioDeviceType.Asio)
            {
                sldrDeviceLatency.IsEnabled = false;
                sldrDeviceLatency.Value = 25;
                lblChosenLatency.Content = "Not Applicable";
            }
            else
            {
                if (currentOutput.ChosenLatency > 0)
                    sldrDeviceLatency.Value = currentOutput.ChosenLatency;
                else
                    sldrDeviceLatency.Value = 100;

                sldrDeviceLatency.IsEnabled = true;
            }

            SaveSelectedDeviceToSettings();
            OnRaiseSelectedOutputChangedEvent(new SelectedOutputChangedEventArgs(currentOutput));

            if (IsAcourateAsioVolumeAllowed())
            {
                chkAcourateAsioVolume.IsEnabled = true;
            }
            else
            {
                chkAcourateAsioVolume.IsChecked = false;
                chkAcourateAsioVolume.IsEnabled = false;
            }
        }

        private void SaveSelectedDeviceToSettings()
        {
            Properties.Settings.Default.SelectedOutput = currentOutput;
            Properties.Settings.Default.Save();
        }


        protected virtual void OnRaiseSelectedOutputChangedEvent(SelectedOutputChangedEventArgs socea)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            SelectedOutputChangedEventHandler handler = SelectedOutputChangedEvent;
            // Raise the event
            if (handler != null)
                handler(this, socea);
        }

        protected virtual void OnRaisePlaybackTimeoutChangedEvent(PlaybackTimeoutChangedEventArgs ptcea)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            PlaybackTimeoutChangedEventHandler handler = PlaybackTimeoutChangedEvent;
            // Raise the event
            if (handler != null)
                handler(this, ptcea);
        }

        private void sldrDeviceLatency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (lstAudioDevices.SelectedIndex == -1)
                return;

            ((AudioOutput)lstAudioDevices.SelectedItem).ChosenLatency = (int)sldrDeviceLatency.Value;
            lblChosenLatency.Content = sldrDeviceLatency.Value + " milliseconds";
            SaveSelectedDeviceToSettings();
            OnRaiseSelectedOutputChangedEvent(new SelectedOutputChangedEventArgs(currentOutput));
        }

        //private void sldrSwitchTracks_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    int playbackTimeout = (int)((Slider)sender).Value;
        //    if (playbackTimeout * 50 < 1000)
        //        lblSwitchTracksContent.Content = playbackTimeout * 50 + " milliseconds";
        //    else
        //        lblSwitchTracksContent.Content = (playbackTimeout * 50) / 1000.0 + " seconds";

        //    Properties.Settings.Default.PlaybackTimeout = playbackTimeout;
        //    // saving instead at window close
        //    //Properties.Settings.Default.Save(); 

        //    OnRaisePlaybackTimeoutChangedEvent(new PlaybackTimeoutChangedEventArgs(playbackTimeout));
        //}

        private void chkVolumeEnabled_Checked(object sender, RoutedEventArgs e)
        {
            bool enableVolume = (bool)((CheckBox)sender).IsChecked;
            if (enableVolume == false)
            {
                chkAcourateAsioVolume.IsChecked = false;
                chkAcourateAsioVolume.IsEnabled = false;
                Properties.Settings.Default.IsAcourateVolumeEnabled = false;
            }
            else
            {
                if (IsAcourateAsioVolumeAllowed())
                {
                    chkAcourateAsioVolume.IsEnabled = true;
                }
            }
            Properties.Settings.Default.IsVolumeEnabled = enableVolume;
            Properties.Settings.Default.Save();
            OnRaiseVolumeEnabledEvent(new VolumeEnabledEventArgs(enableVolume));
        }


        #region Library logic
        public MusicLibrary SelectedLibrary
        {
            get { return selectedLibrary; }
        }

        void btnAddLibrary_Click(object sender, RoutedEventArgs e)
        {
            string libraryRoot;
            string libraryName;

            InputDialog iDialog = new InputDialog("Please give this library a name: ", "");
            if (iDialog.ShowDialog() == true)
            {
                libraryName = iDialog.Answer;

                FolderBrowser fb = new FolderBrowser();
                fb.IncludeFiles = false;
                fb.Description = "Please select root folder for library";
                if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    libraryRoot = fb.SelectedPath;
                    MusicLibrary ml = new MusicLibrary(libraryName, libraryRoot);
                    ml.StatusChangedEvent += HandleStatusChangedEvent;

                    Thread MyNewThread = new Thread(new ThreadStart(() =>
                    {
                        ml.CreateLibrary();

                    }));
                    MyNewThread.Start();

                    libraries.Add(ml);
                    lstLibraries.Items.Refresh();
                    lstLibraries.SelectedItem = ml;
                    selectedLibrary = ml;
                }
            }
        }


        void HandleStatusChangedEvent(object sender, StatusChangedEventArgs e)
        {
            //Console.WriteLine("Status: " + e.Message);
            //Have to do this because no other thread can update a UI element except the main thread.
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblStatusUpdate.Content = e.Message;
                // unselect and selected the selected library to fire a library change event and get it updated in the rest of the application 
                if (e.Message.Equals("Finished updating music library.") || e.Message.Equals("Finished processing music library."))
                {
                    int selectedIndex = lstLibraries.SelectedIndex;
                    lstLibraries.SelectedIndex = -1;
                    lstLibraries.SelectedIndex = selectedIndex;
                }
            }));
        }

        private void btnRemoveLibrary_Click(object sender, RoutedEventArgs e)
        {
            string sMessageBoxText = "Are you sure you want to delete the library " + selectedLibrary.LibraryName + "?";
            string sCaption = "Delete Library";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    MusicLibrary.DeleteLibrary(selectedLibrary.LibraryName);
                    libraries.Remove((MusicLibrary)lstLibraries.SelectedItem);
                    lstLibraries.Items.Refresh();
                    if (lstLibraries.Items.Count > 0)
                    {
                        lstLibraries.SelectedItem = lstLibraries.Items[0];
                        selectedLibrary = (MusicLibrary)lstLibraries.SelectedItem;
                    }
                    else
                        selectedLibrary = null;
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }

            }
        }


        private void lstLibraries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstLibraries.SelectedIndex == -1)
                return;
            selectedLibrary = (MusicLibrary)lstLibraries.SelectedItem;
            OnRaiseSelectedLibraryChangedEvent(new SelectedLibraryChangedEventArgs(selectedLibrary));
            Properties.Settings.Default.SelectedLibraryName = selectedLibrary.LibraryName;
            Properties.Settings.Default.Save();
        }


        protected virtual void OnRaiseSelectedLibraryChangedEvent(SelectedLibraryChangedEventArgs slcea)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            SelectedLibraryChangedEventHandler handler = SelectedLibraryChangedEvent;
            // Raise the event
            if (handler != null)
                handler(this, slcea);
        }

        protected virtual void OnRaiseVolumeEnabledEvent(VolumeEnabledEventArgs veea)
        {
            VolumeEnabledEventHandler handler = VolumeEnabledEvent;
            // Raise the event
            if (handler != null)
                handler(this, veea);
        }

        protected virtual void OnRaiseAcourateVolumeEnabledEvent(VolumeEnabledEventArgs veea)
        {
            AcourateVolumeEnabledEventHandler handler = AcourateVolumeEnabledEvent;
            // Raise the event
            if (handler != null)
                handler(this, veea);
        }

        protected virtual void OnRaiseMemoryPlayEnabledEvent(MemoryPlayEnabledEventArgs mpeea)
        {
            MemoryPlayEnabledEventHandler handler = MemoryPlayEnabledEvent;
            // Raise the event
            if (handler != null)
                handler(this, mpeea);
        }

        private void btnUpdateLibrary_Click(object sender, RoutedEventArgs e)
        {
            selectedLibrary.StatusChangedEvent += HandleStatusChangedEvent;

            Thread MyNewThread = new Thread(new ThreadStart(() =>
            {
                selectedLibrary.UpdateLibrary();
            }));
            MyNewThread.Start();
        }
        #endregion

        private void chkMemoryPlayEnabled_Checked(object sender, RoutedEventArgs e)
        {
            bool enableMemoryPlay = (bool)((CheckBox)sender).IsChecked;
            Properties.Settings.Default.IsMemoryPlayEnabled = enableMemoryPlay;
            Properties.Settings.Default.Save();
            OnRaiseMemoryPlayEnabledEvent(new MemoryPlayEnabledEventArgs(enableMemoryPlay));
        }

        private void chkAcourateAsioVolume_Checked(object sender, RoutedEventArgs e)
        {
            bool enableAcourateVolume = (bool)((CheckBox)sender).IsChecked;
            Properties.Settings.Default.IsAcourateVolumeEnabled = enableAcourateVolume;
            Properties.Settings.Default.Save();
            OnRaiseAcourateVolumeEnabledEvent(new VolumeEnabledEventArgs(enableAcourateVolume));
        }


    }
}
