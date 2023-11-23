using System;
using System.Collections.Generic;
using System.IO;
using Magenta.Core.Execution.DataBase;
using Newtonsoft.Json;

namespace Magenta.Core.Execution.Executors;

public class ESP32WordsArray
{
    private const int MaxRetries = 3;
    private const int DelayBetweenRetriesMs = 500;
    public static ESP32WordsArray _instance;
    private WordsArray words;

    private ESP32WordsArray()
    {
        LoadFromFile();
        if (words != null) return;
        // к - еративность
    }

    public static ESP32WordsArray Instance
    {
        get
        {
            if (_instance == null) _instance = new ESP32WordsArray();
            return _instance;
        }
        private set => _instance = value ?? throw new ArgumentNullException(nameof(value));
    }

    public void LoadFromFile()
    {
        if (!File.Exists(Config.Instance.ESP_WORDS_ARRAY_PATH)) return;

        var jsonString = File.ReadAllText(Config.Instance.ESP_WORDS_ARRAY_PATH);
        words = JsonConvert.DeserializeObject<WordsArray>(jsonString);
    }

    public void SaveToFile()
    {
        var jsonString = JsonConvert.SerializeObject(words);
        File.WriteAllText(Config.Instance.ESP_WORDS_ARRAY_PATH, jsonString);
    }

    public void AddItem(EspItem item)
    {
        words.AddWord(item);
    }

    public bool Contains(string s)
    {
        return words.Contains(s);
    }

    public bool GetState(string name)
    {
        return words.GetState(name);
    }

    public void SetState(string name, bool b)
    {
        words.SetState(name, b);
    }

    public EspItem GetEspItem(string command)
    {
        return words.GetEspItemByName(command);
    }

    public IEnumerable<EspItem> GetArray()
    {
        return words.GetArray();
    }

    public void UpdateItem(EspItem item)
    {
        words.UpdateItem(item);
    }
}