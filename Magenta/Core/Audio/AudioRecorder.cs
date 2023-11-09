using System;
using System.Diagnostics;
using NAudio.Lame;
using NAudio.Wave;

namespace Magenta.Core.Audio;

public class AudioRecorder
{
    public delegate void RecordEndedEvent();

    public delegate void RecordEndStartedEvent();

    public delegate void RecordStartedEvent();

    private LameMP3FileWriter mp3Writer;
    private Stopwatch silenceTimer;
    private Stopwatch stopTimer;
    private WaveFileWriter waveFileWriter;
    private WaveInEvent waveSource;
    public bool isEnded { get; private set; }

    public event RecordStartedEvent recordStarted;
    public event RecordEndedEvent recordEnded;
    public event RecordEndStartedEvent recordEndStarted;

    public void StartRecording()
    {
        if (isEnded) return;
        Trace.WriteLine("Recording started");
        silenceTimer = new Stopwatch();
        stopTimer = new Stopwatch();
        waveSource = new WaveInEvent();
        // waveSource.DeviceNumber = Config.Instance.AudioDeviceIndex;

        waveSource.WaveFormat =
            new WaveFormat(Config.Instance.SAMPLE_RATE, Config.Instance.CHANNELS);

        waveFileWriter =
            new WaveFileWriter(Config.Instance.TempFilesPath + "output.wav", waveSource.WaveFormat);


        waveSource.DataAvailable += WaveSource_DataAvailable;
        waveSource.RecordingStopped += WaveSource_RecordingStopped;


        waveSource.StartRecording();
        stopTimer.Start();
    }

    private void WaveSource_DataAvailable(object sender, WaveInEventArgs e)
    {
        var rms = CalculateRMS(e.Buffer, e.BytesRecorded);
        Trace.WriteLine(rms);

        if (stopTimer.ElapsedMilliseconds >= Config.Instance.STOP_DURATION_MS && stopTimer.IsRunning)
        {
            isEnded = false;
            stopTimer.Stop();
            StopRecording();
        }

        if (rms > Config.Instance.SILENCE_THRESHOLD && !isEnded)
        {
            Trace.WriteLine("Voice Detected");
            stopTimer.Stop();
            isEnded = true;
            recordStarted?.Invoke();
            silenceTimer.Start();
        }

        if (isEnded)
        {
            waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
            if (rms < Config.Instance.SILENCE_THRESHOLD)
            {
                if (silenceTimer.ElapsedMilliseconds >= Config.Instance.SILENCE_DURATION_MS)
                {
                    silenceTimer.Stop();
                    Trace.WriteLine("Silence Detected");
                    StopRecording();
                }
            }
            else
            {
                silenceTimer.Restart();
            }
        }
    }

    public void StopRecording()
    {
        waveSource.StopRecording();
    }

    private void WaveSource_RecordingStopped(object sender, StoppedEventArgs e)
    {
        Trace.WriteLine("RECORDING ENDED");
        waveFileWriter.Close();
        waveFileWriter.Dispose();

        if (isEnded)
        {
            isEnded = false;
            recordEndStarted?.Invoke();
            using (var reader = new WaveFileReader(Config.Instance.TempFilesPath + "output.wav"))
            {
                mp3Writer = new LameMP3FileWriter(Config.Instance.TempFilesPath + "output.mp3", reader.WaveFormat,
                    128);

                reader.CopyTo(mp3Writer);

                mp3Writer.Close();
                mp3Writer.Dispose();
            }

            Trace.WriteLine("RECORDING WRITE TO FILE ENDED");
            recordEnded?.Invoke();
        }
    }

    private float CalculateRMS(byte[] buffer, int bytesRead)
    {
        var sumOfSquares = 0.0f;

        for (var i = 0; i < bytesRead; i += 2)
        {
            var sample = BitConverter.ToInt16(buffer, i);
            var sampleValue = sample / 327.680f;
            sumOfSquares += sampleValue * sampleValue;
        }

        var rms = (float)Math.Sqrt(sumOfSquares / (bytesRead / 2));

        return rms;
    }
}