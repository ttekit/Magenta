using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Deepgram;
using Deepgram.Models;
using Google.Cloud.Speech.V1;

namespace Magenta.Core.Audio;

public class AudioRecognizer
{
    public string Result { get; private set; }
    public event OnRecognitionEnded RecognitionEndedEvent;

    public async Task<string> Recognize(string audioFilePath)
    {
        Trace.WriteLine("RECOGNIZE STARTED");
        var credentials = new Credentials(File.ReadAllText(Config.Instance.API_KEYS_PATH + "deepgram.txt"));

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
            Trace.Write("RESPONSE: ");
            var temp = response.ToSRT().Split(" - ");
            for (var i = 1; i < temp.Length; i++)
            {
                Console.WriteLine(temp);
                Result += temp[i];
            }

            RecognitionEndedEvent?.Invoke();
            return Result;
        }
    }

    public string RecognizeVersionTwo(string audioFilePath)
    {
        var speech = SpeechClient.Create();
        var res = "";
        var config = new RecognitionConfig
        {
            Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
            SampleRateHertz = 44100,
            LanguageCode = LanguageCodes.Russian.Russia
        };
        var audio = RecognitionAudio.FromFile(audioFilePath);

        var response = speech.Recognize(config, audio);

        foreach (var result in response.Results)
        foreach (var alternative in result.Alternatives)
            res = alternative.Transcript;

        Result = res;
        RecognitionEndedEvent?.Invoke();
        File.Delete(audioFilePath);
        return res;
    }
}

public delegate void OnRecognitionEnded();