using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Media;

namespace SoundAround
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SoundPlayer player = new SoundPlayer();

        List<Song> Songs = new List<Song>();
        List<Bestandtype> Bestandtypen = new List<Bestandtype>();

        public MainWindow()
        {
            InitializeComponent();
            DatabaseOphalen();
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
                int selectedSong = lsbBestanden.SelectedIndex;
                player.Stream = Songs[selectedSong].Bestand;
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
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
                    //BinaryReader br = new BinaryReader(file.OpenFile());
                    byte[] data = File.ReadAllBytes(file.FileName);
                    MemoryStream ms = new MemoryStream(data);

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

                    //byte[] buffer = new byte[1000000000];
                    //bestand = file.OpenFile();
                    //bestand.Read(buffer, 0, buffer.Length);

                    //song.Bestand = br.ReadBytes((int)bestand.Length);

                    //song.Bestand = ;
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
    }
}