using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WhisperingAudioMusicLibrary;

namespace WhisperingAudioMusicPlayer
{
    /// <summary>
    /// Interaction logic for SelectPlaylistDialog.xaml
    /// </summary>
    public partial class SavePlaylistDialog : Window
    {
        private string selectedPlaylistName;

        public SavePlaylistDialog()
        {
            InitializeComponent();

            List<Playlist> playlists = Playlist.GetAvailablePlaylists();
            foreach (Playlist pl in playlists)
                lstPlaylists.Items.Add(pl);

            if (lstPlaylists.Items.Count == 1)
                lstPlaylists.SelectedItem = 0;
        }

        public SavePlaylistDialog(string initialPlaylistName)
        {
            InitializeComponent();

            List<Playlist> playlists = Playlist.GetAvailablePlaylists();
            foreach (Playlist pl in playlists)
                lstPlaylists.Items.Add(pl);

            if (lstPlaylists.Items.Count == 1)
                lstPlaylists.SelectedItem = 0;

            txtPlaylistName.Text = initialPlaylistName;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txtPlaylistName.Text.Trim().Length > 0)
            {
                selectedPlaylistName = txtPlaylistName.Text;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("You have to give the playlist a name.");
            }
        }

        public string SelectedPlaylistName
        {
            get { return selectedPlaylistName; }
        }

        private void lstPlaylists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPlaylists.SelectedIndex != -1)
                txtPlaylistName.Text = ((Playlist)lstPlaylists.SelectedItem).Name;
            else
                txtPlaylistName.Text = "";
        }
    }
}
