using System;
using System.Diagnostics;
using System.Threading.Channels;
using Magenta.Core.Audio;
using Magenta.Core.Execution;
using Magenta.Core.Web;


namespace Magenta.Core;

public class MainApplication
{
    private bool _isWorkingEnded = false;
    private WakeWordDetector _wakeWordDetector;
    private Speech _speech;
    private ChatGpt _gpt;
    private Executor _executor;
    public WakeWordDetector WakeWordDetector => _wakeWordDetector;
    public Speech Speech => _speech;
    public ChatGpt Gpt => _gpt;
    public Executor Executor => _executor;


    public MainApplication()
    {
        _wakeWordDetector = new WakeWordDetector();
        _gpt = new ChatGpt();
        _speech = new Speech();
        _executor = new Executor();

        _speech.Recognizer.RecognitionEndedEvent += RecognizerOnRecognitionEndedEvent;
        _speech.Dubber.AnnounceEnded += DubberOnAnnounceEnded;
        _gpt.resultsObtained += GptOnresultsObtained;

        MainWindow.MediaPlayer.MediaEnded += MediaPlayerOnMediaEnded;
    }

    private void DubberOnAnnounceEnded()
    {
        _isWorkingEnded = true;
    }

    private void MediaPlayerOnMediaEnded(object? sender, EventArgs e)
    {
        if (_isWorkingEnded)
        {
            Trace.WriteLine("CHAIN APPL STARTED");
            this.StartChain();
        }
    }

    private void StartChain()
    {
        if (_isWorkingEnded)
        {
            _speech.Record();
        }
    }

    private void GptOnresultsObtained()
    {
        _speech.Announce(_gpt.Result);
    }

    private void RecognizerOnRecognitionEndedEvent()
    {
        string res = _speech.Recognizer.Result;
        Trace.WriteLine("SIMPLIFY STARTED");
        string simplifyRes = _speech.Simplify(res);
        Trace.WriteLine("SIMPLIFY ENDED");

        Trace.WriteLine("EXECUTION STARTED");
        string resultOfAction = _executor.ExecuteCommand(simplifyRes);
        if (resultOfAction != "")
        {
            res += $" Результат: {resultOfAction}";
        }

        Trace.WriteLine("EXECUTION ENDED");

        Trace.WriteLine("USER ANSWER STARTED");
        _gpt.SendMessage(res);
        Trace.WriteLine("USER ANSWER ENDED");
    }

    public void StartListening()
    {
        _wakeWordDetector.MagentaDetected += MagentaDetected;
        _wakeWordDetector.Start();
    }

    private void MagentaDetected()
    {
        _speech.Record();
    }
}