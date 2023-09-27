using Magenta.Core.Audio;
using Magenta.Core.Web;


namespace Magenta.Core;

public class Start
{
    private WakeWordDetector _wakeWordDetector;
    private Speech _speech;
    private ChatGpt _gpt;
    public WakeWordDetector WakeWordDetector => _wakeWordDetector;
    public Speech Speech => _speech;
    public ChatGpt Gpt => _gpt;

    public Start()
    {
        _wakeWordDetector = new WakeWordDetector();
        _gpt = new ChatGpt();
        _speech = new Speech();
        
        _speech.Recognizer.RecognitionEndedEvent += RecognizerOnRecognitionEndedEvent;
        _gpt.resultsObtained += GptOnresultsObtained;
    }

    private void GptOnresultsObtained()
    {
        _speech.Announce(_gpt.Result);
    }

    private void RecognizerOnRecognitionEndedEvent()
    {
        _gpt.SendMessage(_speech.Recognizer.Result);
    }

    public void StartListening()
    {
        _wakeWordDetector.MagentaDetected += MagentaDetected;
        _wakeWordDetector.ShutUpDetected += ShutUpDetected;
        _wakeWordDetector.Start();
    }

    private void MagentaDetected()
    {
        _speech.Record();
    }

    private void ShutUpDetected()
    {
    }
}