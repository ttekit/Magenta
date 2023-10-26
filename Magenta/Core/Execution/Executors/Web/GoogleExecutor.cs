using System;
using System.Text;

namespace Magenta.Core.Execution.Executors;

public class GoogleExecutor : IExecutor
{
    public string Command { get; set; }

    public string Execute()
    {
        try
        {
            string encodedQuery = System.Web.HttpUtility.UrlEncode(Command, Encoding.UTF8);
            string apiUrl = "https://www.google.com/search?q=" + encodedQuery;

            System.Diagnostics.Process.Start(apiUrl);
            return "успешно загуглено";
        }
        catch (Exception e)
        {
            throw new ApplicationException("An error occurred while trying to open the URL.", e);
        }
    }
}