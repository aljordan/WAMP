using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WhisperingAudioMusicLibrary;

namespace WhisperingAudioMusicPlayer
{
    /// <summary>
    /// Interaction logic for SelectPlaylistDialog.xaml
    /// </summary>
    public partial class SelectPlaylistDialog : Window
    {
        private Playlist selectedPlaylist;

        public SelectPlaylistDialog()
        {
            InitializeComponent();

            List<Playlist> playlists = Playlist.GetAvailablePlaylists();
            foreach (Playlist pl in playlists)
                lstPlaylists.Items.Add(pl);

            if (lstPlaylists.Items.Count == 1)
                lstPlaylists.SelectedItem = 0;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (lstPlaylists.SelectedItem != null)
                DialogResult = true;
        }

        public Playlist SelectedPlaylist
        {
            get { return selectedPlaylist; }
        }

        private void lstPlaylists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPlaylist = (Playlist)lstPlaylists.SelectedItem;
        }

        private void lstPlaylists_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            selectedPlaylist = (Playlist)lstPlaylists.SelectedItem;
            if (lstPlaylists.SelectedItem != null)
                btnOk.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
        }
    }
}
