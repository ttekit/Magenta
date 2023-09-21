using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Magenta.Core.Web;

public class WebRequests
{
    private readonly HttpWebRequest _request;
    private string _webLink;

    public WebRequests(string link, HttpWebRequest request)
    {
        _webLink = link;
        _request = request;
    }

    public string execute(JObject data)
    {
        using (var stream = new StreamWriter(_request.GetRequestStream()))
        {
            stream.Write(data.ToString());
        }

        var response = (HttpWebResponse)_request.GetResponse();

        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

        Trace.WriteLine(responseString);
        return responseString;
    }
}