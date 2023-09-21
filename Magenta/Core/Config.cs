using System;

namespace Magenta.Core;

public class Config
{
    private static Config _config;
    public int SILENCE_DURATION_MS = 1000;

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
    public string ApiKeysPath => @"C:\Users\tekit\keys\c#\";
    public string TempFilesPath => RootPath + @"temp\";

    public string HistoryDir => RootPath + @"history\";

    // MICRO
    public int SAMPLE_RATE => 44100;
    public int CHANNELS => 1;
}