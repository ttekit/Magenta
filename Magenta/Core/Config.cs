using System;
using System.IO;
using Newtonsoft.Json;

namespace Magenta.Core;

public class Config
{
    private static Config _config;

    private Config()
    {
    }

    [JsonIgnore] public string ROOT_PATH => @"E:\Magenta\Magenta\";

    [JsonIgnore] public string CONFIG_PATH => @"E:\Magenta\Magenta\config.json";
    [JsonIgnore] public string API_KEYS_PATH => @"C:\Users\tekit\zzzkeys\c#\";
    [JsonIgnore] public string TEMP_FILES_PATH => ROOT_PATH + @"temp\";

    [JsonIgnore] public string HISTORY_DIR => ROOT_PATH + @"history\";
    [JsonIgnore] public string AUDIO_DIR => ROOT_PATH + @"audio\";

    [JsonIgnore] public string RECORD_START_SOUND_URI => AUDIO_DIR + "bipStart.mp3";
    [JsonIgnore] public string RECORD_END_SOUND_URI => AUDIO_DIR + "bipEnd.mp3";
    [JsonIgnore] public string ANSWER_SETTINGS_PATH => ROOT_PATH + "answerSettings.json";

    public static Config Instance
    {
        get
        {
            if (_config == null) _config = new Config();
            return _config;
        }
        private set => _config = value ?? throw new ArgumentNullException(nameof(value));
    }

    public int SilenceDurationMs { get; set; } = 1000;

    public int StopDurationMs { get; set; } = 3000;

    public float SilenceThreshold { get; set; } = 0.01f;

    public int AudioDeviceIndex { get; set; } = 0;

    public int SampleRate { get; set; } = 44100;

    public int Channels { get; set; } = 1;

    public static void LoadConfig()
    {
        if (!File.Exists(_config.CONFIG_PATH)) return;

        var jsonString = File.ReadAllText(_config.CONFIG_PATH);
        _config = JsonConvert.DeserializeObject<Config>(jsonString);
    }

    public static void SaveConfig()
    {
        var jsonString = JsonConvert.SerializeObject(_config);
        File.WriteAllText(_config.CONFIG_PATH, jsonString);
    }
}