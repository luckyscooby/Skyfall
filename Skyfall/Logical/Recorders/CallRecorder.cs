using Android.Media;
using System;

namespace Skyfall
{
    public static class CallRecorder
    {
        private static MediaRecorder mediaRecorder = new MediaRecorder();
        public static bool isInstantiated = false;

        public static void Start(string phoneNumber)
        {
            if (isInstantiated) return;
            else if (AudioRecorder.isInstantiated) AudioRecorder.Stop(pause: true);
            else if (VideoRecorder.isInstantiated) VideoRecorder.Stop(pause: true);

            try
            {
                mediaRecorder.SetAudioSource(AudioSource.Unprocessed);
                mediaRecorder.SetOutputFormat(OutputFormat.AacAdts);
                mediaRecorder.SetAudioEncoder(AudioEncoder.Aac);
                mediaRecorder.SetAudioEncodingBitRate(128000);
                mediaRecorder.SetAudioSamplingRate(44100);
                mediaRecorder.SetAudioChannels(2);
                mediaRecorder.SetOutputFile(KernelService.MEDIA_CALL_DIRECTORY + "/" + KernelService.GetFormattedDateTime() + "." + phoneNumber + KernelService.MEDIA_AUDIO_FILE_EXTENSION);

                for (int v = 0; v <= 7; v++)
                {
                    KernelService.audioManager.AdjustStreamVolume(Stream.VoiceCall, Adjust.Raise, VolumeNotificationFlags.Vibrate);
                }
                mediaRecorder.Prepare();
                mediaRecorder.Start();
                isInstantiated = true;
            }
            catch (Exception e) { KernelService.LogException(e); }
        }

        public static void Stop()
        {
            if (!isInstantiated) return;
            else
            {
                try
                {
                    mediaRecorder.Stop();
                    mediaRecorder.Reset();
                    isInstantiated = false;
                }
                catch (Exception e) { KernelService.LogException(e); }

                if (VideoRecorder.isPaused) VideoRecorder.Start();
                else if (AudioRecorder.isPaused) AudioRecorder.Start();
            }
        }
    }
}