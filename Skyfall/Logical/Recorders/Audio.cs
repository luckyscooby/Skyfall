using Android.Media;
using System;
using System.IO;

namespace Skyfall
{
    public static class AudioRecorder
    {
        private static readonly MediaRecorder mediaRecorder = new MediaRecorder();
        private static bool isRecording = false;
        private static string lastAudioFile = "";

        public enum RecorderIntent
        {
            Auto,
            PhoneCall,
            Manual,
            PauseResume,
            Remote // For version 1.4, not yet implemented, just a reminder;
        }

        public static void Start(RecorderIntent recorderIntent)
        {
            try
            {
                switch (recorderIntent)
                {
                    case RecorderIntent.Auto:
                        try { FileStream fileStream = File.Create(Kernel.AUTO_LOCK_FILE); fileStream.Close(); } catch (IOException) { }
                        if (isRecording) return;
                        break;

                    case RecorderIntent.PhoneCall:
                        try { FileStream fileStream = File.Create(Kernel.PHONECALL_LOCK_FILE); fileStream.Close();  } catch (IOException) { }
                        //for (int v = 0; v <= 7; v++) Kernel.audioManager.AdjustStreamVolume(Android.Media.Stream.VoiceCall, Adjust.Raise, VolumeNotificationFlags.Vibrate);
                        if (isRecording) return;
                        break;

                    case RecorderIntent.Manual:
                        try { FileStream fileStream = File.Create(Kernel.MANUAL_LOCK_FILE); fileStream.Close(); } catch (IOException) { }
                        if (isRecording) return;
                        break;

                    default:
                        break;
                }

                mediaRecorder.SetAudioSource(AudioSource.Unprocessed); // OK
                mediaRecorder.SetOutputFormat(OutputFormat.Mpeg4);  // OK
                mediaRecorder.SetAudioEncoder(AudioEncoder.Aac); // OK
                mediaRecorder.SetAudioEncodingBitRate(384000); // OK
                mediaRecorder.SetAudioSamplingRate(44100); // OK
                mediaRecorder.SetAudioChannels(2); // OK
                lastAudioFile = Kernel.MEDIA_AUDIO_DIRECTORY + "/" + Kernel.GetFormattedDateTime() + Kernel.MEDIA_AUDIO_FILE_EXTENSION;
                mediaRecorder.SetOutputFile(lastAudioFile);
                mediaRecorder.Prepare();
                mediaRecorder.Start();

                if (recorderIntent == RecorderIntent.PauseResume) Kernel.LogMessage("<span style=\"color:blueviolet\">⏯ AUDIORECORDER RESUME</span>");

                // Feedback for manual;
                if (recorderIntent == RecorderIntent.Manual) Kernel.ShowToast("ARI");

                isRecording = true;
            }
            catch (Java.Lang.IllegalStateException)
            {
                // Another application has exclusively locked the microphone hardware.
                Kernel.LogMessage("<span style=\"color:orange\">🎤 CANNOT START RECORDING NOW: Another application has the microphone/camera lock</span>");
            }
            //catch (IOException)
            //{
            //    // mediaRecorder.SetOutputFile() failed, reset it and try again;
            //    mediaRecorder.Reset();

            //}
            catch (Exception e) { Kernel.LogException(e); }


        }

        public static void Stop(RecorderIntent recorderIntent)
        {
            switch (recorderIntent)
            {
                case RecorderIntent.Auto:
                    try { File.Delete(Kernel.AUTO_LOCK_FILE); } catch (IOException) { }
                    if (IsHardwareStillLocked()) return;
                    break;

                case RecorderIntent.PhoneCall:
                    try { File.Delete(Kernel.PHONECALL_LOCK_FILE); } catch (IOException) { }
                    if (IsHardwareStillLocked()) return;
                    break;

                case RecorderIntent.Manual:
                    try { File.Delete(Kernel.MANUAL_LOCK_FILE); } catch (IOException) { }
                    if (IsHardwareStillLocked()) return;
                    break;

                default:
                    break;
            }

            try
            {
                mediaRecorder.Stop();
                mediaRecorder.Reset();

                // Consider as recording when pause;
                if (recorderIntent != RecorderIntent.PauseResume)
                {
                    isRecording = false;
                    Kernel.PackFile(lastAudioFile);
                }
                else Kernel.LogMessage("<span style=\"color:blueviolet\">⏯ AUDIORECORDER PAUSE</span>");

                // Feedback for manual;
                if (recorderIntent == RecorderIntent.Manual) Kernel.ShowToast("ARS");
            }
            catch (Java.Lang.IllegalStateException) { }
            catch (Java.Lang.RuntimeException)
            {
                if (isRecording)
                {
                    /// .Stop() was called immediatelly after .Start() certainly due to user interaction with camera;

                    // Remove defected file;
                    if (File.Exists(lastAudioFile)) try { File.Delete(lastAudioFile); } catch (IOException) { }

                    // Reset MediaRecorder object;
                    mediaRecorder.Reset();

                    Kernel.LogMessage("<span style=\"color:blueviolet\">⏯ AUDIORECORDER RECOVERED PAUSE</span>");
                }
                else throw; /// Something else is wrong, throw to next catch (and log);
            }
            catch (Exception e) { Kernel.LogException(e); }
        }

        public static bool IsHardwareStillLocked()
        {
            if (File.Exists(Kernel.AUTO_LOCK_FILE)) return true;
            if (File.Exists(Kernel.PHONECALL_LOCK_FILE)) return true;
            if (File.Exists(Kernel.MANUAL_LOCK_FILE)) return true;

            return false;
        }

        public static void Pause() { if (isRecording) try { AudioRecorder.Stop(RecorderIntent.PauseResume); } catch { } }

        public static void Resume() { if (isRecording) try { AudioRecorder.Start(RecorderIntent.PauseResume); } catch { } }
    }
}