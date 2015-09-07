using System;
using System.Collections.Generic;
using NAudio.Utils;
using NAudio.Wave;

namespace SoundGenerator.Store
{
    public class MultiplexingWaveProvider32 : MultiplexingWaveProvider
    {
        private readonly IList<IWaveProvider> inputs;
        private readonly WaveFormat waveFormat;
        private readonly int outputChannelCount;
        private readonly int inputChannelCount;
        private readonly List<int> mappings;
        private readonly int bytesPerSample;
        private byte[] inputBuffer;

        public MultiplexingWaveProvider32(IEnumerable<IWaveProvider> inputs, int numberOfOutputChannels)
            : base(inputs, numberOfOutputChannels)
        {
            this.inputs = new List<IWaveProvider>(inputs);
            this.outputChannelCount = numberOfOutputChannels;

            if (this.inputs.Count == 0)
            {
                throw new ArgumentException("You must provide at least one input");
            }
            if (numberOfOutputChannels < 1)
            {
                throw new ArgumentException("You must provide at least one output");
            }
            foreach (var input in this.inputs)
            {
                if (this.waveFormat == null)
                {
                    if (input.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
                    {
                        this.waveFormat = new WaveFormat(input.WaveFormat.SampleRate, input.WaveFormat.BitsPerSample,
                            numberOfOutputChannels);
                    }
                    else if (input.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
                    {
                        this.waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(input.WaveFormat.SampleRate,
                            numberOfOutputChannels);
                    }
                    else
                    {
                        throw new ArgumentException("Only PCM and 32 bit float are supported");
                    }
                }
                else
                {
                    if (input.WaveFormat.BitsPerSample != this.waveFormat.BitsPerSample)
                    {
                        throw new ArgumentException("All inputs must have the same bit depth");
                    }
                    if (input.WaveFormat.SampleRate != this.waveFormat.SampleRate)
                    {
                        throw new ArgumentException("All inputs must have the same sample rate");
                    }
                }
                inputChannelCount += input.WaveFormat.Channels;
            }
            this.bytesPerSample = this.waveFormat.BitsPerSample/8;

            var mappings = new List<int>();
            for (int n = 0; n < outputChannelCount; n++)
            {
                mappings.Add(n%inputChannelCount);
            }
        }


        public new int Read(byte[] buffer, int offset, int count)
        {
            var outputBytesPerFrame = bytesPerSample*outputChannelCount;
            var sampleFramesRequested = count/outputBytesPerFrame;
            var inputOffset = 0;
            var sampleFramesRead = 0;
            // now we must read from all inputs, even if we don't need their data, so they stay in sync
            foreach (var input in inputs)
            {
                var inputBytesPerFrame = bytesPerSample*input.WaveFormat.Channels;
                var bytesRequired = sampleFramesRequested*inputBytesPerFrame;
                inputBuffer = BufferHelpers.Ensure(this.inputBuffer, bytesRequired);
                var bytesRead = input.Read(inputBuffer, 0, bytesRequired);
                sampleFramesRead = Math.Max(sampleFramesRead, bytesRead/inputBytesPerFrame);

                for (var n = 0; n < input.WaveFormat.Channels; n++)
                {
                    var inputIndex = inputOffset + n;
                    for (var outputIndex = 0; outputIndex < outputChannelCount; outputIndex++)
                    {
                        if (mappings[outputIndex] == inputIndex)
                        {
                            var inputBufferOffset = n*bytesPerSample;
                            var outputBufferOffset = offset + outputIndex*bytesPerSample;
                            int sample = 0;
                            while (sample < sampleFramesRequested && inputBufferOffset < bytesRead)
                            {
                                Array.Copy(inputBuffer, inputBufferOffset, buffer, outputBufferOffset, bytesPerSample);
                                outputBufferOffset += outputBytesPerFrame;
                                inputBufferOffset += inputBytesPerFrame;
                                sample++;
                            }
                            // clear the end
                            while (sample < sampleFramesRequested)
                            {
                                Array.Clear(buffer, outputBufferOffset, bytesPerSample);
                                outputBufferOffset += outputBytesPerFrame;
                                sample++;
                            }
                        }
                    }
                }
                inputOffset += input.WaveFormat.Channels;
            }

            return sampleFramesRead*outputBytesPerFrame;
        }

        /// <summary>
        /// The WaveFormat of this WaveProvider
        /// </summary>
        public new WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public new void ConnectInputToOutput(int inputChannel, int outputChannel)
        {
            if (inputChannel < 0 || inputChannel >= InputChannelCount)
            {
                throw new ArgumentException("Invalid input channel");
            }
            if (outputChannel < 0 || outputChannel >= OutputChannelCount)
            {
                throw new ArgumentException("Invalid output channel");
            }
            mappings[outputChannel] = inputChannel;
        }

        /// <summary>
        /// The number of input channels. Note that this is not the same as the number of input wave providers. If you pass in
        /// one stereo and one mono input provider, the number of input channels is three.
        /// </summary>
        public new int InputChannelCount
        {
            get { return inputChannelCount; }
        }

        /// <summary>
        /// The number of output channels, as specified in the constructor.
        /// </summary>
        public new int OutputChannelCount
        {
            get { return outputChannelCount; }
        }
    }
}