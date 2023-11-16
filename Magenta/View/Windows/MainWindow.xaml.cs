using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Magenta.Core;
using Magenta.Core.Audio;
using Microsoft.Win32;
using NAudio.CoreAudioApi;

namespace Magenta;

public partial class MainWindow : Window
{
    private readonly MMDeviceCollection _audioDevicesInfo;
    private readonly MainApplication _mainApplication;

    public MainWindow()
    {
        InitializeComponent();
        _mediaPlayer = new MediaPlayer();
        _mainApplication = new MainApplication();
        var enumerator = new MMDeviceEnumerator();
        _audioDevicesInfo = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
        AddListeners();
        Config.LoadConfig();
    }

    public static MediaPlayer _mediaPlayer { get; private set; }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        _mainApplication.StartListening();
        InitFields();
    }

    private void InitFields()
    {
        SampleRateTextBox.Text = Config.Instance.SampleRate.ToString();
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
        _mediaPlayer.MediaEnded += (sender, args) => { _mediaPlayer.Close(); };
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
        Config.SaveConfig();
        System.Windows.Forms.Application.Restart();
    }

    private void StartRecordingSoundButton_OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Audio Fiels (*.mp3;*.wav)";

        if (openFileDialog.ShowDialog() == true)
        {
            File.Copy(openFileDialog.FileName, Config.Instance.RECORD_START_SOUND_URI, true);
        }
    }

    private void EndRecordingSoundButton_OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Audio Fiels (*.mp3;*.wav)";

        if (openFileDialog.ShowDialog() == true)
        {
            File.Copy(openFileDialog.FileName, Config.Instance.RECORD_END_SOUND_URI, true);
        }
    }

    private void HistoryListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        //TODO
    }
}