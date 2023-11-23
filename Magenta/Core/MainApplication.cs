using System;
using System.Diagnostics;
using Magenta.Core.Audio;
using Magenta.Core.Execution;
using Magenta.Core.Web;

namespace Magenta.Core;

public class MainApplication
{
    public MainApplication()
    {
        WakeWordDetector = new WakeWordDetector();
        Gpt = new ChatGpt();
        Speech = new Speech();
        Executor = new Executor();

        Speech.Recognizer.RecognitionEndedEvent += RecognizerOnRecognitionEndedEvent;
        Speech.Dubber.AnnounceEnded += DubberOnAnnounceEnded;
        Gpt.resultsObtained += GptOnresultsObtained;
        MainWindow._mediaPlayer.MediaEnded += MediaPlayerOnMediaEnded;

        Speech.Recorder.recordStarted += () => IsWorkingEnded = false;
    }

    public bool IsWorkingEnded { get; private set; } = true;

    public WakeWordDetector WakeWordDetector { get; }

    public Speech Speech { get; }

    public ChatGpt Gpt { get; }

    public Executor Executor { get; }

    private void DubberOnAnnounceEnded()
    {
        IsWorkingEnded = true;
    }

    private void MediaPlayerOnMediaEnded(object? sender, EventArgs e)
    {
        if (IsWorkingEnded)
        {
            Trace.WriteLine("CHAIN APPL STARTED");
            StartChain();
        }
    }

    private void StartChain()
    {
        if (IsWorkingEnded) Speech.Record();
    }

    private void GptOnresultsObtained()
    {
        Trace.WriteLine("RESULTS OBTAINED");
        Speech.Announce(Gpt.Result);
    }

    private void RecognizerOnRecognitionEndedEvent()
    {
        var res = Speech.Recognizer.Result;
        Trace.WriteLine("SIMPLIFY STARTED");
        var simplifyRes = Speech.Simplify(res);
        Trace.WriteLine("SIMPLIFY ENDED");

        Trace.WriteLine("EXECUTION STARTED");
        var resultOfAction = Executor.ExecuteCommand(simplifyRes);
        if (resultOfAction != "") res += $" Результат: {resultOfAction}";

        Trace.WriteLine("EXECUTION ENDED");

        Trace.WriteLine("USER ANSWER STARTED");
        Gpt.SendMessage(res);
        Trace.WriteLine("USER ANSWER ENDED");
    }

    public void StartListening()
    {
        WakeWordDetector.MagentaDetected += MagentaDetected;
        WakeWordDetector.Start();
    }

    private void MagentaDetected()
    {
        if (!IsWorkingEnded) Speech.Record();
    }
}