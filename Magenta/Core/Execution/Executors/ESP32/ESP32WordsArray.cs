using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Xml;
using Magenta.Core.Execution.DataBase;
using Magenta.Core.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Magenta.Core.Execution.Executors;

public class ESP32WordsArray
{
    public static ESP32WordsArray _instance;
    private readonly WordsArray words;

    private const int MaxRetries = 3;
    private const int DelayBetweenRetriesMs = 500;

    public static ESP32WordsArray Instance
    {
        get
        {
            if (_instance == null) _instance = new ESP32WordsArray();
            return _instance;
        }
        private set => _instance = value ?? throw new ArgumentNullException(nameof(value));
    }

    private ESP32WordsArray()
    {
        words = new WordsArray();
        words.AddWord("чайник");
        words.AddWord("свет");
        words.AddWord("камин");
        GetStateArray();
    }

    public bool Contains(string s)
    {
        return words.Contains(s);
    }

    public string GetIdAbbr(int id)
    {
        return id + 1 switch
        {
            1 => "PO",
            2 => "PT",
            3 => "PF",
            4 => "PFR",
            _ => ""
        };
    }

    public string GetIdAbbr(string word)
    {
        int id = GetWordId(word);
        switch (id + 1)
        {
            case 1: return "PO";
            case 2: return "PT";
            case 3: return "PF";
            case 4: return "PFR";
        }

        return "";
    }

    public int GetWordId(string word)
    {
        return words.GetWordIndex(word);
    }

    public bool GetState(string name)
    {
        return words.GetState(name);
    }

    private void GetStateArray()
    {
        int retryCount = 0;

        while (retryCount < MaxRetries)
        {
            try
            {
                WebRequest request = WebRequest.Create("http://192.168.1.130/pins");
                request.Method = "GET";
                string data = "";
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        data = new StreamReader(stream).ReadToEnd();
                    }
                }

                if (data != "")
                {
                    List<Dictionary<string, int>>
                        jsonArray = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(data);
                    for (var i = 0; i < jsonArray.Count; i++)
                    {
                        words.SetState(i, jsonArray[i]["state"].ToString() == "1");
                    }
                }
                return;
            }
            catch (Exception e)
            {
                retryCount++;
                Thread.Sleep(DelayBetweenRetriesMs);
            }
        }
    }

    public void SetState(int id, bool b)
    {
        words.SetState(id, b);
    }

    public void UpdateStates()
    {
        GetStateArray();
    }
}