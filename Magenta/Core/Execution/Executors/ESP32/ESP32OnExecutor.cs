using System;
using System.Diagnostics;
using System.Net;

namespace Magenta.Core.Execution.Executors;

public class ESP32OnExecutor : IExecutor
{
    public string Command { get; set; }

    public string Execute()
    {
        var urlBase = "http://192.168.1.130/";
        Trace.WriteLine("Started turn on");
        Command = Command.Trim();

        if (ESP32WordsArray.Instance.Contains(Command))
        {
            Trace.WriteLine(Command);
            ESP32WordsArray.Instance.UpdateStates();
            if (ESP32WordsArray.Instance.GetState(Command))
            {
                var id = ESP32WordsArray.Instance.GetWordId(Command);
                Trace.WriteLine("State: " + ESP32WordsArray.Instance.GetState(Command));
                Trace.WriteLine("Word is detected");

                urlBase += ESP32WordsArray.Instance.GetIdAbbr(Command);
                urlBase += "/L";

                sendRequest(urlBase);

                ESP32WordsArray.Instance.SetState(id, true);
                return "Устройство успешно включено";
            }

            return "Не вышло включить устройство, оно уже было включено";
        }

        return "Не вышло включить устройство";
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