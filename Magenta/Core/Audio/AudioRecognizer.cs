using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Deepgram.Models;

namespace Magenta.Core.Audio;

public class AudioRecognizer
{
    public AudioRecognizer()
    {
    }

    public async Task<string> Recognize(string audioFilePath)
    {
        var credentials = new Credentials(File.ReadAllText(Config.Instance.ApiKeysPath + "deepgram.txt"));


        var deepgramClient = new Deepgram.DeepgramClient(credentials);

        Trace.WriteLine("Requesting transcript...");
        Trace.WriteLine("Your file may take up to a couple minutes to process.");
        Trace.WriteLine("While you wait, did you know that Deepgram accepts over 40 audio file formats? Even MP4s.");
        Trace.WriteLine("To learn more about customizing your transcripts check out developers.deepgram.com.");
        using (FileStream fs = File.OpenRead(audioFilePath))
        {
            var response = await deepgramClient.Transcription.Prerecorded.GetTranscriptionAsync(
                new StreamSource(
                    fs,
                    "audio/wav"),            
                new PrerecordedTranscriptionOptions()
                {
                    Punctuate = true,
                    Utterances = true,
                    Model = "base",
                    Language = "ru"
                }
            );

            MessageBox.Show(response.ToSRT());
            return response.ToSRT();
        }

    }
}