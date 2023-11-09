namespace Magenta.Core.Execution.DataBase;

public class Command
{
    public Command(int id, string name, string commandText)
    {
        Id = id;
        Name = name;
        CommandText = commandText;
    }

    public Command()
    {
        Id = 0;
        Name = null;
        CommandText = null;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string CommandText { get; set; }

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }

    public string GetCommandText()
    {
        return CommandText;
    }

    public override string ToString()
    {
        return $"Command{{ Id={Id}, Name='{Name}', CommandText='{CommandText}' }}";
    }
}