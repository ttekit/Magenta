using System;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace Magenta.Core.Execution.Executors;

public class GoogleExecutor : IExecutor
{
    public string Command { get; set; }

    public string Execute()
    {
        try
        {
            var encodedQuery = HttpUtility.UrlEncode(Command, Encoding.UTF8);
            var apiUrl = "https://www.google.com/search?q=" + encodedQuery;

            Process.Start(apiUrl);
            return "успешно загуглено";
        }
        catch (Exception e)
        {
            throw new ApplicationException("An error occurred while trying to open the URL.", e);
        }
    }
}