using System;
using System.Collections.Generic;
using System.Net;
using Magenta.Core.Execution.DataBase;
using Magenta.Core.Web;
using Newtonsoft.Json.Linq;

namespace Magenta.Core.Execution.Executors;

public class ESP32WordsArray
{
    private readonly WordsArray words;
    public static ESP32OffExecutor Instance;

    static ESP32WordsArray()
    {
        Instance = new ESP32OffExecutor();
    }

    private ESP32WordsArray()
    {
        words = new WordsArray();
        words.AddWord("чайник");
        words.AddWord("свет");
        words.AddWord("камин");
        words.AddWord("что-то ещё");
        GetStateArray();
    }

    public string GetIdAbbr(int id)
    {
        return id + 1 switch
        {
            1 => "PO",
            2 => "PT",
            3 => "PF",
            4 => "PFR",
            _ => "",
        };
    }

    private void GetStateArray()
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://192.168.1.130/pins");
            request.Method = "GET";
            request.ContentType = "application/json";

            WebRequests webRequests = new WebRequests("http://192.168.1.130/pins", request);
            var response = webRequests.execute(new JObject());
            var jsonArray = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(response);
            for (int i = 0; i < jsonArray.Count; i++)
            {
                words.SetState(i, jsonArray[i]["state"] == 1);
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}