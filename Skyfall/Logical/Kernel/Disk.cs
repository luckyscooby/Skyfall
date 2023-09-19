/// 1.5 Revision: OK
using Android.Media;
using System;
using System.IO;

namespace Skyfall
{
    public static class Disk
    {
        //private const string WORK_DIRECTORY = "Skyfall";

        public const string STATES_LOG_FILE = "States.htm";
        public const string NOTIFICATIONS_LOG_FILE = "Notifications.htm";
        private const string LOG_HTML_HEADER = "<meta charset=\"UTF-8\"><style>body{margin:0;font-family:monospace;font-size:11px;white-space:nowrap;}</style>";

        public const string MEDIA_AUDIO_DIRECTORY = "Audio";
        public const string MEDIA_AUDIO_FILE_EXTENSION = ".wav";
        public const long MEDIA_AUDIO_MAX_SIZE = 3221225472; // ~3GB / 3221225472 bytes

        public static StreamWriter stateWriter = null;
        public static StreamWriter notificationWriter = null;

        public static void LogMessage(ref StreamWriter streamWriter, string message, bool timestamp = true)
        {
            try
            {
                switch (SettingDriver.screenState)
                {
                    case SettingDriver.ScreenState.Off:
                        streamWriter.Write("<div style=\"background-color:darkgray\">");
                        break;

                    case SettingDriver.ScreenState.On:
                        streamWriter.Write("<div style=\"background-color:aliceblue\">");
                        break;
                }

                if (timestamp) streamWriter.Write("<span style=\"color:gainsboro\">" + DateTime.Now + " > </span>");

                streamWriter.Write(message + "</div>");
            }
            catch { _ = Kernel.Beep(Tone.CdmaSoftErrorLite); }
        }

        public static void LogException(Exception e)
        {
            LogMessage(ref stateWriter, "<span style=\"color:red\">❌ <strong>" + e.Message + "</strong></span>" + "<span style=\"color:red\">❌ <em>" + e.StackTrace + "</em></span>");
        }

        public static void SetupWorkDirectory()
        {
            try
            {
#pragma warning disable CS0618 // O tipo ou membro é obsoleto
                //Directory.SetCurrentDirectory(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath);
#pragma warning restore CS0618 // O tipo ou membro é obsoleto
                Directory.SetCurrentDirectory(Kernel.applicationContext.ExternalCacheDir.AbsolutePath);
                //if (!Directory.Exists(WORK_DIRECTORY)) Directory.CreateDirectory(WORK_DIRECTORY);
                //Directory.SetCurrentDirectory(WORK_DIRECTORY);
                if (!Directory.Exists(MEDIA_AUDIO_DIRECTORY)) Directory.CreateDirectory(MEDIA_AUDIO_DIRECTORY);
                //if (!File.Exists(".nomedia")) { FileStream fileStream = File.Create(".nomedia"); fileStream.Close(); fileStream.Dispose(); }

                stateWriter = new StreamWriter(STATES_LOG_FILE, true);
                notificationWriter = new StreamWriter(NOTIFICATIONS_LOG_FILE, true);
                stateWriter.AutoFlush = true;
                notificationWriter.AutoFlush = true;
                LogMessage(ref stateWriter, LOG_HTML_HEADER, false);
                LogMessage(ref notificationWriter, LOG_HTML_HEADER, false);
            }
            catch { _ = Kernel.Beep(Tone.SupError); }
        }
    }
}
