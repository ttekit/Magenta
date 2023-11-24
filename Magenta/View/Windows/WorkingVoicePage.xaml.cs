using System.Security.Cryptography.Pkcs;
using System.Windows;

namespace Magenta.View.Windows;

public partial class WorkingVoicePage : Window
{
    public int BaseSize => 200;

    public WorkingVoicePage()
    {
        InitializeComponent();
    }

    public void SetSize(int size)
    {
        VoiceCircle.Width = size;
        VoiceCircle.Height = size;
    }
}