using System;
using Magenta.Core.Web;

namespace Magenta.Core.Audio;

public class Speech : ISpeech
{
    public Speech()
    {
        Recorder = new AudioRecorder();
        Recognizer = new AudioRecognizer();
        Simplifier = new Simplifier();
        Dubber = new TextDubber();

        Recorder.recordEnded += RecorderOnRecordEnded;
    }

    public AudioRecorder Recorder { get; }

    public AudioRecognizer Recognizer { get; }

    public Simplifier Simplifier { get; }

    public TextDubber Dubber { get; }

    public string GetRecognitionResult => Recognizer.Result;
    public string GetSimplifyResult => Simplifier.Result;

    public void Record()
    {
        Recorder.StartRecording();
    }

    public string Recognition(string recordPath)
    {
        return Recognizer.RecognizeVersionTwo(recordPath);
    }

    public string Analize(string text)
    {
        throw new NotImplementedException();
    }

    public string Simplify(string text)
    {
        return Simplifier.Simplify(text);
    }


    private void RecorderOnRecordEnded()
    {
        Recognition(Config.Instance.TempFilesPath + "output.wav");
    }

    public void Announce(string text)
    {
        Dubber.Announce(text);
    }
}