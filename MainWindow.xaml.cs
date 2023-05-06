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
    public partial class MainWindow : Window
    {
        int intervalMode;
        bool rec = false;
        bool isLyrics = false;
        static double Fs = 48000;
        static double T = 1.0 / Fs;
        static int N;
        static double Fn = Fs / 2;
        WaveIn waveIn;
        private List<double> all_freqs = new List<double>();
        bool recording_new_song = false;
        bool sing_a_song = false;
        int count = 0;
        string chosenPath = "";
        System.Media.SoundPlayer sp;
        int mark = 0;
        int maxmark = 0;

        List<string> notes = new List<string>();
        List<double> noteVal = new List<double>();
        bool noteMode = true;


        private class TF{
            public double x=0;
            public double y=0;
        }

        TF tf = new TF();
        Rectangle step = new Rectangle();
        double zer = 0;
        double ma = 0;
        Rectangle next = new Rectangle();

        private double toCoord(double fr, double zeroCoord, double maxCoord)
        {
            return zeroCoord - (zeroCoord - maxCoord) * fr / 2000;
        }

        private void moveY(double fr, double zeroCoord, double maxCoord, Rectangle temp)
        {
            double newcoord = toCoord(fr, zeroCoord, maxCoord);
            if (newcoord <= zeroCoord && newcoord >= maxCoord)
                Canvas.SetTop(temp, (newcoord - tf.y));
        }

        public MainWindow()
        {
            InitializeComponent();
            lyrics.Text = "Текст песни отсутствует";
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

            
            step.Width = 50;
            step.Height = 10;
            step.VerticalAlignment = VerticalAlignment.Top;
            step.HorizontalAlignment = HorizontalAlignment.Left;
            step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF673AB7");

            step.Margin = new Thickness(bord.Margin.Left*1.5, bord.Margin.Top+bord.Height-(step.Height), 0, 0);
            canv.Children.Add(step);

            tf.x = bord.Margin.Left * 1.5;
            tf.y = bord.Margin.Top + bord.Height - (step.Height);

            zer = bord.Margin.Top + bord.Height - (step.Height);
            ma = bord.Margin.Top + (step.Height);


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
            next.Width = 50;
            next.Height = 10;
            next.VerticalAlignment = VerticalAlignment.Top;
            next.HorizontalAlignment = HorizontalAlignment.Left;
            next.Fill = Brushes.Black;

            next.Margin = new Thickness(bord.Margin.Left * 1.5+70, bord.Margin.Top + bord.Height - (next.Height), 0, 0);
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
                        mark = 0;
                        maxmark = 0;
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
                        if (isLyrics == true)
                        {
                            //lyrics.ScrollToEnd();
                        }
                    }
                    else
                    {
                        intervalMode = intervalCB.SelectedIndex;
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
                        double newmark = (mark * 100 / maxmark) * 3;
                        if (newmark > 100) newmark = 100;
                        MessageBox.Show("Ваша оценка: " + Convert.ToInt16(newmark).ToString()  + " баллов из 100", "Результат");
                        mark = 0;
                        maxmark = 0;
                        moveY(0, zer, ma, step);
                        canv.Children.Remove(next);
                        step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF673AB7");
                        targetTB.Text = "";
                        tempL.Foreground = new SolidColorBrush(Colors.Black);
                        tempL.Content = "-";
                    }
                    else if (!sing_a_song && !recording_new_song)
                    {
                        moveY(0, zer, ma, step);
                        step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF673AB7");
                        tempL.Foreground = new SolidColorBrush(Colors.Black);
                        tempL.Content = "-";
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

                    lyrics.Text = "";
                    try
                    {
                        StreamReader sr = new StreamReader(chosenPath.Substring(0, chosenPath.Length - 4) + ".log");
                        bool cas = false;
                        while (!sr.EndOfStream)
                        {
                            if (cas == false)
                            {
                                cas = true;
                                lyrics.Text += sr.ReadLine();
                            }
                            lyrics.Text += "\n" + sr.ReadLine();
                        }
                        isLyrics = true;

                    }
                    catch
                    {
                        lyrics.Text = "Текст песни отсутствует";
                        isLyrics = false;
                    }
                }
            }
        }
        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
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
            for (int i = 0; i < 35 * sig.Length / Fn; i++)
            {
                sig[i] = 0;
            }

            write(sig);

        }

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

                freq = list1[max_index].X;
            }
            catch
            {
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

            double allowableDifference = 0.60575;

            double m = 41.21;

            while (m < 2638)
            {
                if (freq >= m)
                    allowableDifference *= 2;
                else
                    break;
                if (m > 328 && m < 650) 
                    allowableDifference += 0.15;
                m *= 2;
            }

            if (!sing_a_song && !recording_new_song && targetTB.Text != "")
            {
                intervalMode = intervalCB.SelectedIndex;
                double targ = 0;
                if (noteMode)
                {
                    if (targetTB.Text == tempL.Content.ToString())
                    {
                        tempL.Foreground = new SolidColorBrush(Colors.Green);
                        step.Fill = Brushes.Green;
                    }
                    else if (intervalMode != 0 && list1[max_index].Y > 0.001)
                    {
                        int temp = notes.IndexOf(targetTB.Text);
                        double tempD = 0;
                        if (temp != -1 && freq>20)
                        {
                            tempD = noteVal[temp];

                            double k = noteVal.OrderBy(x => Math.Abs(x - freq)).ElementAt(0);
                            string freqstr = notes[noteVal.IndexOf(k)];
                            k = noteVal[notes.IndexOf(freqstr)];

                            double bv = 0;
                            double sv = 0;
                            if (k > tempD)
                            { bv = k; sv = tempD; }
                            else
                            { bv = tempD; sv = k; }
                            if (intervalMode==1 && Math.Abs(bv/sv - 1.25) < 0.015)
                            {
                                tempL.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC839");
                                step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC839");
                            }
                            else if (intervalMode == 2 && Math.Abs(bv / sv - 1.2) < 0.015)
                            {
                                tempL.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC839");
                                step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC839");
                            }
                            else if (intervalMode == 3 && Math.Abs(bv / sv - 1.33) < 0.015)
                            {
                                tempL.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC839");
                                step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC839");
                            }
                            else if (intervalMode == 4 && Math.Abs(bv / sv - 1.5) < 0.015)
                            {
                                tempL.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC839");
                                step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC839");
                            }
                            else
                            {
                                tempL.Foreground = new SolidColorBrush(Colors.Red);
                                step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF673AB7");
                            }
                        }
                        else
                        {
                            tempL.Foreground = new SolidColorBrush(Colors.Red);
                            step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF673AB7");
                        }
                    }
                    else
                    {
                        tempL.Foreground = new SolidColorBrush(Colors.Red);
                        step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF673AB7");
                    }
                }
                else
                {
                    try
                    {
                        targ = Convert.ToInt16(targetTB.Text);
                    }
                    catch
                    {
                        targetTB.Text = "0";
                        targ = 0;
                    }

                    if (Math.Abs(freq - targ) < allowableDifference)
                    {
                        tempL.Foreground = new SolidColorBrush(Colors.Green);
                        step.Fill = Brushes.Green;
                    }

                    else
                    {
                        tempL.Foreground = new SolidColorBrush(Colors.Red);
                        step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF673AB7");
                    }
                }
            }
            else if (sing_a_song && all_freqs.Count >= count) {

                double k = noteVal.OrderBy(x => Math.Abs(x - freq)).ElementAt(0);
                string freqstr = notes[noteVal.IndexOf(k)];
                k = noteVal[notes.IndexOf(freqstr)];

                double k1 = noteVal.OrderBy(x => Math.Abs(x - all_freqs[count - 1])).ElementAt(0);
                string freqstr1 = notes[noteVal.IndexOf(k1)];
                k1 = noteVal[notes.IndexOf(freqstr1)];

                if (Math.Abs(all_freqs[count - 1] - freq) < allowableDifference)
                {
                    tempL.Foreground = new SolidColorBrush(Colors.Green);
                    step.Fill = Brushes.Green;
                    next.Fill = Brushes.Green;
                    ++mark;
                    ++maxmark;
                }
                else if (Math.Abs(k1 - k) < 0.01 && freq>20 && list1[max_index].Y > 0.001 /*&& Math.Abs(freq-k) < allowableDifference*2*/)
                {
                    tempL.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC839");
                    step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC839");
                    next.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC839");
                    ++mark;
                    ++maxmark;
                }
                else
                {
                    tempL.Foreground = new SolidColorBrush(Colors.Red);
                    step.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF673AB7");
                    next.Fill = Brushes.Black;
                    ++maxmark;
                }
            }
        }
    }
}
