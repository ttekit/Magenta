using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Magenta.Core.Web;

namespace Magenta.Core.Audio;

public class Speech : ISpeech
{
    private AudioRecorder _recorder;
    private AudioRecognizer _recognizer;
    private Simplifier _simplifier;

    public AudioRecorder Recorder => _recorder;

    public AudioRecognizer Recognizer => _recognizer;

    public Simplifier Simplifier => _simplifier;

    public Speech()
    {
        _recorder = new AudioRecorder();
        _recorder.recordEnded += RecorderOnRecordEnded;
        _recognizer = new AudioRecognizer();
        _simplifier = new Simplifier();
    }

    private void RecorderOnRecordEnded()
    {
        Recognition(Config.Instance.TempFilesPath + "output.wav");
    }


    public void Record()
    {
        _recorder.StartRecording();
    }

    public string Recognition(string recordPath)
    {
        return _recognizer.Recognize(recordPath).Result;
    }

    public string Analize(string text)
    {
        throw new NotImplementedException();
    }

    public string Simplify(string text)
    {
        return _simplifier.Simplify(text);
    }

    public string GetRecognitionResult => _recognizer.Result;
    public string GetSimplifyResult => _simplifier.Result;
}