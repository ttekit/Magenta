using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Magenta.Core;
using Magenta.Core.Audio;
using Magenta.Core.Web;

namespace Magenta;

public partial class MainWindow : Window
{
    private MainApplication _mainApplication;
    private static MediaPlayer _mediaPlayer;
    public static MediaPlayer MediaPlayer => _mediaPlayer;

    public MainWindow()
    {
        InitializeComponent();
        _mediaPlayer = new MediaPlayer();
        _mainApplication = new MainApplication();
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        _mainApplication.StartListening();

        _mainApplication.WakeWordDetector.MagentaDetected += () =>
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
        _mainApplication.WakeWordDetector.ShutUpDetected += () =>
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _mediaPlayer.Stop();
                    _mediaPlayer.Close();
                });
            });
        };
        _mainApplication.Speech.Recorder.recordEndStarted += () =>
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
        _mainApplication.Speech.Recognizer.RecognitionEndedEvent += () =>
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RecResulTextBlock.Text = _mainApplication.Speech.Recognizer.Result;
                    ResponseTextBlock.Text = _mainApplication.Gpt.Result;
                });
            });
        };
        _mainApplication.Speech.Dubber.AnnounceEnded += () =>
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