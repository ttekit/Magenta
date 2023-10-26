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

        List<FileInfo> files = new List<FileInfo>();
        DirectoryInfo directoryInfo =
            new DirectoryInfo(@"C:\Users\tekit\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\");
        ListFiles(directoryInfo, files);

        string finalCommand = Command;
        files = files.Where(file => file.Name.ToLower().Contains(finalCommand.ToLower())).ToList();

        foreach (var info in files)
        {
            Trace.WriteLine(info.FullName);
        }

        if (files.Count == 0)
        {
            Start(Command);
        }
        else
        {
            try
            {
                foreach (FileInfo file in files)
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = file.FullName;
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                }
            }
            catch (IOException e)
            {
                throw new Exception(e.Message);
            }
        }

        return "успешно открыто";
    }

    private void Start(string prmName)
    {
        CommandTable commandTable = new CommandTable();
        string executeFilePath = commandTable.SelectCommandByName(prmName.Split(' ')[0].Trim()).CommandText;
        if (executeFilePath != null)
        {
            try
            {
                Process.Start(executeFilePath);
            }
            catch (IOException e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    private void ListFiles(DirectoryInfo directory, List<FileInfo> files)
    {
        FileInfo[] fileList = directory.GetFiles();
        if (fileList != null)
        {
            foreach (FileInfo file in fileList)
            {
                files.Add(file);
            }

            DirectoryInfo[] subDirectories = directory.GetDirectories();
            foreach (DirectoryInfo subDir in subDirectories)
            {
                ListFiles(new DirectoryInfo(subDir.FullName), files);
            }
        }
    }
}