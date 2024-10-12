using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;

namespace SoundAround
{
    static class ExtensionsClass
    {
        private static Random random = new Random();

        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T tempValue = list[k];
                list[k] = list[n];
                list[n] = tempValue;
            }
        }
    }

    /// <summary>
    /// Interaction logic for soundaround.xaml
    /// </summary>
    public partial class soundaround : Window
    {
        // mediaelement aanmaken
        MediaElement player = new MediaElement();

        // timer aanmaken
        DispatcherTimer timer = new DispatcherTimer();

        // lijsten aanmaken
        List<Album> albums = new List<Album>();
        List<Artist> artists = new List<Artist>();
        List<FileType> fileTypes = new List<FileType>();
        List<Song> musicLibrary = new List<Song>();
        List<Song> queue = new List<Song>();

        // klasses aanmaken
        Song currentSong = new Song();

        enum Menu
        {
            Start,
            Muziekbibliotheek,
            Wachtrij
        }

        enum Tab
        {
            Nummers,
            Albums,
            Artiesten
        }

        // variabelen aanmaken
        Menu menu = Menu.Start;
        Menu activePlaylist = Menu.Muziekbibliotheek;
        Tab tab = Tab.Nummers;
        int currentType = -1;
        bool selectionChangeActive = true;
        bool shuffle = false;
        bool play = false;
        bool repeat = false;
        double volume = 100;

        string folder = $@"C:\Users\{Environment.UserName}\Music\SoundAround";

        public soundaround()
        {
            InitializeComponent();

            DatabaseOphalen();

            // folder aanmaken als deze nog niet bestaat
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // timer instellen
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += new EventHandler(Update);
            timer.Start();

            // mediaelement instellen
            player.LoadedBehavior = MediaState.Manual;
            player.UnloadedBehavior = MediaState.Manual;
            player.Volume = volume / 100;
            player.Clock = null;
            player.MediaEnded += SongEnded;

            if (btnShuffle.BorderThickness == new Thickness(0, 0, 0, 1))
            {
                shuffle = true;
            }

            if (btnRepeat.BorderThickness == new Thickness(0, 0, 0, 1))
            {
                repeat = true;
            }

            this.Closed += DeleteFiles;
        }

        private void DatabaseOphalen()
        {
            try
            {
                //selectionChangeActive = false;

                //de lijsten invullen met de database gegevens
                albums = AlbumDA.Ophalen();
                artists = ArtistDA.Ophalen();
                fileTypes = FileTypeDA.Ophalen();
                musicLibrary = SongDA.Ophalen();
                queue = SongDA.Ophalen();

                //de lijsten sorteren op alfabetische volgorde
                musicLibrary.Sort((x, y) => string.Compare(x.Name, y.Name));
                queue.Sort((x, y) => string.Compare(x.Name, y.Name));

                // de wachtrij shuffelen als shuffle aan staat
                if (shuffle)
                {
                    queue.Shuffle();
                }

                //selectionChangeActive = true;
                UpdateGUI();
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void Update(object sender, EventArgs e)
        {
            try
            {
                // de huidige positie van het nummer updaten
                if (play)
                {
                    lblHuidigePositie.Content = $"{player.Position.ToString(@"hh\:mm\:ss")}";
                }
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void UpdateGUI()
        {
            try
            {
                selectionChangeActive = false;

                switch (menu)
                {
                    case Menu.Start:
                        //grid zichtbaar maken en de andere niet zichtbaar
                        grdStart.Visibility = Visibility.Visible;
                        grdMuziekbibliotheek.Visibility = Visibility.Hidden;
                        grdWachtrij.Visibility = Visibility.Hidden;

                        //dikte van de rand veranderen
                        btnStart.BorderThickness = new Thickness(0, 0, 0, 1);
                        btnMuziekbibliotheek.BorderThickness = new Thickness(0, 0, 0, 0);
                        btnWachtrij.BorderThickness = new Thickness(0, 0, 0, 0);

                        //listbox leegmaken
                        lsbStart.Items.Clear();
                        break;
                    case Menu.Muziekbibliotheek:
                        //grid zichtbaar maken en de andere niet zichtbaar
                        grdStart.Visibility = Visibility.Hidden;
                        grdMuziekbibliotheek.Visibility = Visibility.Visible;
                        grdWachtrij.Visibility = Visibility.Hidden;

                        //dikte van de rand veranderen
                        btnStart.BorderThickness = new Thickness(0, 0, 0, 0);
                        btnMuziekbibliotheek.BorderThickness = new Thickness(0, 0, 0, 1);
                        btnWachtrij.BorderThickness = new Thickness(0, 0, 0, 0);

                        //listbox leegmaken
                        lsbMuziekbibliotheek.Items.Clear();

                        if (tab == Tab.Nummers)
                        {
                            //dikte van de rand veranderen
                            btnNummers.BorderThickness = new Thickness(0, 0, 0, 1);
                            btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 0);
                            btnAlbums.BorderThickness = new Thickness(0, 0, 0, 0);

                            //listbox invullen
                            foreach (Song song in musicLibrary)
                            {
                                lsbMuziekbibliotheek.Items.Add(song.Name);
                            }

                            if (activePlaylist == Menu.Muziekbibliotheek)
                            {
                                lsbMuziekbibliotheek.SelectedItem = currentSong.Name;
                            }
                        }

                        if (tab == Tab.Albums)
                        {
                            //dikte van de rand veranderen
                            btnNummers.BorderThickness = new Thickness(0, 0, 0, 0);
                            btnAlbums.BorderThickness = new Thickness(0, 0, 0, 1);
                            btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 0);

                            //listbox invullen
                            foreach (Album album in albums)
                            {
                                lsbMuziekbibliotheek.Items.Add(album.album);
                            }
                        }

                        if (tab == Tab.Artiesten)
                        {
                            //dikte van de rand veranderen
                            btnNummers.BorderThickness = new Thickness(0, 0, 0, 0);
                            btnAlbums.BorderThickness = new Thickness(0, 0, 0, 0);
                            btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 1);

                            //listbox invullen
                            foreach (Artist artiest in artists)
                            {
                                lsbMuziekbibliotheek.Items.Add(artiest.artist);
                            }
                        }
                        break;
                    case Menu.Wachtrij:
                        //grid zichtbaar maken en de andere niet zichtbaar
                        grdStart.Visibility = Visibility.Hidden;
                        grdMuziekbibliotheek.Visibility = Visibility.Hidden;
                        grdWachtrij.Visibility = Visibility.Visible;

                        //dikte van de rand veranderen
                        btnStart.BorderThickness = new Thickness(0, 0, 0, 0);
                        btnMuziekbibliotheek.BorderThickness = new Thickness(0, 0, 0, 0);
                        btnWachtrij.BorderThickness = new Thickness(0, 0, 0, 1);

                        //listbox leegmaken
                        lsbWachtrij.Items.Clear();

                        if (activePlaylist == Menu.Wachtrij)
                        {
                            lsbWachtrij.SelectedItem = currentSong.Name;
                        }

                        //listbox invullen
                        foreach (Song song in queue)
                        {
                            lsbWachtrij.Items.Add(song.Name);
                        }
                        break;
                }
                lsbMuziekbibliotheek.SelectedItem = currentSong.Name;
                lsbWachtrij.SelectedItem = currentSong.Name;
                lblSongName.Content = currentSong.Name;
                lblEindePositie.Content = $"{player.NaturalDuration}";
                
                selectionChangeActive = true;
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            try
            {
                txbZoeken.Text = "Zoeken";
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            try
            {
                txbZoeken.Text = txbZoeken.Text.Replace("Zoeken", "");
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void PlaySong()
        {
            try
            {
                if (activePlaylist == Menu.Muziekbibliotheek)
                {
                    currentSong = musicLibrary[lsbMuziekbibliotheek.SelectedIndex];
                }

                if (activePlaylist == Menu.Wachtrij)
                {
                    currentSong = queue[lsbWachtrij.SelectedIndex];
                }

                selectionChangeActive = false;
                player.Stop();
                for (int index = 0; index < fileTypes.Count; index++)
                {
                    if (currentSong.FileType_ID == fileTypes[index].FileType_ID)
                    {
                        currentType = index;
                    }
                }

                MemoryStream ms = new MemoryStream(currentSong.SongFile);
                string filepath = $@"C:\Users\{Environment.UserName}\Music\SoundAround\{currentSong.Name} SoundAround{fileTypes[currentType].filetype}";

                if (player.Source != new Uri(filepath))
                {
                    File.WriteAllBytes(filepath, ms.ToArray());
                }

                player.Source = new Uri(filepath);
                player.Play();
                play = true;
                btnPause.BorderThickness = new Thickness(0, 0, 0, 0);
                selectionChangeActive = true;
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void SongEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                // opnieuw afspelen van het nummer als repeat aan staat
                if (repeat)
                {
                    player.Position = TimeSpan.FromSeconds(0);
                }
                // volgend nummer afspelen
                else
                {
                    btnNext_Click(sender, e);
                }
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void btnZoeken_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //menu gelijk zetten aan start
                menu = Menu.Start;

                //gui invullen
                UpdateGUI();
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void btnMuziekbibliotheek_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //menu gelijk zetten aan start
                menu = Menu.Muziekbibliotheek;

                //gui invullen
                UpdateGUI();
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void btnWachtrij_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //menu gelijk zetten aan start
                menu = Menu.Wachtrij;

                //gui invullen
                UpdateGUI();
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void btnNummers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //tablad gelijk zetten aan start
                tab = Tab.Nummers;

                //gui invullen
                UpdateGUI();
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void btnAlbums_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //tablad gelijk zetten aan start
                tab = Tab.Albums;

                //gui invullen
                UpdateGUI();
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void btnArtiesten_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //tablad gelijk zetten aan start
                tab = Tab.Artiesten;

                //gui invullen
                UpdateGUI();
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
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

                        FileTypeDA.Toevoegen(fileType);
                        fileTypes = FileTypeDA.Ophalen();

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

                        if (!SongDA.Toevoegen(song))
                        {
                            MessageBox.Show("Failed to upload the song");
                        }

                    }
                }
                DatabaseOphalen();
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //shuffle aan
                if (!shuffle)
                {
                    btnShuffle.BorderThickness = new Thickness(0, 0, 0, 1);
                    shuffle = true;

                    //de lijst willekeurig maken
                    queue.Shuffle();
                    UpdateGUI();
                }
                //shuffle uit
                else
                {
                    btnShuffle.BorderThickness = new Thickness(0, 0, 0, 0);
                    shuffle = false;
                    queue.Sort((x, y) => string.Compare(x.Name, y.Name));
                    UpdateGUI();
                }
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selectionChangeActive = false;
                if (activePlaylist == Menu.Muziekbibliotheek)
                {
                    lsbMuziekbibliotheek.SelectedItem = currentSong.Name;
                    //ga naar laatste item
                    if (lsbMuziekbibliotheek.SelectedIndex == -1 || lsbMuziekbibliotheek.SelectedIndex == 0)
                    {
                        lsbMuziekbibliotheek.SelectedIndex = musicLibrary.Count - 1;
                    }
                    //vorig liedje
                    else
                    {
                        lsbMuziekbibliotheek.SelectedIndex--;
                    }
                }
                if (activePlaylist == Menu.Wachtrij)
                {
                    lsbWachtrij.SelectedItem = currentSong.Name;
                    //ga naar laatste item
                    if (lsbWachtrij.SelectedIndex == -1 || lsbWachtrij.SelectedIndex == 0)
                    {
                        lsbWachtrij.SelectedIndex = queue.Count - 1;
                    }
                    //vorig liedje
                    else
                    {
                        lsbWachtrij.SelectedIndex--;
                    }
                }
                selectionChangeActive = true;
                PlaySong();
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //play
                if (!play)
                {
                    if (currentSong == null)
                    {
                        if (menu == Menu.Muziekbibliotheek)
                        {
                            selectionChangeActive = false;
                            lsbMuziekbibliotheek.SelectedIndex = 0;
                            selectionChangeActive = true;
                            currentSong = musicLibrary[lsbMuziekbibliotheek.SelectedIndex];
                        }

                        if (menu == Menu.Wachtrij)
                        {
                            selectionChangeActive = false;
                            lsbWachtrij.SelectedIndex = 0;
                            selectionChangeActive = true;
                            currentSong = queue[lsbWachtrij.SelectedIndex];
                        }
                        PlaySong();
                        btnPause.BorderThickness = new Thickness(0, 0, 0, 0);
                        return;
                    }
                    player.Play();
                    play = true;
                    btnPause.BorderThickness = new Thickness(0, 0, 0, 0);
                }
                //pauze
                else
                {
                    player.Pause();
                    play = false;
                    btnPause.BorderThickness = new Thickness(0, 0, 0, 1);
                }
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selectionChangeActive = false;
                if (activePlaylist == Menu.Muziekbibliotheek)
                {
                    //begin opnieuw
                    if (lsbMuziekbibliotheek.SelectedIndex == musicLibrary.Count - 1)
                    {
                        lsbMuziekbibliotheek.SelectedIndex = 0;
                    }
                    //volgend liedje
                    else
                    {
                        lsbMuziekbibliotheek.SelectedIndex++;
                    }
                }
                if (activePlaylist == Menu.Wachtrij)
                {
                    //begin opnieuw
                    if (lsbWachtrij.SelectedIndex == queue.Count - 1)
                    {
                        lsbWachtrij.SelectedIndex = 0;
                    }
                    //volgend liedje
                    else
                    {
                        lsbWachtrij.SelectedIndex++;
                    }
                }
                selectionChangeActive = true;
                //voer speelliedje uit
                PlaySong();
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void btnRepeat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //herhalen
                if (!repeat)
                {
                    repeat = true;
                    btnRepeat.BorderThickness = new Thickness(0, 0, 0, 1);
                }
                //niet herhalen
                else
                {
                    repeat = false;
                    btnRepeat.BorderThickness = new Thickness(0, 0, 0, 0);
                }
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void lsbMuziekbibliotheek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lsbMuziekbibliotheek.SelectedIndex != -1 && tab == Tab.Nummers && selectionChangeActive)
                {
                    activePlaylist = Menu.Muziekbibliotheek;
                    PlaySong();
                }
            }
            catch (Exception error)
            {
                //foutmelding
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
                DatabaseOphalen();
            }
            catch (Exception error)
            {
                //foutmelding
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
                if (lsbWachtrij.SelectedIndex != -1 && selectionChangeActive)
                {
                    activePlaylist = Menu.Wachtrij;
                    PlaySong();
                }
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void DeleteFiles(object sender, EventArgs e)
        {
            try
            {
                Directory.Delete(folder, true);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
    }
}