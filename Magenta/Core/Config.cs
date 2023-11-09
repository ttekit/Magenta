using System;

namespace Magenta.Core;

public class Config
{
    private static Config _config;
    public int SILENCE_DURATION_MS = 1000;
    public int STOP_DURATION_MS = 3000;

    public float SILENCE_THRESHOLD = 0.01f;

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

    //FILES
    public string RootPath => @"E:\Magenta\Magenta\";
    public string ApiKeysPath => @"C:\Users\tekit\zzzkeys\c#\";
    public string TempFilesPath => RootPath + @"temp\";

    public string HistoryDir => RootPath + @"history\";
    public string AudioDir => RootPath + @"audio\";

    public string RECORD_START_SOUND_URI => AudioDir + "bipStart.mp3";
    public string RECORD_END_SOUND_URI => AudioDir + "bipEnd.mp3";

    public int AudioDeviceIndex = 0;

    // MICRO
    public int SAMPLE_RATE => 44100;
    public int CHANNELS => 1;
}