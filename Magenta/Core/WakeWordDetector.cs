using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Pv;

namespace Magenta.Core;

public class WakeWordDetector
{
    public delegate void MagentaDetectedEvent();

    public delegate void ShutUpDetectedEvent();

    private readonly string _accessKey;

    private readonly List<string> _keywordPaths;
    private readonly string _modelPath;

    private readonly List<float> _sensitivities;
    private int _audioDeviceIndex;

    public WakeWordDetector()
    {
        _accessKey = File.ReadAllText(Config.Instance.ApiKeysPath + "porc.txt");
        _keywordPaths = new List<string>
            { @"C:\Magenta_en_windows_v2_2_0.ppn", @"C:\Shut-up_en_windows_v2_2_0.ppn" };
        _sensitivities = new List<float> { 0.8f, 0.8f };
        _modelPath = null;
        _audioDeviceIndex = -1;
    }

    public event MagentaDetectedEvent MagentaDetected;
    public event ShutUpDetectedEvent ShutUpDetected;

    public void Start()
    {
        new Thread(o =>
        {
            Run(File.ReadAllText(Config.Instance.ApiKeysPath + "porc.txt"), _modelPath, _keywordPaths, _sensitivities,
                Config.Instance.AudioDeviceIndex);
        }).Start();
    }

    public void Run(
        string accessKey,
        string modelPath,
        List<string> keywordPaths,
        List<float> sensitivities,
        int audioDeviceIndex)
    {
        using (var porcupine = Porcupine.FromKeywordPaths(accessKey, keywordPaths, modelPath, sensitivities))
        {
            using (var recorder =
                   PvRecorder.Create(porcupine.FrameLength, audioDeviceIndex))
            {
                Trace.WriteLine($"Using device: {recorder.SelectedDevice}");

                recorder.Start();

                while (recorder.IsRecording)
                {
                    var frame = recorder.Read();

                    var result = porcupine.Process(frame);
                    if (result == 0)
                        MagentaDetected?.Invoke();
                    else if (result == 1) ShutUpDetected?.Invoke();


                    Thread.Yield();
                }
            }
        }
    }
}