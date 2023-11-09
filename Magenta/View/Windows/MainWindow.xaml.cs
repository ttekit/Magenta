using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Magenta.Core;
using Magenta.Core.Audio;
using NAudio.CoreAudioApi;

namespace Magenta;

public partial class MainWindow : Window
{
    private readonly MMDeviceCollection _audioDevicesInfo;
    private readonly MainApplication _mainApplication;

    public MainWindow()
    {
        InitializeComponent();
        MediaPlayer = new MediaPlayer();
        _mainApplication = new MainApplication();
        var enumerator = new MMDeviceEnumerator();
        _audioDevicesInfo = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
        AddListeners();
        // Config.LoadConfig();
    }

    public static MediaPlayer MediaPlayer { get; private set; }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        _mainApplication.StartListening();
        InitFields();
    }

    private void InitFields()
    {
        SampleRateTextBox.Text = Config.Instance.SAMPLE_RATE.ToString();
        foreach (var info in _audioDevicesInfo) AudioDevicesComboBox.Items.Add(info.DeviceFriendlyName);
    }

    private void AddListeners()
    {
        _mainApplication.WakeWordDetector.MagentaDetected += () =>
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MediaPlayer.Open(new Uri(Config.Instance.RECORD_START_SOUND_URI));
                    MediaPlayer.Play();
                });
            });
        };
        _mainApplication.WakeWordDetector.ShutUpDetected += () =>
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MediaPlayer.Stop();
                    MediaPlayer.Close();
                });
            });
        };
        _mainApplication.Speech.Recorder.recordEndStarted += () =>
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MediaPlayer.Open(new Uri(Config.Instance.RECORD_END_SOUND_URI));
                    MediaPlayer.Play();
                });
            });
        };
        _mainApplication.Speech.Dubber.AnnounceEnded += () =>
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MediaPlayer.Open(new Uri(TextDubber.AudioFilePath));
                    MediaPlayer.Play();
                });
            });
        };
    }

    private void AudioDevicesComboBox_OnSelected(object sender, RoutedEventArgs e)
    {
        var desiredDeviceName = AudioDevicesComboBox.SelectedItem.ToString();
        var id = 0;
        for (var i = 0; i < _audioDevicesInfo.Count; i++)
            if (_audioDevicesInfo[i].DeviceFriendlyName == desiredDeviceName)
            {
                id = i;
                break;
            }

        Config.Instance.AudioDeviceIndex = id;
    }

    private void SaveChangesButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Config.Instance.SaveConfig();
    }
}