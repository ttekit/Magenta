using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using NAudio.Wave;
using Pv;

namespace Magenta.Core;

public class Start
{
    public static void StartListening()
    {
        WakeWordDetector wakeWordDetector = new WakeWordDetector();
        wakeWordDetector.MagentaDetected += () =>
        {
            MessageBox.Show($"[{DateTime.Now.ToLongTimeString()}] Detected Magenta");
        };
        wakeWordDetector.ShutUpDetected += () =>
        {
            MessageBox.Show($"[{DateTime.Now.ToLongTimeString()}] Detected Shut Up");
        };
        wakeWordDetector.Start();
    }
}