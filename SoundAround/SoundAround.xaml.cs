﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Windows;

namespace SoundAround
{
    /// <summary>
    /// Interaction logic for soundaround.xaml
    /// </summary>
    public partial class soundaround : Window
    {
        //soundplayer aanmaken
        SoundPlayer player = new SoundPlayer();

        //lijsten aanmaken
        List<Album> Albums = new List<Album>();
        List<Artiest> Artiesten = new List<Artiest>();
        List<Bestandtype> Bestandtypen = new List<Bestandtype>();
        List<Genre> Genres = new List<Genre>();
        List<Song> Songs = new List<Song>();

        //variabelen aanmaken
        int selectedSong;

        public soundaround()
        {
            InitializeComponent();
            DatabaseOphalen();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //grid zichtbaar maken en de andere niet zichtbaar
            grdStart.Visibility = Visibility.Visible;
            grdMuziekbibliotheek.Visibility = Visibility.Hidden;
            grdWachtrij.Visibility = Visibility.Hidden;

            //dikte van de rand veranderen
            btnStart.BorderThickness = new Thickness(0, 0, 0, 1);
            btnMuziekbibliotheek.BorderThickness = new Thickness(0, 0, 0, 0);
            btnWachtrij.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        private void btnMuziekbibliotheek_Click(object sender, RoutedEventArgs e)
        {
            //grid zichtbaar maken en de andere niet zichtbaar
            grdStart.Visibility = Visibility.Hidden;
            grdMuziekbibliotheek.Visibility = Visibility.Visible;
            grdWachtrij.Visibility = Visibility.Hidden;

            //dikte van de rand veranderen
            btnStart.BorderThickness = new Thickness(0, 0, 0, 0);
            btnMuziekbibliotheek.BorderThickness = new Thickness(0, 0, 0, 1);
            btnWachtrij.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        private void btnWachtrij_Click(object sender, RoutedEventArgs e)
        {
            //grid zichtbaar maken en de andere niet zichtbaar
            grdStart.Visibility = Visibility.Hidden;
            grdMuziekbibliotheek.Visibility = Visibility.Hidden;
            grdWachtrij.Visibility = Visibility.Visible;

            //dikte van de rand veranderen
            btnStart.BorderThickness = new Thickness(0, 0, 0, 0);
            btnMuziekbibliotheek.BorderThickness = new Thickness(0, 0, 0, 0);
            btnWachtrij.BorderThickness = new Thickness(0, 0, 0, 1);
        }

        private void btnNummers_Click(object sender, RoutedEventArgs e)
        {
            //dikte van de rand veranderen
            btnNummers.BorderThickness = new Thickness(0, 0, 0, 1);
            btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 0);
            btnAlbums.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        private void btnAlbums_Click(object sender, RoutedEventArgs e)
        {
            //dikte van de rand veranderen
            btnNummers.BorderThickness = new Thickness(0, 0, 0, 0);
            btnAlbums.BorderThickness = new Thickness(0, 0, 0, 1);
            btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        private void btnArtiesten_Click(object sender, RoutedEventArgs e)
        {
            //dikte van de rand veranderen
            btnNummers.BorderThickness = new Thickness(0, 0, 0, 0);
            btnAlbums.BorderThickness = new Thickness(0, 0, 0, 0);
            btnArtiesten.BorderThickness = new Thickness(0, 0, 0, 1);
        }

        private void btnNummersToevoegen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool controle;
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
                    SoundPlayer sp = new SoundPlayer();

                    bestandtype.bestandtype = file.DefaultExt;

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
                    sp.Stream = ms;
                    song.Naam = file.SafeFileName;
                    song.Duur = "0";

                    controle = SongDA.Toevoegen(song);
                    if (controle)
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DatabaseOphalen()
        {
            Bestandtypen = BestandtypeDA.Ophalen();
            Songs = SongDA.Ophalen();
            GUIInvullen();
        }

        public void GUIInvullen()
        {
            lsbBestanden.Items.Clear();
            foreach (Song song in Songs)
            {
                lsbBestanden.Items.Add(song.Naam);
            }
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selectedSong = lsbBestanden.SelectedIndex;
                MemoryStream ms = new MemoryStream(Songs[selectedSong].Bestand);
                player.Stream = ms;
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}