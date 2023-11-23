using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Magenta.Core.Execution.Executors;

public class EspItem
{
    private string _name;
    private string _offlink;
    private string _onlink;
    private string _statelink;

    public EspItem(string name, string offlink, string onlink, string stateLink)
    {
        _name = name;
        _offlink = offlink;
        _onlink = onlink;
        _statelink = stateLink;
        State = false;
    }

    public string Statelink
    {
        get => _statelink;
        set => _statelink = value ?? throw new ArgumentNullException(nameof(value));
    }

    public bool State { get; set; }

    public string Name
    {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Offlink
    {
        get => _offlink;
        set => _offlink = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Onlink
    {
        get => _onlink;
        set => _onlink = value ?? throw new ArgumentNullException(nameof(value));
    }

    public bool ContainsWord(string word)
    {
        return word.Contains(_name);
    }

    public void UpdateState()
    {
        var retryCount = 0;

        while (retryCount < 3)
            try
            {
                var request = WebRequest.Create(_statelink);
                request.Method = "GET";
                var data = "";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        data = new StreamReader(stream).ReadToEnd();
                    }
                }

                if (data != "") State = data.Trim() == "on";

                return;
            }
            catch (Exception e)
            {
                retryCount++;
                Thread.Sleep(100);
            }
    }

    public override string ToString()
    {
        return _name;
    }
}