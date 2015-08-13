using System;

namespace SoundGenerator.Store
{
    public class SinWaveProvider32 : WaveProvider32
    {
        private int sample;

        public SinWaveProvider32()
        {
            Frequency = 300;
            Amplitude = 0.25f;
        }

        public float Amplitude { get; set; }

        public float Frequency { get; set; }


        public override int Read(float[] buffer, int offset, int count)
        {
            var sampleRate = WaveFormat.SampleRate;
            for (var i = 0; i < count; i++)
            {
                buffer[i + offset] = CreateSignalsLine(sampleRate);
                sample++;
                if (sample >= sampleRate)
                {
                    sample = 0;
                }
            }
            return count;
        }

        private float Signal(int sampleRate)
        {
            return (float)(Amplitude * Math.Sin((2 * Math.PI * sample * Frequency) / sampleRate));
        }

        private float CreateSignalsLine(int sampleRate)
        {
            float returnedSignal = 0;
            for (var i = 0; i < 300; i++)
            {
                Amplitude = GetAmplitude(255);
                Frequency = GetFrequency(i);
                var signalG = (float) (Amplitude*Math.Sin((2*Math.PI*sample*Frequency)/sampleRate));
                returnedSignal += signalG;
             }
             return returnedSignal;
        }

        public float GetAmplitude(int color)
        {
            return color/500f;
        }

        public float GetFrequency(int pixelPosition)
        {
            return pixelPosition * 16 + 200f;
        }
    }
}