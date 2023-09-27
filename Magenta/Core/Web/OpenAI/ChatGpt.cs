using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Magenta.Core.Web;

public class ChatGpt
{
    private readonly string settings =
        "Ответь в стиле jarvis к особе мужского пола: ";

    private readonly string apiKey;
    private readonly string historyFileName;
    private readonly JArray messages;

    public string? Result { get; private set; }
    public event ResultsWereObtainedEvent resultsObtained;

    public ChatGpt()
    {
        try
        {
            apiKey = File.ReadAllText(Config.Instance.ApiKeysPath + "openAi.txt");
            messages = new JArray();
            var settingsMessage = new JObject();
            settingsMessage["role"] = "system";
            messages.Add(settingsMessage);
            settingsMessage["content"] = settings;
            historyFileName = DateTime.Now.ToString("yyyy-MM-ddTHH_mm_ss") + ".json";
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public string? SendMessage(string text)
    {
        text += ".";
        var message = new JObject();
        message["role"] = "user";
        message["content"] = text;

        messages.Add(message);
        var url = new Uri("https://api.openai.com/v1/chat/completions");
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Headers["Authorization"] = "Bearer " + apiKey;
        httpWebRequest.Method = "POST";

        var data = new JObject();
        data["model"] = "gpt-3.5-turbo";
        data["messages"] = messages;
       
        var requests = new WebRequests("", httpWebRequest);
        var response = requests.execute(data);
        var content = JObject.Parse(response)["choices"]?[0]?["message"]?["content"]?.ToString();
        messages.Add(JObject.Parse(response)["choices"]?[0]?["message"] ?? throw new InvalidOperationException());
        
        SaveToFile();
        Result = content;
        resultsObtained?.Invoke();
        
        return Result;
    }


    private void SaveToFile()
    {
        try
        {
            var dir = new DirectoryInfo(Config.Instance.HistoryDir);
            var file = new FileInfo(Path.Combine(dir.FullName, historyFileName));


            if (!dir.Exists) dir.Create();

            if (!file.Exists) File.Create(file.FullName).Close();

            File.WriteAllText(file.FullName, messages.ToString());
        }
        catch (IOException e)
        {
            throw new Exception(e.Message);
        }
    }
}

public delegate void ResultsWereObtainedEvent();