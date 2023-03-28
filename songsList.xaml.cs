using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
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
using System.Windows.Shapes;

namespace OSNK_1_wpf
{
    /// <summary>
    /// Логика взаимодействия для songsList.xaml
    /// </summary>
    ///
    public partial class songsList : Window
    {
        public string chosenSongPath = "";
        string[] files;
        List<string> paths = new List<string>();
        class Songs
        {
            public Songs(string Song, int Level)
            {
                this.Song = Song;
                this.Level = Level;
            }

            public string Song { get; set; }
            public int Level { get; set; }
        }
        //List<Songs> songs = new List<Songs>();
        ObservableCollection<Songs> songs = new ObservableCollection<Songs>();
        public songsList()
        {
            InitializeComponent();

            foreach (UIElement el in grid2.Children)
            {
                if (el is DataGrid)
                {
                    ((DataGrid)el).MouseDoubleClick += DataGrid_MouseDoubleClick;
                }
            }

            files = Directory.GetFiles("songs");

            foreach (var file in files)
            {
                if (file.Substring(file.Length - 3) == "wav")
                {
                    paths.Add(file);
                    string[] temp = file.Split('_');
                    string songName = temp[0].Substring(temp[0].IndexOf('\\') + 1);
                    if (songName.Length > 26)
                    {
                        songName = songName.Substring(0, 24) + "..";
                    }
                    songs.Add(new Songs(songName, temp[1][0] - 48));
                }

            }

            songL.ItemsSource = songs;

        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = songL.SelectedIndex;

            chosenSongPath = paths[index];

            this.Close();

        }
    }
}
