using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Magenta.Core;

public class Config
{
    private static Config _config;
    [JsonIgnore] public string ROOT_PATH => @"E:\Magenta\Magenta\";

    [JsonIgnore] public string CONFIG_PATH => @"E:\Magenta\Magenta\config.json";
    [JsonIgnore] public string API_KEYS_PATH => @"C:\Users\tekit\zzzkeys\c#\";
    [JsonIgnore] public string TEMP_FILES_PATH => ROOT_PATH + @"temp\";

    [JsonIgnore] public string HISTORY_DIR => ROOT_PATH + @"history\";
    [JsonIgnore] public string AUDIO_DIR => ROOT_PATH + @"audio\";

    [JsonIgnore] public string RECORD_START_SOUND_URI => AUDIO_DIR + "bipStart.mp3";
    [JsonIgnore] public string RECORD_END_SOUND_URI => AUDIO_DIR + "bipEnd.mp3";

    private Config()
    {
    }

    public static Config Instance
    {
        get
        {
            if (_config == null) _config = new Config();
            return _config;
        }
        private set => _config = value ?? throw new ArgumentNullException(nameof(value));
    }

    private int _silenceDurationMs = 1000;
    private int _stopDurationMs = 3000;
    private float _silenceThreshold = 0.01f;
    private int _audioDeviceIndex = 0;
    private int _sampleRate = 44100;
    private int _channels = 1;

    public int SilenceDurationMs
    {
        get => _silenceDurationMs;
        set => _silenceDurationMs = value;
    }

    public int StopDurationMs
    {
        get => _stopDurationMs;
        set => _stopDurationMs = value;
    }

    public float SilenceThreshold
    {
        get => _silenceThreshold;
        set => _silenceThreshold = value;
    }

    public int AudioDeviceIndex
    {
        get => _audioDeviceIndex;
        set => _audioDeviceIndex = value;
    }

    public int SampleRate
    {
        get => _sampleRate;
        set => _sampleRate = value;
    }

    public int Channels
    {
        get => _channels;
        set => _channels = value;
    }

    public static void LoadConfig()
    {
        if (!File.Exists(_config.CONFIG_PATH)) return;

        string jsonString = File.ReadAllText(_config.CONFIG_PATH);
        _config = JsonConvert.DeserializeObject<Config>(jsonString);
    }

    public static void SaveConfig()
    {
        string jsonString = JsonConvert.SerializeObject(_config);
        File.WriteAllText(_config.CONFIG_PATH, jsonString);
    }
}