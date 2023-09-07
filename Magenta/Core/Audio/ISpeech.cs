using System.IO;

namespace Magenta.Core.Audio;

public interface ISpeech
{
    void Record();

    string Recognition(string recordPath);

    string Analize(string text);
    
    string Simplify(string text);
}