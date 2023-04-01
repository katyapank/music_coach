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
using NAudio.Wave;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using ZedGraph;
using System.IO;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Windows.Forms.Design;

namespace OSNK_1_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool rec = false;
        static double Fs = 48000; // Частота дискретизации !В данной программе ТОЛЬКО целые числа
        static double T = 1.0 / Fs; // Шаг дискретизации
        static int N; //Длина сигнала (точек)
        static double Fn = Fs / 2;// Частота Найквиста
        WaveIn waveIn;
        private List<double> all_freqs = new List<double>();
        bool recording_new_song = false;
        bool sing_a_song = false;
        int count = 0;
        string chosenPath = "";
        System.Media.SoundPlayer sp;

        List<string> notes = new List<string>();
        List<double> noteVal = new List<double>();
        bool noteMode = true;


        private class TF{
            public double x=0;
            public double y=0;
        }

        TF tf = new TF();
        Ellipse step = new Ellipse();
        double zer = 0;
        double ma = 0;

        Ellipse next = new Ellipse();

        private double toCoord(double fr, double zeroCoord, double maxCoord)
        {
            //return fr * maxCoord / 2000 + zeroCoord;
            return zeroCoord - (zeroCoord - maxCoord) * fr / 2000;
        }

        private void moveY(double fr, double zeroCoord, double maxCoord, Ellipse temp)
        {
            double newcoord = toCoord(fr, zeroCoord, maxCoord);
            if (newcoord <= zeroCoord && newcoord >= maxCoord)
                Canvas.SetTop(temp, (newcoord - tf.y));
        }

        public MainWindow()
        {
            InitializeComponent();
            foreach (UIElement el in grid1.Children)
            {
                if (el is Button)
                {
                    ((Button)el).Click += Button_Click;
                }
                else if (el is TextBox)
                {
                    ((TextBox)el).TextChanged += TextBox_TextChanged;
                }
                else if (el is ComboBox)
                {
                    ((ComboBox)el).SelectionChanged += ComboBox_SelectionChanged;
                }
                else if (el is Slider)
                {
                    ((Slider)el).ValueChanged += Slider_ValueChanged;
                }
            }

            string filePath = "notes1.csv";
            StreamReader reader = null;
            if (File.Exists(filePath))
            {
                reader = new StreamReader(File.OpenRead(filePath));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    int k = 0;
                    foreach (var item in values)
                    {
                        if (k++%2==0)
                            notes.Add(item);
                        else
                            noteVal.Add(Convert.ToDouble(item));
                    }

                }
            }

            
            step.Width = 10;
            step.Height = 3;
            step.VerticalAlignment = VerticalAlignment.Top;
            step.HorizontalAlignment = HorizontalAlignment.Left;
            step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF673AB7");

            step.Margin = new Thickness(bord.Margin.Left*1.5, bord.Margin.Top+bord.Height-(step.Width/2), 0, 0);
            canv.Children.Add(step);

            tf.x = bord.Margin.Left * 1.5;
            tf.y = bord.Margin.Top + bord.Height - (step.Width / 2);

            zer = bord.Margin.Top + bord.Height - (step.Width / 2);
            ma = bord.Margin.Top + (step.Width / 2);

            

            //Canvas.SetTop(step, 0);

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (modeCB.SelectedIndex == 0)
            {
                noteMode = true;
            }
            else if (modeCB.SelectedIndex == 1)
            {
                noteMode = false;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void addPoints()
        {
            next.Width = 10;
            next.Height = 1;
            next.VerticalAlignment = VerticalAlignment.Top;
            next.HorizontalAlignment = HorizontalAlignment.Left;
            next.Fill = Brushes.Black;

            next.Margin = new Thickness(bord.Margin.Left * 1.5+10, bord.Margin.Top + bord.Height - (next.Width / 2), 0, 0);
            canv.Children.Add(next);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)e.OriginalSource).Name == "startB")
            {
                if (rec == false)
                {
                    if (checkMode.IsChecked == true)
                    {
                        sing_a_song = false;
                    }
                    else
                    {
                        if (chosenPath=="")
                        {
                            MessageBox.Show("Песня не выбрана", "Ошибка");
                            return;
                        }
                        sing_a_song=true;
                    }


                    menuB.IsEnabled = false;
                    checkMode.IsEnabled = false;
                    rec = true;
                    if (recording_new_song)
                        all_freqs.Clear();

                    if (sing_a_song)
                    {
                        addPoints();
                        count = 0;
                        all_freqs.Clear();
                        StreamReader sr = new StreamReader(chosenPath.Substring(0, chosenPath.Length - 4) + ".txt");
                        while (!sr.EndOfStream)
                        {
                            all_freqs.Add(Convert.ToDouble(sr.ReadLine()));
                        }
                        
                    }
                    if (recording_new_song || sing_a_song)
                    {
                        sp = new System.Media.SoundPlayer(chosenPath);
                        sp.Play();
                    }

                    this.waveIn = new WaveIn();
                    this.waveIn.DeviceNumber = 0;
                    this.waveIn.DataAvailable += this.waveIn_DataAvailable;
                    this.waveIn.RecordingStopped += this.waveIn_RecordingStopped;
                    this.waveIn.WaveFormat = new WaveFormat((int)Fs, 1);
                    this.waveIn.StartRecording();
                    ((Button)e.OriginalSource).Content = "Остановить запись";
                }
                else
                {
                    menuB.IsEnabled = true;
                    checkMode.IsEnabled = true;
                    if (sing_a_song || recording_new_song)
                        sp.Stop();

                    rec = false;
                    this.stop_recording();

                    if (sing_a_song)
                    {

                        canv.Children.Remove(next);
                    }

                    ((Button)e.OriginalSource).Content = "Начать запись";
                    tempL.Foreground = new SolidColorBrush(Colors.Black);

                    if (recording_new_song)
                        File.WriteAllLines(chosenPath.Substring(0,chosenPath.Length-4)+".txt", all_freqs.ConvertAll(x => x.ToString()));
                }
            }
            else if (((Button)e.OriginalSource).Name == "menuB")
            {
                songsList songMenu = new songsList();
                songMenu.ShowDialog();
                chosenPath = songMenu.chosenSongPath;
                if (chosenPath != "")
                {
                    string k = chosenPath.Substring(chosenPath.IndexOf('\\') + 1);
                    menuB.Content = k.Substring(0, k.IndexOf('_'));
                }
            }
        }
        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            //данные из буфера распределяем в массив чтобы в нем они были в формате ?PCM?
            byte[] buffer = e.Buffer;
            N = buffer.Length;
            int bytesRecorded = e.BytesRecorded;
            Complex[] sig = new Complex[bytesRecorded / 2];
            for (int i = 0, j = 0; i < e.BytesRecorded; i += 2, j++)
            {
                short sample = (short)((buffer[i + 1] << 8) | buffer[i + 0]);
                sig[j] = sample / 32768f;
            }

            Fourier.Forward(sig, FourierOptions.Matlab);
            // обнуляем спектр на небольших частотах (там постоянная составляющая и вообще много помех)
            for (int i = 0; i < 35 * sig.Length / Fn; i++)
            {
                sig[i] = 0;
            }

            write(sig);

        }
        //Окончание записи
        private void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            waveIn.Dispose();
            waveIn = null;
        }
        private void stop_recording()
        {
            this.waveIn.StopRecording();
        }
        private void write(Complex[] signal)
        {
            //var sw = new Stopwatch();
            //sw.Start();
            PointPairList list1 = new PointPairList();
            int max_index = 0;
            double freq = 0;
            double K = signal.Length / 2;
            for (int i = 0; i < K; i++)
            {
                list1.Add(i * Fn / K, Complex.Abs(signal[i]) / N * 2);
            }

            foreach (ZedGraph.PointPair i in list1)
            {
                if (i.Y > list1[max_index].Y)
                {
                    max_index = list1.IndexOf(i);
                }
            }

            try
            {

                freq = list1[max_index].X;  //  иногда здесь выдаёт ошибку ... 
            }
            catch
            {
                MessageBox.Show("Здесь была бы ошибка...");
                return;
            }

            

            string s = "";
            if (!noteMode)
                s = ((int)freq).ToString();
            else
            {
                double k = noteVal.OrderBy(x => Math.Abs(x - freq)).ElementAt(0);
                s = notes[noteVal.IndexOf(k)];
            }

            if (list1[max_index].Y > 0.001)
            {
                tempL.Content = s;
                moveY(freq, zer, ma, step);
                if (recording_new_song)
                    all_freqs.Add(freq);
            }
            else
            {
                moveY(0, zer, ma, step);
                tempL.Content = "-";
                if (recording_new_song)
                    all_freqs.Add(0);
            }

            if (sing_a_song && count < all_freqs.Count)
            {
                if (!noteMode)
                    targetTB.Text = all_freqs[count].ToString();
                else
                {
                    double k = noteVal.OrderBy(x => Math.Abs(x - all_freqs[count])).ElementAt(0);
                    if (all_freqs[count] > 15)
                        targetTB.Text = notes[noteVal.IndexOf(k)];
                    else
                        targetTB.Text = "0";
                }
                moveY(all_freqs[count], zer, ma, next);
            }
            else if (count == all_freqs.Count-1)
            {
                targetTB.Text = "0";
            }

            ++count;

            if (!sing_a_song && !recording_new_song && targetTB.Text != "" && !noteMode)
            {
                if (Math.Abs(freq - Convert.ToInt16(targetTB.Text)) < 15){
                    tempL.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    tempL.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
            else if (sing_a_song && all_freqs.Count >= count) { 
                if (Math.Abs(all_freqs[count-1] - freq) < 20)
                    tempL.Foreground = new SolidColorBrush(Colors.Green);
                else
                    tempL.Foreground = new SolidColorBrush(Colors.Red);
            }

            //sw.Stop();
            //Console.WriteLine(sw.Elapsed);
        }
    }
}
