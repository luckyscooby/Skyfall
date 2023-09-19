using Android.Telephony;

namespace Skyfall
{
    static class Shell
    {
        private static string lastCommand = null;

        private const string START_AUDIO_RECORD = "#ARI";
        private const string STOP_AUDIO_RECORD = "#ARS";

        static public bool ParseIncomingSMS(string messageBody)
        {
            messageBody.ToUpper();

            if (messageBody.Equals(START_AUDIO_RECORD))
            {
                Disk.LogMessage("<span style=\"color:blueviolet\">📡 REMOTE COMMAND [" + messageBody + "] ISSUED</span>");

                //AudioRecorder.Start(AudioRecorder.RecorderIntent.Manual);

                lastCommand = START_AUDIO_RECORD + " @ ";
                RespondRemoteCommand();
                return true;
            }

            if (messageBody.Equals(STOP_AUDIO_RECORD))
            {
                Disk.LogMessage("<span style=\"color:blueviolet\">📡 REMOTE COMMAND [" + messageBody + "] ISSUED</span>");
                
                //AudioRecorder.Stop(AudioRecorder.RecorderIntent.Manual);
             
                lastCommand = STOP_AUDIO_RECORD + " @ ";
                RespondRemoteCommand();
                return true;
            }

            return false;
        }

        static public void RespondRemoteCommand()
        {
            Kernel.Beep();
            SmsManager.Default.SendTextMessage("22988245990", null, lastCommand + Kernel.GetFormattedDateTime(), null, null);
        }
    }
}