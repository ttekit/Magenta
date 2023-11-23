using System.Windows;
using Magenta.Core.Execution.Executors;

namespace Magenta.View.Windows;

public partial class EditEspDivice : Window
{
    private readonly EspItem _item;

    public EditEspDivice(EspItem item)
    {
        InitializeComponent();
        _item = item;
        NameTextBox.Text = item.Name;
        OnLinkTextBox.Text = item.Onlink;
        OffLinkTextBox.Text = item.Offlink;
        StateLinkTextBox.Text = item.Statelink;
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        ESP32WordsArray.Instance.UpdateItem(_item);
        Close();
    }
}