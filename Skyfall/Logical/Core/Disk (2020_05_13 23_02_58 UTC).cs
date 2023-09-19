using Android.OS;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Skyfall
{
    partial class Kernel
    {
        private const string WORK_DIRECTORY = ".Skyfall";

        private const string STATES_LOG_FILE = ".DeviceStates.htm";
        private const string NOTIFICATIONS_LOG_FILE = ".FilteredNotifications.htm";
        private const string ALL_NOTIFICATIONS_LOG_FILE = ".AllNotifications.htm";
        private const string LOG_HTML_HEADER = "<meta charset=\"UTF-8\"><style>body{margin:0;font-family:monospace;font-size:11px;white-space:nowrap;}</style>";

        public const string MEDIA_AUDIO_DIRECTORY = "Audio";
        public const string MEDIA_AUDIO_FILE_EXTENSION = ".m4a";

        public const string AUTO_LOCK_FILE = ".ARAutoLock";
        public const string PHONECALL_LOCK_FILE = ".ARPhoneCallLock";
        public const string MANUAL_LOCK_FILE = ".ARManualLock";

        public static void LogMessage(string message, bool timestamp = true, bool appNotification = false)
        {
            try
            {
                using StreamWriter streamWriter = new StreamWriter(appNotification ? NOTIFICATIONS_LOG_FILE : STATES_LOG_FILE, true);
                switch (UserStateReceiver.screenState)
                {
                    case UserStateReceiver.ScreenState.Off:
                        streamWriter.Write("<div style=\"background-color:darkgray\">");
                        break;

                    case UserStateReceiver.ScreenState.On:
                        streamWriter.Write("<div style=\"background-color:aliceblue\">");
                        break;
                }

                if (timestamp) streamWriter.Write("<span style=\"color:gainsboro\">" + DateTime.Now + " > </span>");

                streamWriter.Write(message + "</div>");
            }
            catch { }
        }

        public static void LogException(Exception e)
        {
            LogMessage("<span style=\"color:red\">❌ <strong>EXCEPTION</strong></span>");
            LogMessage("<span style=\"color:red\">❌ <em>" + e.Message + "</em></span>");
            LogMessage("<span style=\"color:red\">❌ <em>" + e.StackTrace + "</em></span>");
        }

        public static void SetupWorkDirectory()
        {
            try { Directory.SetCurrentDirectory(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath); } catch { }
            try { Directory.CreateDirectory(WORK_DIRECTORY); } catch { }
            try { Directory.SetCurrentDirectory(WORK_DIRECTORY); } catch { }
            try { Directory.CreateDirectory(MEDIA_AUDIO_DIRECTORY); } catch { }

            try { FileStream filesStream = File.Create(".nomedia"); filesStream.Close(); } catch { }

            if (!File.Exists(STATES_LOG_FILE)) LogMessage(LOG_HTML_HEADER, false);
            if (!File.Exists(NOTIFICATIONS_LOG_FILE)) LogMessage(LOG_HTML_HEADER, false, true);
        }

        public static async void PackFile(string inputFile)
        {
            try
            {
                long inputFileBytes = new FileInfo(inputFile).Length;
                long availableStorageBytes = new StatFs(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath).AvailableBytes;
                if (inputFileBytes > availableStorageBytes) return;
                else
                {
                    using (ZipArchive archive = ZipFile.Open(inputFile + ".zip", ZipArchiveMode.Create))
                    {
                        await Task.Run(() => archive.CreateEntryFromFile(inputFile, inputFile, CompressionLevel.Optimal));
                    }
                    File.Delete(inputFile);
                }
            }
            catch (Exception e) { LogException(e); }
        }
    }
}