using System;

namespace Magenta.Core.Audio;

public class Speech : ISpeech
{
    public void Record()
    {
        new AudioRecorder().StartRecording();
    }

    public string Recognition(string recordPath)
    {
        return new AudioRecognizer().Recognize(recordPath).Result;
    }

    public string Analize(string text)
    {
        throw new NotImplementedException();
    }

    public string Simplify(string text)
    {
        throw new NotImplementedException();
    }
}