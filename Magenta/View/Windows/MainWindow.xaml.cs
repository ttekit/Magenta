using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Magenta.Core;
using Magenta.Core.Audio;
using Magenta.Core.Execution.Executors;
using Magenta.Core.Web;
using Magenta.View.Windows;
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
        AnswerSettings.Instance.LoadSettings();
        InitTextBoxes();
        InitHistoryView();
        InitEspListBox();
    }

    public static MediaPlayer _mediaPlayer { get; private set; }

    private void InitEspListBox()
    {
        ESPListView.ItemsSource = ESP32WordsArray.Instance.GetArray();
    }

    private void InitHistoryView()
    {
        var files = Directory.GetFiles(Config.Instance.HISTORY_DIR);
        HistoryListView.ItemsSource = files;
    }

    private void InitTextBoxes()
    {
        SatiricTextBox.Text = AnswerSettings.Instance.Satiric;
        BehaviorTextBox.Text = AnswerSettings.Instance.Style;
        HumorTextBox.Text = AnswerSettings.Instance.Humor;
        AggerssiveTextBox.Text = AnswerSettings.Instance.Agresive;
        ToleranceTextBox.Text = AnswerSettings.Instance.Tolerance;
    }

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
                    if (_mainApplication.IsWorkingEnded)
                    {
                        _mediaPlayer.Open(new Uri(Config.Instance.RECORD_START_SOUND_URI));
                        _mediaPlayer.Play();
                    }
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
        UpdateAnswerSettings();
        ESP32WordsArray.Instance.SaveToFile();

        System.Windows.Forms.Application.Restart();
        Application.Current.Shutdown();
    }

    private void UpdateAnswerSettings()
    {
        var satiric = SatiricTextBox.Text;
        var style = BehaviorTextBox.Text;
        var humor = HumorTextBox.Text;
        var agressive = AggerssiveTextBox.Text;
        var tolerance = ToleranceTextBox.Text;

        AnswerSettings.Instance.UpdateData(style, satiric, humor, agressive, tolerance);
        AnswerSettings.Instance.SaveToFile();
    }

    private void StartRecordingSoundButton_OnClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Audio Fiels (*.mp3;*.wav)";

        if (openFileDialog.ShowDialog() == true)
            File.Copy(openFileDialog.FileName, Config.Instance.RECORD_START_SOUND_URI, true);
    }

    private void EndRecordingSoundButton_OnClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Audio Fiels (*.mp3;*.wav)";

        if (openFileDialog.ShowDialog() == true)
            File.Copy(openFileDialog.FileName, Config.Instance.RECORD_END_SOUND_URI, true);
    }

    private void HistoryListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var selectedHistory = HistoryListView.SelectedItem.ToString();
        _mainApplication.Gpt.SetHistory(selectedHistory);
        MessageBox.Show("История обновлена");
    }

    private void ViewHistoryButton_OnClick(object sender, RoutedEventArgs e)
    {
        var view = new HistoryView(_mainApplication.Gpt.GetHistory());
        view.ShowDialog();
    }

    private void ESPAddButton_OnClick(object sender, RoutedEventArgs e)
    {
        var addEspDivice = new AddEspDivice();
        addEspDivice.ShowDialog();
    }

    private void ESPListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var selectedItem = (EspItem)ESPListView.SelectedItem;
        var editEspDivice = new EditEspDivice(selectedItem);
        editEspDivice.ShowDialog();
    }

    private void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        Thread.CurrentThread.Abort();
    }
}