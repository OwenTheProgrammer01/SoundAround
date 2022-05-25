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

namespace SoundAround
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            foreach (Song song in Songs)
            {
                lsbBestanden.Items.Add(song.Naam);
            }
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool controle;

                OpenFileDialog file = new OpenFileDialog();
                file.DefaultExt = ".wav";
                file.Filter = "WAV-bestand (.wav)|* .wav";

                if (file.ShowDialog() == true)
                {
                    Song song = new Song();
                    Bestandtype bestandtype = new Bestandtype();
                    Artiest artiest = new Artiest();
                    Genre genre = new Genre();
                    Album album = new Album();
                    BinaryReader br = new BinaryReader(file.OpenFile());

                    bestandtype.bestandtype = file.DefaultExt;
                    controle = BestandtypeDA.Toevoegen(bestandtype);
                    if (controle)
                    {
                        MessageBox.Show("Bestandtype upload gelukt");
                        DatabaseOphalen();
                    }
                    else
                    {
                        MessageBox.Show("Bestandtype upload gefaald");
                    }

                    foreach (Bestandtype _bestandtype in Bestandtypen)
                    {
                        if (_bestandtype.bestandtype == bestandtype.bestandtype)
                        {
                            song.Bestandtype_ID = _bestandtype.Bestandtype_ID;
                        }
                    }

                    song.Artiest_ID = 1;
                    song.Genre_ID = 1;
                    song.Album_ID = 1;

                    byte[] buffer = new byte[1000000000];
                    Stream bestand = file.OpenFile();
                    bestand.Read(buffer, 0, buffer.Length);

                    song.Bestand = br.ReadBytes((int)bestand.Length);
                    song.Naam = file.SafeFileName;
                    song.Duur = TimeSpan.FromSeconds(file.OpenFile().Length);

                    controle = SongDA.Toevoegen(song);
                    if (controle)
                    {
                        MessageBox.Show("Song upload gelukt");
                        DatabaseOphalen();
                    }
                    else
                    {
                        MessageBox.Show("Song upload gefaald");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}