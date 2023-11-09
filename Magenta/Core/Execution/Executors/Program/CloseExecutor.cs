using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Magenta.Core.Execution.DataBase;

namespace Magenta.Core.Execution.Executors;

public class CloseExecutor : IExecutor
{
    public string Command { get; set; }

    public string Execute()
    {
        Trace.WriteLine("Enter data: " + Command);
        var commandTable = new CommandTable();
        var closeFilePath = commandTable.SelectCommandByName(Command.Trim());

        string[] split;
        if (closeFilePath.CommandText != null)
            split = closeFilePath.CommandText.Split('/');
        else
            split = Command.Split('/');

        var stringBuilder = new StringBuilder();
        for (var i = 0; i < split.Length; i++)
            if (split.Length - i < 3)
            {
                stringBuilder.Append(split[i]);
                stringBuilder.Append(" ");
            }

        Trace.WriteLine("SB: " + stringBuilder);
        Command = stringBuilder.ToString().Trim();
        Trace.WriteLine("Command: " + Command);

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "tasklist",
                    Arguments = "/fo csv /nh /fi \"STATUS eq RUNNING\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string line;

            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                var columns = line.Split(',');
                if (columns[0].ToLower().Contains(Command))
                {
                    Process.Start("taskkill", "/F /IM " + columns[0]);
                    return "успешно закрыто";
                }
            }
        }
        catch (IOException e)
        {
            throw new Exception(e.Message);
        }

        return "не удалось закрыть";
    }
}