namespace Magenta.Core.Execution;

public interface IExecutor
{
    public string Command { get; set; }
    public string Execute();
}