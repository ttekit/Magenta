using System;
using System.Diagnostics;

namespace Magenta.Core.Execution.Executors;

public class ESP32OnExecutor : IExecutor
{
    public string Command { get; set; }

    public string Execute()
    {
        // string urlBase = "http://192.168.1.130/";
        // Trace.WriteLine("Started turn on");
        // Command = Command.Trim();
        //
        // if (ESP32WordsArray.Instance.Contains(Command))
        // {
        //     int id =  ESP32WordsArray.Instance.GetWordIndex(Command);
        //     Trace.WriteLine(Command);
        //
        //     if ( ESP32WordsArray.Instance.GetState(id))
        //     {
        //         Trace.WriteLine("State: " +  ESP32WordsArray.Instance.GetState(id));
        //         Trace.WriteLine("Word is detected");
        //         
        //         urlBase +=  ESP32WordsArray.Instance.GetIdAbbr(id);
        //         urlBase += "/L";
        //         WebExecutor.Instance.SendGetTextHtmlRequest(new Uri(urlBase));
        //         
        //         words.SetState(id, true);
        //         return "Устройство успешно включено";
        //     }
        //
        //     return "Не вышло включить устройство, оно уже было включено";
        // }
        //
        // return "Не вышло включить устройство";
        return "";
    }
}