/// 1.5 Revision: OK
using Android.Media;
using System;
using System.IO;
using System.Threading;

namespace Skyfall
{
    public static class AudioRecorder
    {
        private static readonly int samplingRate = 44100;
        private static readonly ChannelIn channelIn = ChannelIn.Mono;
        private static readonly Encoding encoding = Encoding.Pcm16bit;
        private static readonly int bufferSize = AudioRecord.GetMinBufferSize(samplingRate, channelIn, encoding);
        private static byte[] audioSample = new byte[bufferSize];
        private static readonly AudioRecord audioRecord = new AudioRecord(AudioSource.VoiceRecognition, samplingRate, channelIn, encoding, bufferSize);
        private static string currentAudioFile = string.Empty;

        public static void Start()
        {
            if (audioRecord.RecordingState == RecordState.Recording) return;
            try
            {
                audioRecord.StartRecording();

                Thread thread = new Thread(WriteToDisk);
                thread.Start();
            }
            catch (Exception e) { _ = Kernel.Beep(Tone.CdmaSoftErrorLite); Disk.LogException(e); }
        }

        public static void Stop()
        {
            if (audioRecord.RecordingState == RecordState.Stopped) return;
            try
            {
                audioRecord.Stop();
            }
            catch (Exception e) { _ = Kernel.Beep(Tone.CdmaSoftErrorLite); Disk.LogException(e); }
        }

        public static void Schedule()
        {
            DateTime now = DateTime.Now;
            if (now.Hour == 0 && now.Minute == 0) { Stop(); Start(); } else Start();
        }

        private static void WriteToDisk()
        {
            try
            {
                /// Create new audio file
                currentAudioFile = Disk.MEDIA_AUDIO_DIRECTORY + "/" + SettingDriver.GetFormattedDateTime() + Disk.MEDIA_AUDIO_FILE_EXTENSION;
                using (FileStream fileStream = new FileStream(currentAudioFile, FileMode.Create))
                {
                    /// PCM sample data flushing loop
                    while (audioRecord.RecordingState == RecordState.Recording)
                    {
                        audioRecord.Read(audioSample, 0, bufferSize);
                        try { fileStream.Write(audioSample, 0, bufferSize); fileStream.Flush(); } catch { } // Ignore errors; this will create gaps in audio file for each error, but prevent full session failure;

                        if (fileStream.Length >= Disk.MEDIA_AUDIO_MAX_SIZE)
                        {
                            fileStream.Close();
                            WriteWavHeader(samplingRate, 16, 1);
                            WriteToDisk();
                        }
                    }

                    /// After loop finished (RecordState.Stopped)
                    fileStream.Close();
                    WriteWavHeader(samplingRate, 16, 1);
                }
            }
            catch (Exception e) { _ = Kernel.Beep(Tone.CdmaSoftErrorLite); Disk.LogException(e); }
        }

        private static void WriteWavHeader(int sampleRate, short bitsPerSample, short channels)
        {
            try
            {
                using (FileStream fs = File.Open(currentAudioFile, FileMode.Open, FileAccess.ReadWrite))
                {
                    using (BinaryWriter writer = new BinaryWriter(fs, System.Text.Encoding.UTF8))
                    {
                        writer.Seek(0, SeekOrigin.Begin);

                        // ChunkID               
                        writer.Write('R');
                        writer.Write('I');
                        writer.Write('F');
                        writer.Write('F');

                        // ChunkSize               
                        writer.Write(BitConverter.GetBytes(fs.Length + 36), 0, 4);

                        // Format               
                        writer.Write('W');
                        writer.Write('A');
                        writer.Write('V');
                        writer.Write('E');

                        //SubChunk               
                        writer.Write('f');
                        writer.Write('m');
                        writer.Write('t');
                        writer.Write(' ');

                        // SubChunk1Size - 16 for PCM
                        writer.Write(BitConverter.GetBytes(16), 0, 4);

                        // AudioFormat - PCM=1
                        writer.Write(BitConverter.GetBytes((short)1), 0, 2);

                        // Channels: Mono=1, Stereo=2
                        writer.Write(BitConverter.GetBytes(channels), 0, 2);

                        // SampleRate
                        writer.Write(sampleRate);

                        // ByteRate
                        int byteRate = sampleRate * 1 * bitsPerSample / 8;
                        writer.Write(BitConverter.GetBytes(byteRate), 0, 4);

                        // BlockAlign
                        int blockAlign = channels * bitsPerSample / 8;
                        writer.Write(BitConverter.GetBytes((short)blockAlign), 0, 2);

                        // BitsPerSample
                        writer.Write(BitConverter.GetBytes(bitsPerSample), 0, 2);

                        // SubChunk2ID
                        writer.Write('d');
                        writer.Write('a');
                        writer.Write('t');
                        writer.Write('a');

                        // Subchunk2Size
                        writer.Write(BitConverter.GetBytes(fs.Length), 0, 4);
                    }
                }
            }
            catch (Exception e) { _ = Kernel.Beep(Tone.CdmaSoftErrorLite); Disk.LogException(e); }
        }

        //private static void TransmitAudio()
        //{
        //    try
        //    {
        //    listen:
        //        tcpListener.Start();
        //        TcpClient tcpClient = tcpListener.AcceptTcpClient();
        //        NetworkStream networkStream = tcpClient.GetStream();

        //        while (audioRecord.RecordingState == RecordState.Recording && tcpClient.Connected) { try { networkStream.Write(audioSample, 0, audioSample.Length); } catch { } }
        //        try { tcpClient.Close(); tcpListener.Stop(); } catch { }
        //        goto listen;
        //    }
        //    catch { }
        //}
    }
}