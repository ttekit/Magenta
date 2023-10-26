using System.Collections.Generic;

namespace Magenta.Core.Execution.DataBase;

public class WordsArray
{
    private List<string> words;
    private List<bool> state;

    public WordsArray()
    {
        this.words = new List<string>();
        this.state = new List<bool>();
    }

    public void AddWord(string word)
    {
        words.Add(word);
        state.Add(false);
    }

    public int GetWordIndex(string word)
    {
        for (int i = 0; i < words.Count; i++)
        {
            if (word.Contains(words[i]))
            {
                return i;
            }
        }

        return -1;
    }

    public bool Contains(string word)
    {
        foreach (string s in words)
        {
            if (word.Contains(s))
            {
                return true;
            }
        }

        return false;
    }

    public List<string> GetWords()
    {
        return words;
    }

    public void SetState(int id, bool state)
    {
        this.state[id] = state;
    }

    public bool GetState(int id)
    {
        return this.state[id];
    }
}