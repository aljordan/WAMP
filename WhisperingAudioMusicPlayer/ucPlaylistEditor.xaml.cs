using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WhisperingAudioMusicLibrary;

namespace WhisperingAudioMusicPlayer
{
    public delegate void PlaylistContentsChangedEventHandler(object sender, PlaylistEventArgs eArgs);


    /// <summary>
    /// Interaction logic for ucPlaylistEditor.xaml
    /// </summary>
    public partial class ucPlaylistEditor : UserControl
    {
        public event PlaylistContentsChangedEventHandler PlaylistContentsChangedEvent;
        private MusicLibrary selectedLibrary;
        private Playlist currentPlaylist;
        private ObservableCollection<Track> observablePlaylist; // used for drag and drop reordering
        private bool sendPlaylistChangesToPlayer;

        public ucPlaylistEditor()
        {
            InitializeComponent();
            rdoArtistSearch.IsChecked = true;
            observablePlaylist = new ObservableCollection<Track>();
            lstPlaylist.ItemsSource = observablePlaylist;

            // for drag and drop reorder of playlist
            Style itemContainerStyle = new Style(typeof(ListBoxItem));
            itemContainerStyle.Setters.Add(new Setter(AllowDropProperty, true));
            itemContainerStyle.Setters.Add(new EventSetter(PreviewMouseMoveEvent, new MouseEventHandler(s_PreviewMouseMove)));
            itemContainerStyle.Setters.Add(new EventSetter(DropEvent, new DragEventHandler(lstPlaylist_Drop)));
            itemContainerStyle.Resources.Add(SystemColors.HighlightBrushKey, Brushes.Transparent);
            itemContainerStyle.Resources.Add(SystemColors.InactiveSelectionHighlightBrushKey, Brushes.Transparent);
            itemContainerStyle.Resources.Add(SystemColors.InactiveSelectionHighlightTextBrushKey, Brushes.White);
            lstPlaylist.ItemContainerStyle = itemContainerStyle;

            try
            {
                sendPlaylistChangesToPlayer = Properties.Settings.Default.SendPlaylistChangesToPlayer;
                chkSendPlaylistToPlayer.IsChecked = sendPlaylistChangesToPlayer;
            }
            catch (Exception)
            {
                sendPlaylistChangesToPlayer = false;
                chkSendPlaylistToPlayer.IsChecked = false;
            }
        }

