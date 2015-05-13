using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.XAudio2;
using SharpDX.Multimedia;
using SharpDX.IO;
using System.IO;

namespace laExternalMulti.Objects
{
    class SoundManager : IDisposable
    {
        #region VARIABLES
        private XAudio2 audio;
        private MasteringVoice masteringVoice;
        private SoundStream[] streams;
        private AudioBuffer[] buffers;
        private SourceVoice[] voices;
        #endregion

        #region CONSTRUCTOR
        public SoundManager(int cntVoices)
        {
            audio = new XAudio2();
            masteringVoice = new MasteringVoice(audio);
            masteringVoice.SetVolume(0.5f);
            voices = new SourceVoice[cntVoices];
            buffers = new AudioBuffer[cntVoices];
            streams = new SoundStream[cntVoices];
        }
        #endregion

        #region METHODS
        public void Add(int index, UnmanagedMemoryStream sourceStream)
        {
            streams[index] = new SoundStream(sourceStream);
            buffers[index] = new AudioBuffer();
            buffers[index].Stream = streams[index].ToDataStream();
            buffers[index].AudioBytes = (int)streams[index].Length;
            buffers[index].Flags = BufferFlags.EndOfStream;
            voices[index] = new SourceVoice(audio, streams[index].Format);
        }
        public void Play(int index)
        {
            voices[index].Stop();
            voices[index].FlushSourceBuffers();
            voices[index].SubmitSourceBuffer(buffers[index], streams[index].DecodedPacketsInfo);
            voices[index].Start();
        }

        public void SetVolume(float val)
        {
            masteringVoice.SetVolume(val);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (audio != null)
            {
                audio.StopEngine();
                audio.Dispose();
                audio = null;
                foreach (SourceVoice voice in voices)
                    voice.Dispose();
                foreach (SoundStream stream in streams)
                    stream.Dispose();
            }
        }
        #endregion
    }
}
