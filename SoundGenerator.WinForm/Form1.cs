using System;
using System.Collections.Generic;
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
                var sineWaveProvider = Signal();
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

        private SinWaveProvider32 Signal()
        {
            var sineWaveProvider = new SinWaveProvider32();
            sineWaveProvider.SetWaveFormat(16000, 1); // 16kHz mono
            return sineWaveProvider;
        }        
        
        private SinWaveProvider32 Signal2()
        {
            var sineWaveProvider = new SinWaveProvider32 {Amplitude = 0.5f};
            sineWaveProvider.SetWaveFormat(16000, 1); // 16kHz mono
            return sineWaveProvider;
        }

        private void ConvertImage_Click(object sender, EventArgs e)
        {
            _waveOut = new WaveOut();
            _waveOut.Init(MultiplexingWave());
            _waveOut.Play();
        }

        private MultiplexingWaveProvider32 MultiplexingWave()
        {
            var inputs = new List<IWaveProvider> {Signal(), Signal2()};
            return new MultiplexingWaveProvider32(inputs, 2);
        }
    }
}
