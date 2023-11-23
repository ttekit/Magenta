using System.Windows;
using Magenta.Core.Execution.Executors;

namespace Magenta.View.Windows;

public partial class AddEspDivice : Window
{
    public AddEspDivice()
    {
        InitializeComponent();
    }

    private void AddButton_OnClick(object sender, RoutedEventArgs e)
    {
        var name = NameTextBox.Text;
        var offlink = OffLinkTextBox.Text;
        var onlink = OnLinkTextBox.Text;
        var statuslink = StateLinkTextBox.Text;

        var item = new EspItem(name, offlink, onlink, statuslink);
        ESP32WordsArray.Instance.AddItem(item);
        Close();
    }
}