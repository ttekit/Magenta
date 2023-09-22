using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Magenta.Core;
using Magenta.Core.Audio;
using Pv;

namespace Magenta;

/// <summary>
///     Interaction magical logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Start _start;

    public MainWindow()
    {
        InitializeComponent();
        _start = new Start();
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        _start.StartListening();


        _start.Speech.Recognizer.RecognitionEndedEvent += () =>
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RecResulTextBlock.Text = _start.Speech.Recognizer.Result;
                });
            });
        };
    }
}