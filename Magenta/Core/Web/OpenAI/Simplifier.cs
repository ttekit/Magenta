using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Magenta.Core.Web;

public class Simplifier
{
    private const string LINK = "https://api.openai.com/v1/completions";
    private readonly string _apiKey;

    private readonly string _settingsPromt =
        "Сократи следующую фразу, оставив только необходимую команду и детали к ней: ";

    private HttpWebRequest _request;

    private WebRequests _webRequest;

    public Simplifier()
    {
        _apiKey = File.ReadAllText(Config.Instance.ApiKeysPath + "openAi.txt");
    }

    public string Result { get; private set; }

    public string Simplify(string text)
    {
        var data = new JObject();
        data["model"] = "gpt-3.5-turbo-instruct";
        data["prompt"] = _settingsPromt + text;
        data["max_tokens"] = 1200;
        data["temperature"] = 0f;


        _request = (HttpWebRequest)WebRequest.Create(LINK);
        _request.Method = "POST";
        _request.Headers.Add("Authorization", "Bearer " + _apiKey);
        _request.ContentType = "application/json";
        _webRequest = new WebRequests(LINK, _request);
        var jsonObject = JObject.Parse(_webRequest.execute(data));

        Result = (string)jsonObject["choices"][0]["text"];

        return Result;
    }
}