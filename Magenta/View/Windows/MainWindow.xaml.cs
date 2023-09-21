using System.Windows;
using Magenta.Core;
using Magenta.Core.Audio;
using Pv;

namespace Magenta;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{


    Porcupine porcupine;
    private short[] audioFrame;
    
    private readonly AudioRecorder _recorder;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void StartButton_OnClick(object sender, RoutedEventArgs e)
    {
        _recorder.StartRecording();
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        Start.StartListening();
    }
}