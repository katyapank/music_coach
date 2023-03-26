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

        public MainWindow()
        {
            InitializeComponent();
            foreach(UIElement el in grid1.Children)
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
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)e.OriginalSource).Name == "startB")
            {
                if (rec == false)
                {
                    rec = true;
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
                    rec = false;
                    this.stop_recording();
                    ((Button)e.OriginalSource).Content = "Начать запись";
                    tempL.Foreground = new SolidColorBrush(Colors.Black);
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
            //list1[max_index].Y = list1[max_index].Y;
            freq = list1[max_index].X;
            string s = ((int)freq).ToString();
            if (list1[max_index].Y > 0.001)
                tempL.Content = s;
            else
                tempL.Content = "-";

            if (targetTB.Text != "")
            {
                if (Math.Abs(freq - Convert.ToInt16(targetTB.Text)) < 15){
                    tempL.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    tempL.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }
    }
}
