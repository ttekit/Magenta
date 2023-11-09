using System;
using System.IO;
using System.Threading;
using Google.Cloud.TextToSpeech.V1;

namespace Magenta.Core.Audio;

public class TextDubber
{
    public static readonly string AudioFilePath = Config.Instance.TempFilesPath + "textAnnounce.wav";
    public event AnnounceEndedEvent AnnounceEnded;
    public event AnnounceStartedEvent AnnounceStarted;

    public void Announce(string textToSpeak)
    {
        AnnounceStarted?.Invoke();

        var jsonKeyFilePath = Config.Instance.ApiKeysPath + "google.json";
        var clientBuilder = new TextToSpeechClientBuilder
        {
            JsonCredentials = File.ReadAllText(jsonKeyFilePath)
        };
        var client = clientBuilder.Build();


        var input = new SynthesizeSpeechRequest
        {
            Input = new SynthesisInput
            {
                Text = textToSpeak
            },
            Voice = new VoiceSelectionParams
            {
                LanguageCode = "ru-RU",
                Name = "ru-RU-Standard-D"
            },
            AudioConfig = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Linear16,
                Pitch = -2
            }
        };

        var response = client.SynthesizeSpeech(input);
        while (true)
            try
            {
                File.WriteAllBytes(AudioFilePath, response.AudioContent.ToByteArray());
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Thread.Sleep(100);
            }

        AnnounceEnded?.Invoke();
    }
}

public delegate void AnnounceStartedEvent();

public delegate void AnnounceEndedEvent();