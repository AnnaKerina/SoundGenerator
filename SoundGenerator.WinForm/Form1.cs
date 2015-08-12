using System;
using System.Windows.Forms;
using NAudio.Wave;
using SoundGenerator.Store;

namespace SoundGenerator.WinForm
{
    public partial class Form1 : Form
    {
        private WaveOut _waveOut;

        public Form1()
        {
            InitializeComponent();
        }

        private void Play_Click(object sender, EventArgs e)
        {
            StartStopSineWave();
        }

        private void StartStopSineWave()
        {
            if (_waveOut == null)
            {
                var sineWaveProvider = new SinWaveProvider32();
                sineWaveProvider.SetWaveFormat(16000, 1); // 16kHz mono
                sineWaveProvider.Frequency = 1000;
                sineWaveProvider.Amplitude = 0.25f;
                _waveOut = new WaveOut();
                _waveOut.Init(sineWaveProvider);
                _waveOut.Play();
            }
            else
            {
                _waveOut.Stop();
                _waveOut.Dispose();
                _waveOut = null;
            }
        }

        private void Mixer()
        {
            var waveChannel32 = new WaveChannel32[4]; 

        }
    }
}
