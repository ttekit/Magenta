using System;
using System.IO;
using Google.Cloud.TextToSpeech.V1;

namespace Magenta.Core.Audio;

public class TextDubber
{
    public event AnnounceEndedEvent AnnounceEnded;
    public static readonly string AudioFilePath = Config.Instance.TempFilesPath + "textAnnounce.wav"; 
    public void Announce(string textToSpeak)
    {
        string jsonKeyFilePath = Config.Instance.ApiKeysPath + "google.json";
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
                Name = "ru-RU-Standard-D",
            },
            AudioConfig = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Linear16,
                Pitch = -2
            }
        };

        var response = client.SynthesizeSpeech(input);

        File.WriteAllBytes(AudioFilePath, response.AudioContent.ToByteArray());
        AnnounceEnded?.Invoke();
        
    }
}

public delegate void AnnounceEndedEvent();