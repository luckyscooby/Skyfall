using Android.Content;
using Android.Net;
using Android.Provider;
using Android.Telephony;
using static Android.Provider.ContactsContract;

namespace Skyfall
{
    [BroadcastReceiver]
    public class PhoneDriver : BroadcastReceiver
    {
        private static string lastPhoneNumber = null;

        public override void OnReceive(Context context, Intent intent)
        {
            TelephonyManager telephonyManager = (TelephonyManager)Kernel.applicationContext.GetSystemService(Context.TelephonyService);

            if (intent.Action.Equals(TelephonyManager.ActionPhoneStateChanged))
            {
                lastPhoneNumber = intent.GetStringExtra(TelephonyManager.ExtraIncomingNumber);
                if (lastPhoneNumber == null) return;
                else
                {
                    /// Ringing ///
                    if (telephonyManager.CallState.Equals(CallState.Ringing))
                    {
                        Disk.LogMessage(ref Disk.deviceStateWriter, "<span style=\"color:mediumblue\">☎ << " + RetrieveContactName() + "</span>");
                    }
                    /// Offhook ///
                    else if (telephonyManager.CallState.Equals(CallState.Offhook))
                    {
                        Disk.LogMessage(ref Disk.deviceStateWriter, "<span style=\"color:mediumblue\">☎ /\\ " + RetrieveContactName() + "</span>");
                    }
                    /// Hungup ///
                    else if (telephonyManager.CallState.Equals(CallState.Idle))
                    {
                        Disk.LogMessage(ref Disk.deviceStateWriter, "<span style=\"color:mediumblue\">☎ \\/ " + RetrieveContactName() + "</span>");
                    }
                }
            }
            /// Outgoing ///
            else if (intent.Action.Equals(Intent.ActionNewOutgoingCall))
            {
                lastPhoneNumber = intent.GetStringExtra(Intent.ExtraPhoneNumber);
                Disk.LogMessage(ref Disk.deviceStateWriter, "<span style=\"color:mediumblue\">☎ >> " + RetrieveContactName() + "</span>");
            }
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