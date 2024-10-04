using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace SoundAround
{
    static class ExtensionsClass
    {
        //random aanmaken
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
        //Mediaelement aanmaken
        MediaElement player = new MediaElement();

        //lijsten aanmaken
        List<Album> Albums = new List<Album>();
        List<Artist> Artists = new List<Artist>();
        List<FileType> FileTypes = new List<FileType>();
        List<Song> Muziekbibliotheek = new List<Song>();
        List<Song> Wachtrij = new List<Song>();

        //klasses aanmaken
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

        //variabelen aanmaken
        Menu menu = Menu.Start;
        Tab tab = Tab.Nummers;
        string activeMenuSongPlayer = "start";
        int currentType = -1;
        bool selection = true;
        bool shuffle = false;
        bool play = false;
        bool repeat = false;
        double volume = 100;

        public soundaround()
        {
            InitializeComponent();
            DatabaseOphalen();

            string folder = $@"C:\Users\{Environment.UserName}\Music\SoundAround";

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            //media element setup
            player.LoadedBehavior = MediaState.Manual;
            player.UnloadedBehavior = MediaState.Manual;
            player.Volume = volume/100;
            player.Clock = null;
            player.MediaEnded += SongEnd;

            //zoek textbox setup
            txbZoeken.GotFocus += RemoveText;
            txbZoeken.LostFocus += AddText;

            this.Closed += DeleteFiles;

            if (btnShuffle.BorderThickness == new Thickness(0, 0, 0, 1))
            {
                shuffle = true;
            }

            if (btnRepeat.BorderThickness == new Thickness(0, 0, 0, 1))
            {
                repeat = true;
            }
        }

        public void DatabaseOphalen()
        {
            try
            {
                selection = false;
                //de lijsten invullen met de database gegevens
                Albums = AlbumDA.Ophalen();
                Artists = ArtistDA.Ophalen();
                FileTypes = FileTypeDA.Ophalen();
                Muziekbibliotheek = SongDA.Ophalen();
                Wachtrij = SongDA.Ophalen();

                //muziek lijst alfabetisch zetten
                Muziekbibliotheek.Sort((x, y) => string.Compare(x.Name, y.Name));

                if (shuffle)
                {
                    Wachtrij.Shuffle();
                }
                else
                {
                    Wachtrij.Sort((x, y) => string.Compare(x.Name, y.Name));
                }
                selection = true;
                UpdateGUI();
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        public void UpdateGUI()
        {
            try
            {
                selection = false;

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
                            foreach (Song song in Muziekbibliotheek)
                            {
                                lsbMuziekbibliotheek.Items.Add(song.Name);
                            }

                            if (activeMenuSongPlayer == "muziekbibliotheek")
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
                            foreach (Album album in Albums)
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
                            foreach (Artist artiest in Artists)
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

                        if (activeMenuSongPlayer == "wachtrij")
                        {
                            lsbWachtrij.SelectedItem = currentSong.Name;
                        }

                        //listbox invullen
                        foreach (Song song in Wachtrij)
                        {
                            lsbWachtrij.Items.Add(song.Name);
                        }
                        break;
                }
                lsbMuziekbibliotheek.SelectedItem = currentSong.Name;
                lsbWachtrij.SelectedItem = currentSong.Name;
                selection = true;
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

        private void DeleteFiles(object sender, EventArgs e)
        {
            try
            {
                foreach (Song song in Muziekbibliotheek)
                {
                    for (int i = 0; i < FileTypes.Count; i++)
                    {
                        if (song.FileType_ID == FileTypes[i].FileType_ID)
                        {
                            currentType = i;
                        }
                    }

                    string filepath = $@"C:\Users\{Environment.UserName}\Music\SoundAround\{song.Name} SoundAround{FileTypes[currentType].filetype}";
                    File.Delete(filepath);
                }
            }
            catch (Exception error)
            {
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

                        //fileType.filetype = Path.GetExtension(fileDialog.FileName);
                        fileType.filetype = Path.GetExtension(file);

                        foreach (FileType _bestandtype in FileTypes)
                        {
                            if (_bestandtype.filetype == fileType.filetype)
                            {
                                song.FileType_ID = _bestandtype.FileType_ID;
                                goto skip;
                            }
                        }

                        FileTypeDA.Toevoegen(fileType);
                        FileTypes = FileTypeDA.Ophalen();

                        foreach (FileType _bestandtype in FileTypes)
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
                        song.Duration = "23:59:59";

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
                    Wachtrij.Shuffle();
                    UpdateGUI();
                }
                //shuffle uit
                else
                {
                    btnShuffle.BorderThickness = new Thickness(0, 0, 0, 0);
                    shuffle = false;
                    Wachtrij.Sort((x, y) => string.Compare(x.Name, y.Name));
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
                selection = false;
                if (activeMenuSongPlayer == "muziekbibliotheek")
                {
                    lsbMuziekbibliotheek.SelectedItem = currentSong.Name;
                    //ga naar laatste item
                    if (lsbMuziekbibliotheek.SelectedIndex == -1 || lsbMuziekbibliotheek.SelectedIndex == 0)
                    {
                        lsbMuziekbibliotheek.SelectedIndex = Muziekbibliotheek.Count - 1;
                    }
                    //vorig liedje
                    else
                    {
                        lsbMuziekbibliotheek.SelectedIndex--;
                    }
                }
                if (activeMenuSongPlayer == "wachtrij")
                {
                    lsbWachtrij.SelectedItem = currentSong.Name;
                    //ga naar laatste item
                    if (lsbWachtrij.SelectedIndex == -1 || lsbWachtrij.SelectedIndex == 0)
                    {
                        lsbWachtrij.SelectedIndex = Wachtrij.Count - 1;
                    }
                    //vorig liedje
                    else
                    {
                        lsbWachtrij.SelectedIndex--;
                    }
                }
                selection = true;
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
                            selection = false;
                            lsbMuziekbibliotheek.SelectedIndex = 0;
                            selection = true;
                            currentSong = Muziekbibliotheek[lsbMuziekbibliotheek.SelectedIndex];
                        }

                        if (menu == Menu.Wachtrij)
                        {
                            selection = false;
                            lsbWachtrij.SelectedIndex = 0;
                            selection = true;
                            currentSong = Wachtrij[lsbWachtrij.SelectedIndex];
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
                selection = false;
                if (activeMenuSongPlayer == "muziekbibliotheek")
                {
                    //begin opnieuw
                    if (lsbMuziekbibliotheek.SelectedIndex == Muziekbibliotheek.Count - 1)
                    {
                        lsbMuziekbibliotheek.SelectedIndex = 0;
                    }
                    //volgend liedje
                    else
                    {
                        lsbMuziekbibliotheek.SelectedIndex++;
                    }
                }
                if (activeMenuSongPlayer == "wachtrij")
                {
                    //begin opnieuw
                    if (lsbWachtrij.SelectedIndex == Wachtrij.Count - 1)
                    {
                        lsbWachtrij.SelectedIndex = 0;
                    }
                    //volgend liedje
                    else
                    {
                        lsbWachtrij.SelectedIndex++;
                    }
                }
                selection = true;
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
                if (lsbMuziekbibliotheek.SelectedIndex != -1 && tab == Tab.Nummers && selection)
                {
                    activeMenuSongPlayer = "muziekbibliotheek";
                    PlaySong();
                }
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        public void PlaySong()
        {
            try
            {
                if (activeMenuSongPlayer == "muziekbibliotheek")
                {
                    currentSong = Muziekbibliotheek[lsbMuziekbibliotheek.SelectedIndex];
                }

                if (activeMenuSongPlayer == "wachtrij")
                {
                    currentSong = Wachtrij[lsbWachtrij.SelectedIndex];
                }

                selection = false;
                player.Stop();
                for (int index = 0; index < FileTypes.Count; index++)
                {
                    if (currentSong.FileType_ID == FileTypes[index].FileType_ID)
                    {
                        currentType = index;
                    }
                }

                MemoryStream ms = new MemoryStream(currentSong.SongFile);
                string filepath = $@"C:\Users\{Environment.UserName}\Music\SoundAround\{currentSong.Name} SoundAround{FileTypes[currentType].filetype}";

                if (player.Source != new Uri(filepath))
                {
                    File.WriteAllBytes(filepath, ms.ToArray());
                }

                player.Source = new Uri(filepath);
                player.Play();
                play = true;
                btnPause.BorderThickness = new Thickness(0, 0, 0, 0);
                selection = true;

                lblSongName.Content = currentSong.Name;
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void SongEnd(object sender, RoutedEventArgs e)
        {
            try
            {
                //herhaal
                if (repeat)
                {
                    player.Position = TimeSpan.FromSeconds(0);
                }
                //volgend nummer
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

        private void btnNummerVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selection = false;
                SongDA.Delete(currentSong);
                selection = true;
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
            player.Volume = volume/100;
        }

        private void lsbWachtrij_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lsbWachtrij.SelectedIndex != -1 && selection)
                {
                    activeMenuSongPlayer = "wachtrij";
                    PlaySong();
                }
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }
    }
}