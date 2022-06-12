using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace SoundAround
{
    /// <summary>
    /// Interaction logic for soundaround.xaml
    /// </summary>
    public partial class soundaround : Window
    {
        //soundplayer aanmaken
        MediaElement player = new MediaElement();

        //lijsten aanmaken
        List<Album> Albums = new List<Album>();
        List<Artiest> Artiesten = new List<Artiest>();
        List<Bestandtype> Bestandtypen = new List<Bestandtype>();
        List<Genre> Genres = new List<Genre>();
        List<Song> Songs = new List<Song>();

        //variabelen aanmaken
        string zoekopdracht = "";
        int selectedSong;
        int selectedType;
        bool shuffle = false;
        bool play = false;
        bool repeat = false;

        public soundaround()
        {
            InitializeComponent();
            DatabaseOphalen();
            player.Volume = 1;
            player.Clock = null;
        }

        public void DatabaseOphalen()
        {
            try
            {
                //de lijsten invullen met de database gegevens
                Albums = AlbumDA.Ophalen();
                Artiesten = ArtiestDA.Ophalen();
                Bestandtypen = BestandtypeDA.Ophalen();
                Genres = GenreDA.Ophalen();
                Songs = SongDA.Ophalen();
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
                MessageBox.Show(error.Message);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
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
                lsbBestanden.Items.Clear();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void btnMuziekbibliotheek_Click(object sender, RoutedEventArgs e)
        {
            try
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
                lsbBestanden.Items.Clear();

                //listbox invullen
                foreach (Song song in Songs)
                {
                    lsbBestanden.Items.Add(song.Naam);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void btnWachtrij_Click(object sender, RoutedEventArgs e)
        {
            try
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
                lsbBestanden.Items.Clear();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void btnNummers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //dikte van de rand veranderen
                btnNummers.BorderThickness = new Thickness(0, 0, 0, 1);
                btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 0);
                btnAlbums.BorderThickness = new Thickness(0, 0, 0, 0);

                //listbox leegmaken
                lsbBestanden.Items.Clear();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void btnAlbums_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //dikte van de rand veranderen
                btnNummers.BorderThickness = new Thickness(0, 0, 0, 0);
                btnAlbums.BorderThickness = new Thickness(0, 0, 0, 1);
                btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 0);

                //listbox leegmaken
                lsbBestanden.Items.Clear();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void btnArtiesten_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //dikte van de rand veranderen
                btnNummers.BorderThickness = new Thickness(0, 0, 0, 0);
                btnAlbums.BorderThickness = new Thickness(0, 0, 0, 0);
                btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 1);

                //listbox leegmaken
                lsbBestanden.Items.Clear();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void btnNummersToevoegen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string error;

                OpenFileDialog file = new OpenFileDialog();
                file.DefaultExt = ".wav";
                file.Filter = "WAV-bestand (.wav)| * .wav";

                if (file.ShowDialog() == true)
                {
                    Song song = new Song();
                    Bestandtype bestandtype = new Bestandtype();
                    Artiest artiest = new Artiest();
                    Genre genre = new Genre();
                    Album album = new Album();
                    BinaryReader br = new BinaryReader(file.OpenFile());
                    MemoryStream ms;

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

                    skip:
                    song.Artiest_ID = 1;
                    song.Genre_ID = 1;
                    song.Album_ID = 1;
                    song.Bestand = br.ReadBytes((int)file.OpenFile().Length);
                    ms = new MemoryStream(song.Bestand);
                    song.Naam = Path.GetFileNameWithoutExtension(file.FileName);
                    song.Duur = "0";

                    if (SongDA.Toevoegen(song))
                    {
                        error = "Song upload gelukt";
                    }
                    else
                    {
                        error = "Song upload gefaald";
                    }

                    MessageBox.Show(error);

                    DatabaseOphalen();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnShuffle.BorderThickness.Equals(new Thickness(0, 0, 0, 0)))
                {
                    btnShuffle.BorderThickness = new Thickness(0, 0, 0, 1);
                }
                else
                {
                    btnShuffle.BorderThickness = new Thickness(0, 0, 0, 0);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!play)
                {
                    player.Play();
                    play = true;
                }
                else
                {
                    player.Pause();
                    play = false;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void btnRepeat_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void lsbBestanden_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lsbBestanden.SelectedIndex != -1)
                {
                    selectedSong = lsbBestanden.SelectedIndex;
                    MemoryStream ms = new MemoryStream(Songs[selectedSong].Bestand);
                    for (int i = 0; i < Bestandtypen.Count; i++)
                    {
                        if (Songs[selectedSong].Bestandtype_ID == Bestandtypen[i].Bestandtype_ID)
                        {
                            selectedType = i;
                        }
                    }
                    string filepath = $@"C:\Users\{Environment.UserName}\Music\{Songs[selectedSong].Naam}{Bestandtypen[selectedType].bestandtype}";
                    File.WriteAllBytes(filepath, ms.ToArray());
                    player.Source = new Uri(filepath);
                    player.LoadedBehavior = MediaState.Manual;
                    player.UnloadedBehavior = MediaState.Manual;
                    player.Play();
                    play = true;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
    }
}