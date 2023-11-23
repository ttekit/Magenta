using System;

namespace Magenta.Core;

public class HistoryItem
{
    private string _message;
    private string _role;

    public string Role
    {
        get => _role;
        set => _role = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Message
    {
        get => _message;
        set => _message = value ?? throw new ArgumentNullException(nameof(value));
    }

    public override string ToString()
    {
        return $"{_role} : {_message}";
    }
}