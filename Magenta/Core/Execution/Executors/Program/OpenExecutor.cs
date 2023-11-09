using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Magenta.Core.Execution.DataBase;

namespace Magenta.Core.Execution.Executors;

public class OpenExecutor : IExecutor
{
    public string Command { get; set; }


    public string Execute()
    {
        Command = Command.Trim();
        Trace.WriteLine("Program name: " + Command);

        var files = new List<FileInfo>();
        var directoryInfo =
            new DirectoryInfo(@"C:\Users\tekit\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\");
        ListFiles(directoryInfo, files);

        var finalCommand = Command;
        files = files.Where(file => file.Name.ToLower().Contains(finalCommand.ToLower())).ToList();

        foreach (var info in files) Trace.WriteLine(info.FullName);

        if (files.Count == 0)
            Start(Command);
        else
            try
            {
                foreach (var file in files)
                {
                    var proc = new Process();
                    proc.StartInfo.FileName = file.FullName;
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                }
            }
            catch (IOException e)
            {
                throw new Exception(e.Message);
            }

        return "успешно открыто";
    }

    private void Start(string prmName)
    {
        var commandTable = new CommandTable();
        var executeFilePath = commandTable.SelectCommandByName(prmName.Split(' ')[0].Trim()).CommandText;
        if (executeFilePath != null)
            try
            {
                Process.Start(executeFilePath);
            }
            catch (IOException e)
            {
                throw new Exception(e.Message);
            }
    }

    private void ListFiles(DirectoryInfo directory, List<FileInfo> files)
    {
        var fileList = directory.GetFiles();
        if (fileList != null)
        {
            foreach (var file in fileList) files.Add(file);

            var subDirectories = directory.GetDirectories();
            foreach (var subDir in subDirectories) ListFiles(new DirectoryInfo(subDir.FullName), files);
        }
    }
}