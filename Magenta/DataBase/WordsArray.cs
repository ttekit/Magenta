using System;
using System.Collections.Generic;
using Magenta.Core.Execution.Executors;

namespace Magenta.Core.Execution.DataBase;

public class WordsArray
{
    private List<EspItem> data;

    public WordsArray()
    {
        data = new List<EspItem>();
    }


    public List<EspItem> Data
    {
        get => data;
        private set => data = value ?? throw new ArgumentNullException(nameof(value));
    }

    public void AddWord(EspItem word)
    {
        data.Add(word);
    }


    public bool Contains(string word)
    {
        foreach (var s in data)
            if (s.ContainsWord(word))
                return true;

        return false;
    }

    public void SetState(EspItem word, bool state)
    {
        if (data.Contains(word))
            word.State = state;
    }

    public EspItem GetEspItemByName(string name)
    {
        foreach (var item in data)
            if (name.Contains(item.Name))
                return item;

        return null;
    }

    public void SetState(string name, bool state)
    {
        var item = GetEspItemByName(name);
        if (item == null) return;
        item.State = state;
    }

    public bool GetState(string name)
    {
        var item = GetEspItemByName(name);
        if (item == null) return false;
        return item.State;
    }

    public IEnumerable<EspItem> GetArray()
    {
        return data;
    }

    public void UpdateItem(EspItem item)
    {
        for (var i = 0; i < data.Count; i++)
            if (data[i].Name.Contains(item.Name))
            {
                data[i] = item;
                return;
            }

        data.Add(item);
    }
}