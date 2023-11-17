using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Magenta.Core.Execution.Executors;

namespace Magenta.Core.Execution;

public class Executor
{
    private readonly Dictionary<string, IExecutor> _executors;
    private string _command;

    public Executor()
    {
        _executors = new Dictionary<string, IExecutor>();
        _executors["открой"] = new OpenExecutor();
        _executors["открыть"] = new OpenExecutor();
        _executors["закрой"] = new CloseExecutor();
        _executors["закрыть"] = new CloseExecutor();
        _executors["выключить"] = new ESP32OffExecutor();
        _executors["выключи"] = new ESP32OffExecutor();
        _executors["включи"] = new ESP32OnExecutor();
        _executors["включить"] = new ESP32OnExecutor();
        _executors["гугл"] = new GoogleExecutor();
        _executors["загуглить"] = new GoogleExecutor();
        _executors["youtube"] = new YoutubeExecutor();
    }

    public string ExecuteCommand(string command)
    {
        Trace.WriteLine("Started command execution: " + command);
        command = command.Replace("[^A-Za-zА-Яа-я0-9]", " ");
        var commandWords = command.ToLower().Split(" ");
        var choose = "";
        var stringBuilder = new StringBuilder();

        foreach (var commandWord in commandWords)
        {
            if (_executors.ContainsKey(commandWord.Trim())) choose = commandWord.Trim();
            stringBuilder.Append(commandWord).Append(" ");
        }

        Trace.WriteLine("Split string: " + stringBuilder);
        Trace.WriteLine("Choose: " + choose);

        return executeAction(choose, stringBuilder.ToString());
    }

    private string executeAction(string action, string command)
    {
        foreach (var value in _executors.Keys)
            if (Equals(value, action))
            {
                IExecutor executor;
                _executors.TryGetValue(action, out executor);
                Trace.WriteLine(executor + " " + value);
                executor.Command = command.Split(action)[1];
                Trace.WriteLine("Current command details: " + executor.Command);
                return executor.Execute();
            }

        return "";
    }
}