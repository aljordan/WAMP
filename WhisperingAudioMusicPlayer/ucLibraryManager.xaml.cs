using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WhisperingAudioMusicLibrary;
using System.Threading;
using System.Windows.Threading;

namespace WhisperingAudioMusicPlayer
{
    public delegate void SelectedLibraryChangedEventHandler(object sender, SelectedLibraryChangedEventArgs e);

    /// <summary>
    /// Interaction logic for ucLibraryManager.xaml
    /// </summary>
    public partial class ucLibraryManager : UserControl
    {
        public event SelectedLibraryChangedEventHandler SelectedLibraryChangedEvent;
        private List<MusicLibrary> libraries;
        private MusicLibrary selectedLibrary;

        public ucLibraryManager()
        {
            InitializeComponent();
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
        }

        public MusicLibrary SelectedLibrary
        {
            get { return selectedLibrary;  }
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
            this.Dispatcher.BeginInvoke(new Action(() => 
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

        private void btnUpdateLibrary_Click(object sender, RoutedEventArgs e)
        {
            selectedLibrary.StatusChangedEvent += HandleStatusChangedEvent;

            Thread MyNewThread = new Thread(new ThreadStart(() =>
            {
                selectedLibrary.UpdateLibrary();
            }));
            MyNewThread.Start();
        }



    }
}
