using System;
using System.Diagnostics;
using System.Net;

namespace Magenta.Core.Execution.Executors;

public class ESP32OffExecutor : IExecutor
{
    public string Command { get; set; }

    public string Execute()
    {
        Trace.WriteLine("Started turn off");
        Command = Command.Trim();

        var item = ESP32WordsArray.Instance.GetEspItem(Command);
        if (item == null) return "Не вышло выключить устройство";

        item.UpdateState();
        Trace.WriteLine("State 1: " + item.State);
        if (item.State)
        {
            Trace.WriteLine(Command);

            Trace.WriteLine("State: " + ESP32WordsArray.Instance.GetState(Command));
            Trace.WriteLine("Word is detected");
            sendRequest(item.Offlink);
            item.State = true;
            return "Устройство успешно выключено";
        }

        return "устройство уже было выключено";
    }

    private void sendRequest(string q)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(q);

            request.Method = "GET";

            request.GetResponse();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}