using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NAudio.Wave;
using Pv;

namespace Magenta.Core;

public class WakeWordDetector
{
    private readonly string _accessKey;

    private readonly List<string> _keywordPaths;

    private readonly List<float> _sensitivities;
    private string _modelPath;
    private int _audioDeviceIndex;

    public event MagentaDetectedEvent MagentaDetected;
    public event ShutUpDetectedEvent ShutUpDetected;

    public WakeWordDetector()
    {
        _accessKey = File.ReadAllText(Config.Instance.ApiKeysPath + "porc.txt");
        _keywordPaths = new List<string>
            { @"C:\Magenta_en_windows_v2_2_0.ppn", @"C:\Shut-up_en_windows_v2_2_0.ppn" };
        _sensitivities = new List<float> { 0.8f, 0.8f };
        _modelPath = null;
        _audioDeviceIndex = -1;
    }

    public void Start()
    {
        new Thread(o =>
        {
            Run(File.ReadAllText(Config.Instance.ApiKeysPath + "porc.txt"), _modelPath, _keywordPaths, _sensitivities,
                _audioDeviceIndex);
        }).Start();
    }

    public void Run(
        string accessKey,
        string modelPath,
        List<string> keywordPaths,
        List<float> sensitivities,
        int audioDeviceIndex)
    {
        using (Porcupine porcupine = Porcupine.FromKeywordPaths(accessKey, keywordPaths, modelPath, sensitivities))
        {
            using (PvRecorder recorder =
                   PvRecorder.Create(frameLength: porcupine.FrameLength, deviceIndex: audioDeviceIndex))
            {
                Trace.WriteLine($"Using device: {recorder.SelectedDevice}");

                recorder.Start();

                while (recorder.IsRecording)
                {
                    short[] frame = recorder.Read();

                    int result = porcupine.Process(frame);
                    if (result == 0)
                    {
                        MagentaDetected?.Invoke();
                    }
                    else if (result == 1)
                    {
                        ShutUpDetected?.Invoke();
                    }


                    Thread.Yield();
                }
            }
        }
    }

    public delegate void MagentaDetectedEvent();

    public delegate void ShutUpDetectedEvent();
}