using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Deepgram;
using Deepgram.Models;

namespace Magenta.Core.Audio;

public class AudioRecognizer
{
    public event OnRecognitionEnded RecognitionEndedEvent;
    public string Result { get; private set; }

    public async Task<string> Recognize(string audioFilePath)
    {
        Trace.WriteLine("RECOGNIZE STARTED");
        var credentials = new Credentials(File.ReadAllText(Config.Instance.ApiKeysPath + "deepgram.txt"));

        var deepgramClient = new DeepgramClient(credentials);

        using (var fs = File.OpenRead(audioFilePath))
        {
            Trace.WriteLine("DATA READ");
            var response = await deepgramClient.Transcription.Prerecorded.GetTranscriptionAsync(
                new StreamSource(
                    fs,
                    "audio/wav"),
                new PrerecordedTranscriptionOptions
                {
                    Punctuate = true,
                    Utterances = true,
                    Model = "base",
                    Language = "ru"
                }
            );
            Trace.WriteLine("RECOGNIZE ENDED");
            Result = response.ToSRT();
            Trace.WriteLine("RESPONSE: " + response.ToSRT());
            RecognitionEndedEvent?.Invoke();
            return response.ToSRT();
        }
    }
}

public delegate void OnRecognitionEnded();