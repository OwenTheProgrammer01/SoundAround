using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;

namespace SoundAround
{
    static class ExtensionsClass
    {
        private static readonly Random random = new Random();

        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]); // Tuple swap
            }
        }
    }

    /// <summary>
    /// Interaction logic for soundaround.xaml
    /// </summary>
    public partial class soundaround : Window
    {
        // MediaElement for playing the songs
        private MediaElement player = new MediaElement();

        // MediaPlayer for playing the songs
        private readonly MediaPlayer newPlayer = new MediaPlayer();

        // DispatcherTimer for updating the UI (e.g., song position)
        private readonly DispatcherTimer timer = new DispatcherTimer();

        // List of albums, artists, etc.
        private List<Album> albums = new List<Album>();
        private List<Artist> artists = new List<Artist>();
        private List<FileType> fileTypes = new List<FileType>();
        private List<Song> musicLibrary = new List<Song>();
        private List<Song> queue = new List<Song>();

        // currently playing song
        private Song currentSong = new Song();

        private enum Menu { Start, MusicLibrary, Queue }
        private enum Tab { Songs, Albums, Artists }

        private Menu currentMenu = Menu.Start;
        private Menu activePlaylist = Menu.MusicLibrary;
        private Tab currentTab = Tab.Songs;

        private int currentType = -1;
        private bool selectionChangeActive = true;
        private bool shuffle = false;
        private bool isPlaying = false;
        private bool repeat = false;
        private double volume = 100;
        private readonly string folder = $@"C:\Users\{Environment.UserName}\Music\SoundAround";

        public soundaround()
        {
            InitializeComponent();
            InitializeMediaPlayer();
            InitializeTimer();
            FetchDatabase();

            // create folder if it does not exist yet
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            if (btnShuffle.BorderThickness.Bottom == 1) shuffle = true;
            if (btnRepeat.BorderThickness.Bottom == 1)repeat = true;

            this.Closed += DeleteFilesOnClose;
        }

        private void InitializeMediaPlayer()
        {
            player.LoadedBehavior = MediaState.Manual;
            player.UnloadedBehavior = MediaState.Manual;
            player.Volume = volume / 100;
            player.MediaEnded += OnSongEnded;
        }
        private void InitializeTimer()
        {
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Update;
            timer.Start();
        }

        private void FetchDatabase()
        {
            try
            {
                // fetch the data from the database
                albums = AlbumDA.Fetch();
                artists = ArtistDA.Fetch();
                fileTypes = FileTypeDA.Fetch();
                musicLibrary = SongDA.Fetch();
                queue = SongDA.Fetch();

                // sort the lists alphabetically
                musicLibrary.Sort((x, y) => string.Compare(x.Name, y.Name));
                queue.Sort((x, y) => string.Compare(x.Name, y.Name));

                // shuffle the queue if shuffle is enabled
                if (shuffle) queue.Shuffle();

                UpdateUI();
            }
            catch (Exception error)
            {
                // error message
                MessageBox.Show(error.Message);
            }
        }

        private void Update(object sender, EventArgs e)
        {
            // update the current position of the song
            if (isPlaying)
            {
                lblCurrentPosition.Content = $"{player.Position.ToString(@"hh\:mm\:ss")}";
            }
        }

        private void UpdateUI()
        {
            try
            {
                selectionChangeActive = false;

                switch (currentMenu)
                {
                    case Menu.Start:
                        ShowStartMenu();
                        break;
                    case Menu.MusicLibrary:
                        ShowMusicLibraryMenu();
                        break;
                    case Menu.Queue:
                        ShowQueueMenu();
                        break;
                    default:
                        break;
                }

                lsbMusicLibrary.SelectedItem = currentSong.Name;
                lsbQueue.SelectedItem = currentSong.Name;
                lblSongName.Content = currentSong.Name;
                lblCurrentPosition.Content = $"{player.Position.ToString(@"hh\:mm\:ss")}";
                lblEndPosition.Content = player.NaturalDuration.HasTimeSpan ? player.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss") : "Unknown";
                selectionChangeActive = true;
            }
            catch (Exception error)
            {
                // error message
                MessageBox.Show(error.Message);
            }
        }

        private void ShowStartMenu()
        {
            SetVisibility(grdStart, true);
            SetVisibility(grdMuziekbibliotheek, false);
            SetVisibility(grdWachtrij, false);

            SetButtonBorders(btnStart, 1);
            SetButtonBorders(btnMuziekbibliotheek, 0);
            SetButtonBorders(btnWachtrij, 0);

            lsbStart.Items.Clear();
        }

        private void ShowMusicLibraryMenu()
        {
            SetVisibility(grdStart, false);
            SetVisibility(grdMuziekbibliotheek, true);
            SetVisibility(grdWachtrij, false);

            SetButtonBorders(btnStart, 0);
            SetButtonBorders(btnMuziekbibliotheek, 1);
            SetButtonBorders(btnWachtrij, 0);

            PopulateLibraryList();
        }

        private void ShowQueueMenu()
        {
            SetVisibility(grdStart, false);
            SetVisibility(grdMuziekbibliotheek, false);
            SetVisibility(grdWachtrij, true);

            SetButtonBorders(btnStart, 0);
            SetButtonBorders(btnMuziekbibliotheek, 0);
            SetButtonBorders(btnWachtrij, 1);

            lsbQueue.Items.Clear();
            foreach (Song song in queue)
            {
                lsbQueue.Items.Add(song.Name);
            }
        }

        private void SetVisibility(UIElement element, bool isVisible)
        {
            element.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        private void SetButtonBorders(Button button, int thickness)
        {
            button.BorderThickness = new Thickness(0, 0, 0, thickness);
        }

        private void PopulateLibraryList()
        {
            lsbMusicLibrary.Items.Clear();

            switch (currentTab)
            {
                case Tab.Songs:
                    SetTabBorders(btnNummers, 1, btnAlbums, btnArtiesten);
                    foreach (Song song in musicLibrary) lsbMusicLibrary.Items.Add(song.Name);
                    break;
                case Tab.Albums:
                    SetTabBorders(btnAlbums, 1, btnNummers, btnArtiesten);
                    foreach (Album album in albums) lsbMusicLibrary.Items.Add(album.album);
                    break;
                case Tab.Artists:
                    SetTabBorders(btnArtiesten, 1, btnNummers, btnAlbums);
                    foreach (Artist artist in artists) lsbMusicLibrary.Items.Add(artist.artist);
                    break;
            }

            if (activePlaylist == Menu.MusicLibrary) lsbMusicLibrary.SelectedItem = currentSong.Name;
        }

        private void SetTabBorders(Button activeTab, int activeThickness, params Button[] otherTabs)
        {
            activeTab.BorderThickness = new Thickness(0, 0, 0, activeThickness);
            foreach (var tab in otherTabs) tab.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            txbZoeken.Text = "Zoeken";
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            txbZoeken.Text = txbZoeken.Text.Replace("Zoeken", "");
        }

        private void PlaySong()
        {
            try
            {
                currentSong = activePlaylist switch
                {
                    Menu.MusicLibrary => musicLibrary[lsbMusicLibrary.SelectedIndex],
                    Menu.Queue => queue[lsbQueue.SelectedIndex],
                    _ => currentSong
                };

                //selectionChangeActive = false;

                player.Stop();
                currentType = fileTypes.FindIndex(f => f.FileType_ID == currentSong.FileType_ID);

                string filepath = $@"C:\Users\{Environment.UserName}\Music\SoundAround\{currentSong.Name} SoundAround{fileTypes[currentType].filetype}";

                if (player.Source == null || player.Source.AbsolutePath != filepath)
                {
                    File.WriteAllBytes(filepath, currentSong.SongFile);
                    player.Source = new Uri(filepath);
                }

                player.Play();
                isPlaying = true;
                SetButtonBorders(btnPause, 0);

                //selectionChangeActive = true;
            }
            catch (Exception error)
            {
                // error message
                MessageBox.Show(error.Message);
            }
        }

        private void OnSongEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (repeat) player.Position = TimeSpan.FromSeconds(0);
                else btnNext_Click(sender, e);
            }
            catch (Exception error)
            {
                // error message
                MessageBox.Show(error.Message);
            }
        }

        private void btnZoeken_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // set menu to start
            currentMenu = Menu.Start;
            UpdateUI();
        }

        private void btnMuziekbibliotheek_Click(object sender, RoutedEventArgs e)
        {
            // set menu to music library
            currentMenu = Menu.MusicLibrary;
            UpdateUI();
        }

        private void btnWachtrij_Click(object sender, RoutedEventArgs e)
        {
            // set menu to queue
            currentMenu = Menu.Queue;
            UpdateUI();
        }

        private void btnNummers_Click(object sender, RoutedEventArgs e)
        {
            // set tab to songs
            currentTab = Tab.Songs;
            UpdateUI();
        }

        private void btnAlbums_Click(object sender, RoutedEventArgs e)
        {
            // set tab to albums
            currentTab = Tab.Albums;
            UpdateUI();
        }

        private void btnArtiesten_Click(object sender, RoutedEventArgs e)
        {
            // set tab to artists
            currentTab = Tab.Artists;
            UpdateUI();
        }

        private void btnNummerToevoegen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.DefaultExt = "mp3";
                fileDialog.Filter = "mp3 files (*.mp3)|*.mp3|wav files (*.wav)|*.wav";
                fileDialog.Multiselect = true;
                fileDialog.Title = "Select a song";

                if (fileDialog.ShowDialog() == true)
                {
                    Song song = new Song();
                    FileType fileType = new FileType();
                    Artist artiest = new Artist();
                    Album album = new Album();

                    foreach (String file in fileDialog.FileNames)
                    {
                        FileStream fs = File.Open(file, FileMode.Open);
                        BinaryReader br = new BinaryReader(fs);

                        fileType.filetype = Path.GetExtension(file);

                        foreach (FileType _bestandtype in fileTypes)
                        {
                            if (_bestandtype.filetype == fileType.filetype)
                            {
                                song.FileType_ID = _bestandtype.FileType_ID;
                                goto skip;
                            }
                        }

                        FileTypeDA.Add(fileType);
                        fileTypes = FileTypeDA.Fetch();

                        foreach (FileType _bestandtype in fileTypes)
                        {
                            if (_bestandtype.filetype == fileType.filetype)
                            {
                                song.FileType_ID = _bestandtype.FileType_ID;
                            }
                        }

                    skip:
                        song.Artist_ID = 1;
                        song.Album_ID = 1;
                        song.SongFile = br.ReadBytes((int)fs.Length);
                        song.Name = Path.GetFileNameWithoutExtension(fs.Name);
                        //song.Duration = "23:59:59";
                        song.Duration = $"{player.NaturalDuration}";

                        if (!SongDA.Add(song))
                        {
                            MessageBox.Show("Failed to upload the song");
                        }

                    }
                }
                FetchDatabase();
            }
            catch (Exception error)
            {
                // error message
                MessageBox.Show(error.Message);
            }
        }

        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // shuffle the queue
                if (!shuffle)
                {
                    SetButtonBorders(btnShuffle, 1);
                    shuffle = true;
                    queue.Shuffle();
                }
                // sort the queue
                else
                {
                    SetButtonBorders(btnShuffle, 0);
                    shuffle = false;
                    queue.Sort((x, y) => string.Compare(x.Name, y.Name));
                }

                UpdateUI();
            }
            catch (Exception error)
            {
                // error message
                MessageBox.Show(error.Message);
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selectionChangeActive = false;
                if (activePlaylist == Menu.MusicLibrary)
                {
                    lsbMusicLibrary.SelectedItem = currentSong.Name;
                    // ga naar laatste item
                    if (lsbMusicLibrary.SelectedIndex == -1 || lsbMusicLibrary.SelectedIndex == 0)
                    {
                        lsbMusicLibrary.SelectedIndex = musicLibrary.Count - 1;
                    }
                    // vorig liedje
                    else
                    {
                        lsbMusicLibrary.SelectedIndex--;
                    }
                }
                if (activePlaylist == Menu.Queue)
                {
                    lsbQueue.SelectedItem = currentSong.Name;
                    // ga naar laatste item
                    if (lsbQueue.SelectedIndex == -1 || lsbQueue.SelectedIndex == 0)
                    {
                        lsbQueue.SelectedIndex = queue.Count - 1;
                    }
                    // vorig liedje
                    else
                    {
                        lsbQueue.SelectedIndex--;
                    }
                }
                selectionChangeActive = true;
                PlaySong();
            }
            catch (Exception error)
            {
                // error message
                MessageBox.Show(error.Message);
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // play
                if (!isPlaying)
                {
                    if (currentSong == null)
                    {
                        if (currentMenu == Menu.MusicLibrary)
                        {
                            selectionChangeActive = false;
                            lsbMusicLibrary.SelectedIndex = 0;
                            selectionChangeActive = true;
                            currentSong = musicLibrary[lsbMusicLibrary.SelectedIndex];
                        }

                        if (currentMenu == Menu.Queue)
                        {
                            selectionChangeActive = false;
                            lsbQueue.SelectedIndex = 0;
                            selectionChangeActive = true;
                            currentSong = queue[lsbQueue.SelectedIndex];
                        }
                        PlaySong();
                        SetButtonBorders(btnPause, 0);
                        return;
                    }
                    player.Play();
                    isPlaying = true;
                    SetButtonBorders(btnPause, 0);
                }
                // pause
                else
                {
                    player.Pause();
                    isPlaying = false;
                    SetButtonBorders(btnPause, 1);
                }
            }
            catch (Exception error)
            {
                // error message
                MessageBox.Show(error.Message);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selectionChangeActive = false;

                if (activePlaylist == Menu.MusicLibrary)
                {
                    lsbMusicLibrary.SelectedIndex = (lsbMusicLibrary.SelectedIndex + 1) % musicLibrary.Count;
                }
                else if (activePlaylist == Menu.Queue)
                {
                    lsbQueue.SelectedIndex = (lsbQueue.SelectedIndex + 1) % queue.Count;
                }

                selectionChangeActive = true;
                PlaySong();
            }
            catch (Exception error)
            {
                // error message
                MessageBox.Show(error.Message);
            }
        }

        private void btnRepeat_Click(object sender, RoutedEventArgs e)
        {
            // enable repeat
            if (!repeat)
            {
                repeat = true;
                SetButtonBorders(btnRepeat, 1);
            }
            // disable repeat
            else
            {
                repeat = false;
                SetButtonBorders(btnRepeat, 0);
            }
        }

        private void lsbMuziekbibliotheek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lsbMusicLibrary.SelectedIndex != -1 && currentTab == Tab.Songs && selectionChangeActive)
                {
                    activePlaylist = Menu.MusicLibrary;
                    PlaySong();
                }
            }
            catch (Exception error)
            {
                // error message
                MessageBox.Show(error.Message);
            }
        }

        private void btnNummerVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selectionChangeActive = false;
                SongDA.Delete(currentSong);
                selectionChangeActive = true;
                FetchDatabase();
            }
            catch (Exception error)
            {
                // error message
                MessageBox.Show(error.Message);
            }
        }

        private void sldVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            volume = Math.Round(sldVolume.Value, 0);
            lblVolume.Content = $"Volume: {volume}%";
            player.Volume = volume / 100;
        }

        private void lsbWachtrij_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lsbQueue.SelectedIndex != -1 && selectionChangeActive)
                {
                    activePlaylist = Menu.Queue;
                    PlaySong();
                }
            }
            catch (Exception error)
            {
                // error message
                MessageBox.Show(error.Message);
            }
        }

        private void DeleteFilesOnClose(object sender, EventArgs e)
        {
            Directory.Delete(folder, true);
        }
    }
}