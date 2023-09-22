using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using Magenta.Core.Audio;
using NAudio.Wave;
using Pv;

namespace Magenta.Core;

public class Start
{
    private WakeWordDetector _wakeWordDetector;
    private Speech _speech;

    public WakeWordDetector WakeWordDetector => _wakeWordDetector;

    public Speech Speech => _speech;

    public Start()
    {
        _wakeWordDetector = new WakeWordDetector();
        _speech = new Speech();

    }
    
    public void StartListening()
    {
        _wakeWordDetector.MagentaDetected += MagentaDetected;
        _wakeWordDetector.ShutUpDetected += ShutUpDetected;
        _wakeWordDetector.Start();
    }

    private void MagentaDetected()
    {
        _speech.Record();
    }

    private void ShutUpDetected()
    {
        
    }
}