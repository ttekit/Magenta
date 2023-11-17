using System;
using System.Diagnostics;
using System.Net;

namespace Magenta.Core.Execution.Executors;

public class ESP32OffExecutor : IExecutor
{
    public string Command { get; set; }

    public string Execute()
    {
        string urlBase = "http://192.168.1.130/";
        Trace.WriteLine("Started turn off");
        Command = Command.Trim();

        if (ESP32WordsArray.Instance.Contains(Command))
        {
            Trace.WriteLine(Command);
            ESP32WordsArray.Instance.UpdateStates();
            if (!ESP32WordsArray.Instance.GetState(Command))
            {
                int id = ESP32WordsArray.Instance.GetWordId(Command);
                Trace.WriteLine("State: " + ESP32WordsArray.Instance.GetState(Command));
                Trace.WriteLine("Word is detected");

                urlBase += ESP32WordsArray.Instance.GetIdAbbr(Command);
                urlBase += "/H";

                sendRequest(urlBase);

                ESP32WordsArray.Instance.SetState(id, true);
                return "Устройство успешно выключено";
            }

            return "Не вышло выключить устройство, оно уже было выключено";
        }

        return "Не вышло выключить устройство";
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