        // for drag and drop reorder of playlist
        void s_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                if (draggedItem != null)
                {
                    DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                    draggedItem.IsSelected = true;
                }
            }
        }

        //for drag and drop reorder of playlist
        void lstPlaylist_Drop(object sender, DragEventArgs e)
        {
            Track droppedData = e.Data.GetData(typeof(Track)) as Track;
            Track target = ((ListBoxItem)(sender)).DataContext as Track;

            int removedIdx = lstPlaylist.Items.IndexOf(droppedData);
            int targetIdx = lstPlaylist.Items.IndexOf(target);

            if (removedIdx < targetIdx)
            {
                observablePlaylist.Insert(targetIdx + 1, droppedData);
                observablePlaylist.RemoveAt(removedIdx);
            }
            else
            {
                int remIdx = removedIdx + 1;
                if (observablePlaylist.Count + 1 > remIdx)
                {
                    observablePlaylist.Insert(targetIdx, droppedData);
                    observablePlaylist.RemoveAt(remIdx);
                }
            }

            SendPlaylistToPlayer();
        }


        public MusicLibrary SelectedLibrary
        {
            get { return selectedLibrary; }
            set 
            { 
                selectedLibrary = value;
                rdoArtistSearch.IsChecked = true;
                PopulateGenres();
                PopulateArtists();
                lstAlbums.ItemsSource = null;
                lstSongs.ItemsSource = null;
                
            }
        }

        public Playlist CurrentPlaylist
        {
            get { return currentPlaylist; }
            set
            {
                currentPlaylist = value;
                foreach (Track song in currentPlaylist)
                    observablePlaylist.Add(song);
                lstPlaylist.Items.Refresh();
            }
        }

        private void PopulateGenres()
        {
            if (selectedLibrary != null)
            {
                List<string> genres = selectedLibrary.GetGenres();
                lstGenres.ItemsSource = genres;
                lstGenres.Items.Refresh();
            }
        }

        private void PopulateArtists()
        {
            if (selectedLibrary != null)
            {
                List<string> artists = selectedLibrary.GetArtists();
                lstArtists.ItemsSource = artists;
                lstArtists.Items.Refresh();
            }
        }

        private void lstArtists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstArtists.SelectedIndex == -1)
                return;
            List<string> albums = selectedLibrary.GetAlbumsByArtist((string)lstArtists.SelectedItem);
            lstAlbums.ItemsSource = albums;
            lstAlbums.Items.Refresh();

            lstSongs.ItemsSource = null;
            lstSongs.Items.Refresh();

            // if only one album automatically select it to show songs
            if (lstAlbums.Items.Count == 1)
                lstAlbums.SelectedIndex = 0;
        }

        private void lstAlbums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstAlbums.SelectedIndex == -1)
                return;

            List<Track> songs = selectedLibrary.GetSongsByAlbum((string)lstAlbums.SelectedItem);
            try
            {
                lstSongs.Items.Clear(); // sometimes causes an exception, but is needed in certain circumstances
            }
            catch (Exception) { } // do nothing 
            lstSongs.ItemsSource = null;
            lstSongs.ItemsSource = songs;
            lstSongs.Items.Refresh();
        }

        private void lstAlbums_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstAlbums.SelectedIndex == -1)
                return;

            foreach (object o in lstSongs.Items)
                observablePlaylist.Add((Track)o);

            lstPlaylist.Items.Refresh();
            SendPlaylistToPlayer();
        }

        private void lstSongs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstSongs.SelectedIndex == -1)
                return;

            foreach (object o in lstSongs.SelectedItems)
                observablePlaylist.Add((Track)o);

            lstPlaylist.Items.Refresh();
            SendPlaylistToPlayer();
        }

        private void btnAddSongs_Click(object sender, RoutedEventArgs e)
        {
            if (lstSongs.SelectedIndex == -1)
                return;

            foreach (object o in lstSongs.SelectedItems)
                observablePlaylist.Add((Track)o);

            lstPlaylist.Items.Refresh();
            SendPlaylistToPlayer();
        }

        private void btnClearPlaylist_Click(object sender, RoutedEventArgs e)
        {
            observablePlaylist.Clear();
            lstPlaylist.Items.Refresh();
            SendPlaylistToPlayer();
        }

        private void btnRemoveFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            List<Track> selectedItems = new List<Track>();
            foreach (Track t in lstPlaylist.SelectedItems)
                selectedItems.Add(t);

            foreach (Track t in selectedItems)
                observablePlaylist.Remove(t);

            lstPlaylist.Items.Refresh();
            SendPlaylistToPlayer();
        }

        private void SendPlaylistToPlayer()
        {
            if (sendPlaylistChangesToPlayer)
            {
                List<Track> list = new List<Track>();
                foreach (Track t in lstPlaylist.Items)
                {
                    list.Add(t);
                }
                Playlist p = new Playlist("default", list);
                //p.SavePlaylist();
                OnRaisePlaylistContentsChangedEvent(new PlaylistEventArgs(p));
            }
        }


        protected virtual void OnRaisePlaylistContentsChangedEvent(PlaylistEventArgs pea)
        {
            PlaylistContentsChangedEventHandler handler = PlaylistContentsChangedEvent;
            // Raise the event
            if (handler != null)
                handler(this, pea);
        }


        private void btnLoadPlaylist_Click(object sender, RoutedEventArgs e)
        {
            SelectPlaylistDialog dlg = new SelectPlaylistDialog();
            dlg.Owner = Window.GetWindow(this);
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                Playlist pl = dlg.SelectedPlaylist;
                currentPlaylist = pl;

                observablePlaylist.Clear();
                foreach (Track t in pl.GetSongs())
                    observablePlaylist.Add(t);

                lstPlaylist.Items.Refresh();
                SendPlaylistToPlayer();
            }
        }

        private void btnSavePlaylist_Click(object sender, RoutedEventArgs e)
        {
            string initialName;
            if (currentPlaylist != null)
                initialName = currentPlaylist.Name;
            else
                initialName = "";

            SavePlaylistDialog dlg = new SavePlaylistDialog(initialName);
            dlg.Owner = Window.GetWindow(this);
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                List<Track> list = new List<Track>();
                foreach (Track t in lstPlaylist.Items)
                {
                    list.Add(t);
                }
                Playlist p = new Playlist(dlg.SelectedPlaylistName, list);
                p.SavePlaylist();
                currentPlaylist = p;
            }
        }


        private void lstGenres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstGenres.SelectedIndex == -1)
            {
                PopulateArtists();
                return;
            }

            List<string> artists = selectedLibrary.GetArtistsByGenre(lstGenres.SelectedItem.ToString());
            
            lstArtists.ItemsSource = artists;
            lstArtists.Items.Refresh();

            lstAlbums.ItemsSource = null;
            lstAlbums.Items.Refresh();

            lstSongs.ItemsSource = null;
            lstSongs.Items.Refresh();

            //if only one artist in genre, select it automatically
            if (lstArtists.Items.Count == 1)
                lstArtists.SelectedIndex = 0;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (selectedLibrary != null)
            {
                // first clear all lists -- I changed this to only clear what is being searched
                //lstArtists.ItemsSource = null;
                //lstAlbums.ItemsSource = null;
                //lstSongs.ItemsSource = null;

                if ((bool)rdoArtistSearch.IsChecked)
                {
                    lstArtists.ItemsSource = null;
                    List<string> artists = selectedLibrary.GetArtists(txtSearchString.Text);
                    lstArtists.ItemsSource = artists;
                    lstArtists.Items.Refresh();
                }
                else if ((bool)rdoAlbumSearch.IsChecked)
                {
                    lstAlbums.ItemsSource = null;
                    List<string> albums = selectedLibrary.GetAlbums(txtSearchString.Text);
                    lstAlbums.ItemsSource = albums;
                    lstAlbums.Items.Refresh();
                }
                else if ((bool)rdoSongSearch.IsChecked)
                {
                    lstSongs.ItemsSource = null;
                    List<Track> songs = selectedLibrary.GetSongs(txtSearchString.Text);
                    lstSongs.Items.Clear();
                    foreach (Track song in songs)
                        lstSongs.Items.Add(song);
                }
            }
        }

        private void btnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearchString.Text = string.Empty;
            PopulateArtists();
        }

        private void txtSearchString_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // press the search button
                btnSearch.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));

                //// Or - A hell of lot just to press a button.  Thanks Obama!
                //ButtonAutomationPeer peer = new ButtonAutomationPeer(btnSearch);
                //IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                //invokeProv.Invoke();
            }
        }

        private void chkSendPlaylistToPlayer_Checked(object sender, RoutedEventArgs e)
        {
            sendPlaylistChangesToPlayer = (bool)chkSendPlaylistToPlayer.IsChecked;
            Properties.Settings.Default.Save();

            SendPlaylistToPlayer();
        }

    }
}
