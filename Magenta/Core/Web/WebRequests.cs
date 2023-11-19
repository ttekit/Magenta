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
        if (data != null)
            using (var requestStream = new StreamWriter(_request.GetRequestStream()))
            {
                requestStream.Write(data.ToString());
            }

        using (var response = (HttpWebResponse)_request.GetResponse())
        {
            if (response == null) return "";
            using (var responseStream = new StreamReader(response.GetResponseStream()))
            {
                var responseString = responseStream.ReadToEnd();
                return responseString;
            }
        }
    }
}