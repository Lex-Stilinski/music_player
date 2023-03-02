using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace music_player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void Timer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        static string[] fuulpath = new string[0];
        static string[] items = new string[0];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void audioslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblStatus.Content = String.Format("{0} / {1}", media.Position.ToString(@"mm\:ss"), media.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            media.Position = TimeSpan.FromSeconds(audioslider.Value);
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            audioslider.Maximum = media.NaturalDuration.TimeSpan.Ticks;
        }

        private void open_folder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                items = Directory.GetFiles(dialog.FileName).Where(x => x.EndsWith(".mp3") || x.EndsWith(".wav")).ToArray();
                fuulpath = items.ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    FileInfo file = new FileInfo(items[i]);
                    items[i] = file.Name;
                }
                musics.ItemsSource = items;
                musics.SelectedIndex = 0;
                media.Source = new Uri(fuulpath[0]);
                media.Play();
                media.Volume = 0.5;
                volumeslider.Value = media.Volume;
                iconplay.Kind = PackIconKind.Pause;
            }

            Timer();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if ((media.Source != null) && (media.NaturalDuration.HasTimeSpan))
            {
                audioslider.Minimum = 0;
                audioslider.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;
                audioslider.Value = media.Position.TotalSeconds;
            }
            else
                lblStatus.Content = "No file selected...";
        }

        private void musics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = fuulpath[musics.SelectedIndex].ToString();
            media.Source = new Uri(selected);
            iconplay.Kind = PackIconKind.Pause;
            media.Play();
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (iconplay.Kind == PackIconKind.Play) 
            {
                iconplay.Kind = PackIconKind.Pause;
                media.Play();
            }
            else
            {
                iconplay.Kind = PackIconKind.Play;
                media.Pause();
            }
        }

        private void left_Click(object sender, RoutedEventArgs e)
        {

            if (musics.SelectedIndex == 0)
            {
                musics.SelectedIndex = fuulpath.Length - 1;
            }
            else
            {
                musics.SelectedIndex--;
            }
        }

        private void right_Click(object sender, RoutedEventArgs e)
        {
            if (musics.SelectedIndex == (fuulpath.Length - 1))
            {
                musics.SelectedIndex = 0;
            }
            else
            {
                musics.SelectedIndex++;
            }
            media.Play();
            iconplay.Kind = PackIconKind.Pause;
        }

        private void shuffle_Click(object sender, RoutedEventArgs e)
        {
            if (iconShuffle.Kind == PackIconKind.Shuffle)
            {
                iconShuffle.Kind = PackIconKind.ShuffleDisabled;
                var rnd = new Random();
                string[] rndmus = fuulpath.OrderBy(s => rnd.Next()).ToArray();
                string[] rndfull = rndmus.ToArray();
                for (int i = 0; i < rndmus.Length; i++)
                {
                    FileInfo file = new FileInfo(rndmus[i]);
                    rndmus[i] = file.Name;
                }
                musics.ItemsSource = rndmus;
                musics.SelectedIndex = 0;
                media.Source = new Uri(rndfull[0]);
                media.Play();
            }
            else
            {
                iconShuffle.Kind = PackIconKind.Shuffle;
                musics.ItemsSource = items;
                musics.SelectedIndex = 0;
                media.Source = new Uri(fuulpath[0]);
                media.Play();
            }
        }

        private void repeat_Click(object sender, RoutedEventArgs e)
        {
            if (iconRepeat.Kind == PackIconKind.RepeatOne)
            {
                iconRepeat.Kind = PackIconKind.RepeatOff;
            }
            else
            {
                iconRepeat.Kind = PackIconKind.RepeatOne;
            }
        }

        private void volumeslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            volumeslider.Minimum = 0;
            volumeslider.Maximum = 1;
            media.Volume = (double)volumeslider.Value;
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (iconRepeat.Kind == PackIconKind.RepeatOne)
            {
                if (musics.SelectedIndex == (fuulpath.Length - 1))
                {
                    audioslider.Value = 0;
                    musics.SelectedIndex = 0;
                }
                else
                {
                    audioslider.Value = 0;
                    musics.SelectedIndex++;
                }
            }
            else if (iconRepeat.Kind == PackIconKind.RepeatOff)
            {
                audioslider.Value = 0;
                musics.SelectedIndex = musics.SelectedIndex;
            }
        }
    }
}
