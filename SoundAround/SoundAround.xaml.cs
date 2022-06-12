﻿using System;
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
        string menu = "start";
        string tablad = "nummers";
        string zoekopdracht = "";
        int currentSong = -1;
        int currentType;
        bool shuffle = false;
        bool play = false;
        bool repeat = false;
        double volume = 1;

        public soundaround()
        {
            InitializeComponent();
            DatabaseOphalen();
            player.LoadedBehavior = MediaState.Manual;
            player.UnloadedBehavior = MediaState.Manual;
            player.Volume = volume;
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
                invullenGUI();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        public void invullenGUI()
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
                lsbBestanden.Items.Clear();
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
                lsbBestanden.Items.Clear();

                if (tablad.Equals("nummers", StringComparison.CurrentCultureIgnoreCase))
                {
                    //dikte van de rand veranderen
                    btnNummers.BorderThickness = new Thickness(0, 0, 0, 1);
                    btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 0);
                    btnAlbums.BorderThickness = new Thickness(0, 0, 0, 0);

                    //listbox invullen
                    foreach (Song song in Songs)
                    {
                        lsbBestanden.Items.Add(song.Naam);
                    }
                }

                if (tablad.Equals("albums", StringComparison.CurrentCultureIgnoreCase))
                {
                    //dikte van de rand veranderen
                    btnNummers.BorderThickness = new Thickness(0, 0, 0, 0);
                    btnAlbums.BorderThickness = new Thickness(0, 0, 0, 1);
                    btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 0);

                    foreach (Album album in Albums)
                    {
                        lsbBestanden.Items.Add(album.album);
                    }
                }

                if (tablad.Equals("artiesten", StringComparison.CurrentCultureIgnoreCase))
                {
                    //dikte van de rand veranderen
                    btnNummers.BorderThickness = new Thickness(0, 0, 0, 0);
                    btnAlbums.BorderThickness = new Thickness(0, 0, 0, 0);
                    btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 1);

                    foreach (Artiest artiest in Artiesten)
                    {
                        lsbBestanden.Items.Add(artiest.artiest);
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
                lsbBestanden.Items.Clear();
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
                //menu gelijk zetten aan start
                menu = "start";

                //gui invullen
                invullenGUI();
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
                //menu gelijk zetten aan start
                menu = "muziekbibliotheek";

                //gui invullen
                invullenGUI();
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
                //menu gelijk zetten aan start
                menu = "wachtrij";

                //gui invullen
                invullenGUI();
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
                //tablad gelijk zetten aan start
                tablad = "nummers";

                //gui invullen
                invullenGUI();
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
                //tablad gelijk zetten aan start
                tablad = "albums";

                //gui invullen
                invullenGUI();
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
                //tablad gelijk zetten aan start
                tablad = "artiesten";

                //gui invullen
                invullenGUI();
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
                if (!shuffle)
                {
                    btnShuffle.BorderThickness = new Thickness(0, 0, 0, 1);
                    shuffle = true;
                }
                else
                {
                    btnShuffle.BorderThickness = new Thickness(0, 0, 0, 0);
                    shuffle = false;
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
                //als je aan het begin bent dan gaat hij naar het einde
                if (currentSong == -1 || currentSong == 0)
                {
                    currentSong = Songs.Count;
                }
                //anders doe je het vorige liedje
                else
                {
                    currentSong--;
                }

                playSong();
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
                    btnPause.BorderThickness = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    player.Pause();
                    play = false;
                    btnPause.BorderThickness = new Thickness(0, 0, 0, 1);
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
                //als je aan het einde bent dan begint hij opnieuw
                if (currentSong == Songs.Count)
                {
                    currentSong = 1;
                }
                //anders het volgende liedje
                else
                {
                    currentSong++;
                }

                //voer speelliedje uit
                playSong();
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
                if (!repeat)
                {
                    repeat = true;
                    btnPause.BorderThickness = new Thickness(0, 0, 0, 1);
                }
                else
                {
                    repeat = false;
                    btnPause.BorderThickness = new Thickness(0, 0, 0, 0);
                }
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
                    currentSong = lsbBestanden.SelectedIndex;
                    playSong();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        public void playSong()
        {
            player.Stop();
            MemoryStream ms = new MemoryStream(Songs[currentSong].Bestand);
            for (int i = 0; i < Bestandtypen.Count; i++)
            {
                if (Songs[currentSong].Bestandtype_ID == Bestandtypen[i].Bestandtype_ID)
                {
                    currentType = i;
                }
            }
            string filepath = $@"C:\Users\{Environment.UserName}\Music\{Songs[currentSong].Naam}{Bestandtypen[currentType].bestandtype}";
            File.WriteAllBytes(filepath, ms.ToArray());
            player.Source = new Uri(filepath);
            player.Play();
            play = true;
        }
    }
}