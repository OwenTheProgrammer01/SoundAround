using System;
using System.Linq;
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

        public static void Shuffle<T>(this IList<T> list)
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
        List<Artiest> Artiesten = new List<Artiest>();
        List<Bestandtype> Bestandtypen = new List<Bestandtype>();
        List<Song> Songs = new List<Song>();
        List<Song> Wachtrij = new List<Song>();

        //random aanmaken
        Random random = new Random();

        //variabelen aanmaken
        string menu = "start";
        string tablad = "nummers";
        //string zoekopdracht = "";
        int currentSong = -1;
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

            //mediaelement setup
            player.LoadedBehavior = MediaState.Manual;
            player.UnloadedBehavior = MediaState.Manual;
            player.Volume = volume/100;
            player.Clock = null;
            player.MediaEnded += songEnd;

            //zoek textbox setup
            txbZoeken.GotFocus += removeText;
            txbZoeken.LostFocus += addText;

            if (btnShuffle.BorderThickness == new Thickness(0, 0, 0, 1))
            {
                shuffle = true;
            }

            if (btnPause.BorderThickness == new Thickness(0, 0, 0, 0))
            {
                play = true;
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
                //de lijsten invullen met de database gegevens
                Albums = AlbumDA.Ophalen();
                Artiesten = ArtiestDA.Ophalen();
                Bestandtypen = BestandtypeDA.Ophalen();
                Songs = Wachtrij = SongDA.Ophalen();

                //muziek lijst alfabetisch zetten
                Songs.Sort((x, y) => string.Compare(x.Naam, y.Naam));
                Wachtrij.Sort((x, y) => string.Compare(x.Naam, y.Naam));
                invullenGUI();
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        public void invullenGUI()
        {
            try
            {
                if (menu.Equals("start", StringComparison.CurrentCultureIgnoreCase))
                {
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
                }
                else if (menu.Equals("muziekbibliotheek", StringComparison.CurrentCultureIgnoreCase))
                {
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

                    if (tablad.Equals("nummers", StringComparison.CurrentCultureIgnoreCase))
                    {
                        //dikte van de rand veranderen
                        btnNummers.BorderThickness = new Thickness(0, 0, 0, 1);
                        btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 0);
                        btnAlbums.BorderThickness = new Thickness(0, 0, 0, 0);

                        //listbox invullen
                        foreach (Song song in Songs)
                        {
                            lsbMuziekbibliotheek.Items.Add(song.Naam);
                            lsbMuziekbibliotheek.SelectedIndex = currentSong;
                        }
                    }

                    if (tablad.Equals("albums", StringComparison.CurrentCultureIgnoreCase))
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

                    if (tablad.Equals("artiesten", StringComparison.CurrentCultureIgnoreCase))
                    {
                        //dikte van de rand veranderen
                        btnNummers.BorderThickness = new Thickness(0, 0, 0, 0);
                        btnAlbums.BorderThickness = new Thickness(0, 0, 0, 0);
                        btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 1);

                        //listbox invullen
                        foreach (Artiest artiest in Artiesten)
                        {
                            lsbMuziekbibliotheek.Items.Add(artiest.artiest);
                        }
                    }
                }
                else if (menu.Equals("wachtrij", StringComparison.CurrentCultureIgnoreCase))
                {
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
                    
                    if (shuffle)
                    {
                        Wachtrij.Shuffle();
                    }

                    //listbox invullen
                    foreach (Song song in Wachtrij)
                    {
                        lsbWachtrij.Items.Add(song.Naam);
                        lsbWachtrij.SelectedIndex = currentSong;
                    }
                }
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void removeText(object sender, RoutedEventArgs e)
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

        private void addText(object sender, RoutedEventArgs e)
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
                menu = "start";

                //gui invullen
                invullenGUI();
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
                menu = "muziekbibliotheek";

                //gui invullen
                invullenGUI();
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
                menu = "wachtrij";

                //gui invullen
                invullenGUI();
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
                tablad = "nummers";

                //gui invullen
                invullenGUI();
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
                tablad = "albums";

                //gui invullen
                invullenGUI();
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
                tablad = "artiesten";

                //gui invullen
                invullenGUI();
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
                OpenFileDialog file = new OpenFileDialog();
                file.DefaultExt = "mp3";
                file.Filter = "mp3 files (*.mp3)|*.mp3|WAV files (*.wav)|*.wav";

                if (file.ShowDialog() == true)
                {
                    Song song = new Song();
                    Bestandtype bestandtype = new Bestandtype();
                    Artiest artiest = new Artiest();
                    Album album = new Album();
                    BinaryReader br = new BinaryReader(file.OpenFile());

                    bestandtype.bestandtype = Path.GetExtension(file.FileName);

                    foreach (Bestandtype _bestandtype in Bestandtypen)
                    {
                        if (_bestandtype.bestandtype == bestandtype.bestandtype)
                        {
                            song.Bestandtype_ID = _bestandtype.Bestandtype_ID;
                            goto skip;
                        }
                    }

                    BestandtypeDA.Toevoegen(bestandtype);
                    Bestandtypen = BestandtypeDA.Ophalen();

                    foreach (Bestandtype _bestandtype in Bestandtypen)
                    {
                        if (_bestandtype.bestandtype == bestandtype.bestandtype)
                        {
                            song.Bestandtype_ID = _bestandtype.Bestandtype_ID;
                        }
                    }

                    skip:
                    song.Artiest_ID = 1;
                    song.Genre_ID = 1;
                    song.Album_ID = 1;
                    song.Bestand = br.ReadBytes((int)file.OpenFile().Length);
                    song.Naam = Path.GetFileNameWithoutExtension(file.FileName);
                    song.Duur = "00:00:00";

                    if (!SongDA.Toevoegen(song))
                    {
                        MessageBox.Show("Song upload gefaald");
                    }

                    DatabaseOphalen();
                }
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
                    Wachtrij = Songs;
                    Wachtrij.Shuffle();
                    Wachtrij.Remove(Songs[currentSong]);
                    Wachtrij.Insert(0, Songs[currentSong]);
                    invullenGUI();
                }
                //shuffle uit
                else
                {
                    btnShuffle.BorderThickness = new Thickness(0, 0, 0, 0);
                    shuffle = false;
                    Wachtrij.Sort((x, y) => string.Compare(x.Naam, y.Naam));
                    invullenGUI();
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
                //ga naar het laatste item
                if (currentSong == 0)
                {
                    currentSong = Songs.Count - 1;
                }
                //vorige liedje
                else
                {
                    currentSong--;
                }

                if (shuffle)
                {
                    lsbWachtrij.Items.Insert(0, lsbWachtrij.SelectedIndex);
                    selection = false;
                    lsbWachtrij.Items.Remove(lsbWachtrij.SelectedIndex);
                    lsbWachtrij.SelectedIndex = 0;
                    selection = true;
                }

                playSong();
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
                    if (currentSong == -1)
                    {
                        currentSong = 0;
                        playSong();
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
                //begin opnieuw
                if (currentSong >= Songs.Count - 1)
                {
                    currentSong = 0;
                }
                //volgend liedje
                else
                {
                    currentSong++;
                }

                if (shuffle)
                {
                    lsbWachtrij.Items.RemoveAt(0);
                    selection = false;
                    lsbWachtrij.SelectedIndex = 0;
                    selection = true;
                }

                //voer speelliedje uit
                playSong();
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
                if (lsbMuziekbibliotheek.SelectedIndex != -1 || lsbMuziekbibliotheek.SelectedIndex != currentSong && menu == "muziekbibliotheek" && tablad == "nummers" && selection)
                {
                    currentSong = lsbMuziekbibliotheek.SelectedIndex;
                    playSong();
                }
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        public void playSong()
        {
            try
            {
                player.Stop();
                for (int i = 0; i < Bestandtypen.Count; i++)
                {
                    if (Songs[currentSong].Bestandtype_ID == Bestandtypen[i].Bestandtype_ID)
                    {
                        currentType = i;
                    }
                }
                MemoryStream ms = new MemoryStream(Songs[currentSong].Bestand);
                string filepath = $@"C:\Users\{Environment.UserName}\Music\{Songs[currentSong].Naam} SoundAround{Bestandtypen[currentType].bestandtype}";
                if (player.Source != new Uri(filepath))
                {
                    File.WriteAllBytes(filepath, ms.ToArray());
                    player.Source = new Uri(filepath);
                }
                if (lsbMuziekbibliotheek.SelectedIndex != currentSong)
                {
                    selection = false;
                    lsbMuziekbibliotheek.SelectedIndex = currentSong;
                    //lsbWachtrij.SelectedIndex = 0;
                    selection = true;
                }
                player.Play();
                play = true;
                btnPause.BorderThickness = new Thickness(0, 0, 0, 0);
            }
            catch (Exception error)
            {
                //foutmelding
                MessageBox.Show(error.Message);
            }
        }

        private void songEnd(object sender, RoutedEventArgs e)
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
                SongDA.Delete(Songs[currentSong]);
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
                if (lsbWachtrij.SelectedIndex != -1 || lsbWachtrij.SelectedIndex != currentSong && menu == "wachtrij" && selection)
                {
                    for (int i = 0; i < Songs.Count; i++)
                    {
                        if (lsbWachtrij.SelectedItem == Songs[i])
                        {
                            currentSong = i;
                        }
                        playSong();
                    }
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