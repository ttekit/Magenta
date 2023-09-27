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
    private TextDubber _dubber;

    public AudioRecorder Recorder => _recorder;

    public AudioRecognizer Recognizer => _recognizer;

    public Simplifier Simplifier => _simplifier;
    public TextDubber Dubber => _dubber;

    public Speech()
    {
        _recorder = new AudioRecorder();
        _recognizer = new AudioRecognizer();
        _simplifier = new Simplifier();
        _dubber = new TextDubber();

        _recorder.recordEnded += RecorderOnRecordEnded;
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

    public void Announce(string text)
    {
        _dubber.Announce(text);
    }

    public string GetRecognitionResult => _recognizer.Result;
    public string GetSimplifyResult => _simplifier.Result;
}