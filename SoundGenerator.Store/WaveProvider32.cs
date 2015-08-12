using System;
using NAudio;
using NAudio.Wave;

namespace SoundGenerator.Store
{
    public abstract class WaveProvider32 : IWaveProvider
    {
        public WaveProvider32() : this(44100, 1)
        {
        }

        public WaveProvider32(int sampleRate, int channel)
        {
            SetWaveFormat(sampleRate, channel);
        }

        public void SetWaveFormat(int sampleRate, int channel)
        {
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channel);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            var waveBuffer = new WaveBuffer(buffer);
            int samplesRequired = count / 4;
            int samplesRead = Read(waveBuffer.FloatBuffer, offset/4, samplesRequired);
            return samplesRead*4;
        }

        public abstract int Read(float[] buffer, int offset, int count);


        public WaveFormat WaveFormat { get; private set; }
    }
}
