using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Deepgram;
using Deepgram.Models;

namespace Magenta.Core.Audio;

public class AudioRecognizer
{
    public async Task<string> Recognize(string audioFilePath)
    {
        var credentials = new Credentials(File.ReadAllText(Config.Instance.ApiKeysPath + "deepgram.txt"));


        var deepgramClient = new DeepgramClient(credentials);

        using (var fs = File.OpenRead(audioFilePath))
        {
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

            MessageBox.Show(response.ToSRT());
            return response.ToSRT();
        }
    }
}