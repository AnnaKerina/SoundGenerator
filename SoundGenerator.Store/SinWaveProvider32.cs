using System;

namespace SoundGenerator.Store
{
    public class SinWaveProvider32 : WaveProvider32
    {
        private int sample;

        public SinWaveProvider32()
        {
            Frequency = 1000;
            Amplitude = 0.25f;
        }

        public float Amplitude { get; set; }

        public float Frequency { get; set; }

        public override int Read(float[] buffer, int offset, int count)
        {
            var sampleRate = WaveFormat.SampleRate;
            for (var i = 0; i < count; i++)
            {
                buffer[i + offset] = (float) (Amplitude*Math.Sin((2*Math.PI*sample*Frequency)/sampleRate));
                sample++;
                if (sample >= sampleRate)
                {
                    sample = 0;
                }
            }
            return count;
        }
    }
}