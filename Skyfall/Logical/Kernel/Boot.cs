/// 1.5 Revision: OK
using Android;
using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.Media;
using Android.Runtime;
using Android.Views.Accessibility;
using System.Threading.Tasks;

namespace Skyfall
{
    [Service(Permission = Manifest.Permission.BindAccessibilityService, Name = "com.hygnus.skyfall.Kernel")]
    [IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
    partial class Kernel : AccessibilityService
    {
        protected override void OnServiceConnected()
        {
            base.OnServiceConnected();

            AccessibilityServiceInfo accessibilityServiceInfo = new AccessibilityServiceInfo
            {
                EventTypes = EventTypes.NotificationStateChanged | EventTypes.WindowStateChanged | EventTypes.WindowsChanged,
                FeedbackType = FeedbackFlags.Generic,
                Flags = AccessibilityServiceFlags.RetrieveInteractiveWindows
            };
            SetServiceInfo(accessibilityServiceInfo);

            Initialize();
        }

        public override void OnInterrupt() { Terminate(); }

        public override bool OnUnbind(Intent intent) { Terminate(); return base.OnUnbind(intent); }

        /// System event received, route it to corresponding handler;
        public override void OnAccessibilityEvent(AccessibilityEvent e)
        {
            if (e.EventType == EventTypes.NotificationStateChanged) { if (e != null) NotificationDriver.CaptureNotification(e); }
            else if (e.EventType == EventTypes.WindowStateChanged) { if (e != null) ActivityDriver.CaptureActivity(e); }
            else if (e.EventType == EventTypes.WindowsChanged) { if (e != null) ActivityDriver.CaptureTitle(e); }
        }

        public static async Task Beep(Tone tone)
        {
            //new ToneGenerator(Stream.System, 100).StartTone(tone, 1000);
            await Task.Delay(1000);
        }

        public static Context applicationContext = null;
        public static readonly SettingDriver settingDriverReceiver = new SettingDriver();
        public static readonly Time timeDriverReceiver = new Time();

        public void Initialize()
        {
            applicationContext = this;

            Disk.SetupWorkDirectory();

            Disk.LogMessage(ref Disk.stateWriter, "<strong>📱 " + PackageName.ToUpper() + " " + PackageManager.GetPackageInfo(PackageName, 0).VersionName.ToUpper() + "</strong>");

            RegisterReceiver(settingDriverReceiver, new IntentFilter(Intent.ActionScreenOn));
            RegisterReceiver(settingDriverReceiver, new IntentFilter(Intent.ActionScreenOff));
            RegisterReceiver(settingDriverReceiver, new IntentFilter(Intent.ActionPowerConnected));
            RegisterReceiver(settingDriverReceiver, new IntentFilter(Intent.ActionPowerDisconnected));
            RegisterReceiver(settingDriverReceiver, new IntentFilter(Intent.ActionBatteryLow));
            RegisterReceiver(settingDriverReceiver, new IntentFilter(Intent.ActionShutdown));
            RegisterReceiver(settingDriverReceiver, new IntentFilter(Intent.ActionAirplaneModeChanged));
            RegisterReceiver(settingDriverReceiver, new IntentFilter(LocationManager.ModeChangedAction));
            RegisterReceiver(timeDriverReceiver, new IntentFilter(Intent.ActionTimeTick));

            _ = Beep(Tone.CdmaAlertNetworkLite);

            /// Extra initialization routines;
            NetworkDriver.ReportNetworkInterface();
            //AudioRecorder.Start();
        }

        public static void Terminate()
        {
            //AudioRecorder.Stop();

            Disk.LogMessage(ref Disk.stateWriter, "<strong>📱 TERMINATED</strong>");
            Disk.LogMessage(ref Disk.stateWriter, "<br />", false);

            _ = Beep(Tone.CdmaNetworkBusyOneShot);
        }

        //public override void OnLowMemory()
        //{
        //    base.OnLowMemory();

        //    Disk.LogMessage(ref Disk.stateWriter, "📱 LOW RAM");
        //}

        //public override void OnTrimMemory([GeneratedEnum] TrimMemory level)
        //{
        //    base.OnTrimMemory(level);

        //    string trimLevel = string.Empty;
        //    switch (level)
        //    {
        //        case TrimMemory.Background:
        //            trimLevel = "<span style =\"color:gold\">Background</span>";
        //            break;
        //        case TrimMemory.Complete:
        //            trimLevel = "<span style =\"color:orangered\">Complete</span>";
        //            break;
        //        case TrimMemory.Moderate:
        //            trimLevel = "<span style =\"color:darkorange\">Moderate</span>";
        //            break;
        //        case TrimMemory.RunningCritical:
        //            trimLevel = "<span style =\"color:olive\">RunningCritical</span>";
        //            break;
        //        case TrimMemory.RunningLow:
        //            trimLevel = "<span style =\"color:yellowgreen\">RunningLow</span>";
        //            break;
        //        case TrimMemory.RunningModerate:
        //            trimLevel = "<span style =\"color:forestgreen\">RunningModerate</span>";
        //            break;
        //        case TrimMemory.UiHidden:
        //            trimLevel = "<span style =\"color:gray\">UiHidden</span>";
        //            break;
        //    }

        //    Disk.LogMessage(ref Disk.stateWriter, "📱 DEVICE RAM LEVEL: " + trimLevel);
        //}
    }
}