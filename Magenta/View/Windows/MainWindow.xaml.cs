using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Magenta.Core;
using Magenta.Core.Audio;
using Magenta.Core.Web;
using Microsoft.VisualBasic.Devices;

namespace Magenta;

public partial class MainWindow : Window
{
    private Start _start;
    private ChatGpt _gpt;
    private MediaPlayer _mediaPlayer;

    public MainWindow()
    {
        InitializeComponent();
        _start = new Start();
        _gpt = new ChatGpt();
        _mediaPlayer = new MediaPlayer();
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        _start.StartListening();
        _start.WakeWordDetector.MagentaDetected += () =>
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _mediaPlayer.Open(new Uri(Config.Instance.RECORD_START_SOUND_URI));
                    _mediaPlayer.Play();
                });
            });
        };        
        _start.Speech.Recorder.recordEndStarted += () =>
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _mediaPlayer.Open(new Uri(Config.Instance.RECORD_END_SOUND_URI));
                    _mediaPlayer.Play();
                });
            });
        };
        _start.Speech.Recognizer.RecognitionEndedEvent += () =>
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RecResulTextBlock.Text = _start.Speech.Recognizer.Result;
                    ResponseTextBlock.Text = _start.Gpt.Result;
                });
            });
        };
        _start.Speech.Dubber.AnnounceEnded += () =>
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _mediaPlayer.Open(new Uri(TextDubber.AudioFilePath));
                    _mediaPlayer.Play();
                });
            });
        };
    }
}