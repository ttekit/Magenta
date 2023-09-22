using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Magenta.Core.Web;

public class Simplifier
{
    private const string LINK = "https://api.openai.com/v1/completions";
    private readonly string _apiKey;
    private readonly WebRequests _webRequest;
    public string Result { get; private set; }

    public Simplifier()
    {
        _apiKey = File.ReadAllText(Config.Instance.ApiKeysPath + "openAi.txt");

        var request = (HttpWebRequest)WebRequest.Create(LINK);
        request.Method = "POST";
        request.Headers.Add("Authorization", "Bearer " + _apiKey);
        request.ContentType = "application/json";

        _webRequest = new WebRequests(LINK, request);
    }

    public string Simplify(string text)
    {
        var data = new JObject();
        data["model"] = "text-davinci-002";
        data["prompt"] = text;
        data["max_tokens"] = 1200;
        data["temperature"] = 0f;
        Result = _webRequest.execute(data);
        return Result;
    }
}