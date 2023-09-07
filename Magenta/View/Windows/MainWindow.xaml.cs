using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Magenta.Core.Audio;

namespace Magenta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AudioRecorder _recorder;
        public MainWindow()
        {
            InitializeComponent();
            _recorder = new AudioRecorder();
        }

 

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            _recorder.StopRecording();
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            _recorder.StartRecording();
        }
    }
}