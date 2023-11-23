using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Magenta.Core.Web;

public class ChatGpt
{
    private readonly string apiKey;

    private string historyFileName;
    private JArray messages;

    public ChatGpt()
    {
        try
        {
            apiKey = File.ReadAllText(Config.Instance.API_KEYS_PATH + "openAi.txt");
            messages = new JArray();
            var settingsMessage = new JObject();
            settingsMessage["role"] = "system";
            messages.Add(settingsMessage);
            settingsMessage["content"] = AnswerSettings.Instance.formatPromt();
            historyFileName = DateTime.Now.ToString("yyyy-MM-ddTHH_mm_ss") + ".json";
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public string? Result { get; private set; }
    public event ResultsWereObtainedEvent resultsObtained;

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

        Trace.WriteLine(content);

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
            var dir = new DirectoryInfo(Config.Instance.HISTORY_DIR);
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

    public void SetHistory(string selectedHistory)
    {
        historyFileName = selectedHistory;
        messages = JArray.Parse(File.ReadAllText(historyFileName));
    }

    public JArray GetHistory()
    {
        return messages;
    }
}

public delegate void ResultsWereObtainedEvent();