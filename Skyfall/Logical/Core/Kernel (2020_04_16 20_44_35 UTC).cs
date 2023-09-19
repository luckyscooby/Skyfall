using Android;
using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Camera2;
using Android.Locations;
using Android.Media;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Telephony;
using Android.Views.Accessibility;
using Android.Widget;
using Plugin.SimpleAudioPlayer;
using System;
using System.IO;

namespace Skyfall
{
    [Service(Permission = Manifest.Permission.BindAccessibilityService, Name = "com.hygnus.skyfall.Kernel")]
    [IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
    //[MetaData("android.accessibility-service", Resource = "@xml/accessibilityserviceconfig")]
    partial class Kernel : AccessibilityService
    {
        /// Accessibility service started (boot or manually);
        protected override void OnServiceConnected()
        {
            base.OnServiceConnected();

            // Setup service for notifications and activities;
            AccessibilityServiceInfo accessibilityServiceInfo = new AccessibilityServiceInfo
            {
                Flags = AccessibilityServiceFlags.IncludeNotImportantViews,
                EventTypes = EventTypes.NotificationStateChanged | EventTypes.WindowStateChanged,
                FeedbackType = FeedbackFlags.Generic,
            };
            SetServiceInfo(accessibilityServiceInfo);

            // Setup service for broadcasts;
            Initialize();
        }

        /// Accessibility service stopped (shutdown or manually);
        public override void OnInterrupt()
        {
            //applicationContext.UnregisterReceiver(userStateReceiver);
            //applicationContext.UnregisterReceiver(phoneStateReceiver);
            //applicationContext.UnregisterReceiver(timeStateReceiver);
            //connectivityManager.UnregisterNetworkCallback(networkStateReceiver);

            //Kernel.LogMessage("<strong>📱 TERMINATED</strong>");
        }

        /// System event received, pass it to corresponding handler;
        public override void OnAccessibilityEvent(AccessibilityEvent e)
        {
            // Route event;
            if (e.EventType == EventTypes.NotificationStateChanged) { if (e != null) ProcessNotification(e); }
            if (e.EventType == EventTypes.WindowStateChanged) { if (e != null) ProcessActivity(e); }
        }

        public static Context applicationContext = null;

        public static ConnectivityManager connectivityManager = null;
        public static TelephonyManager telephonyManager = null;
        public static WifiManager wifiManager = null;
        public static LocationManager locationManager = null;
        public static NotificationManager notificationManager = null;
        public static PowerManager powerManager = null;
        public static AudioManager audioManager = null;
        public static CameraManager cameraManager = null;

        public static UserStateReceiver userStateReceiver = new UserStateReceiver();
        public static NetworkStateReceiver networkStateReceiver = new NetworkStateReceiver();
        public static PhoneStateReceiver phoneStateReceiver = new PhoneStateReceiver();
        public static TimeStateReceiver timeStateReceiver = new TimeStateReceiver();

        public static string packageName = null;

        private static ISimpleAudioPlayer beepPlayer = null;

        public void Initialize()
        {
            applicationContext = this;

            beepPlayer = CrossSimpleAudioPlayer.Current;
            beepPlayer.Load("c.wav");
            beepPlayer.Volume = 0.05;


            Kernel.SetupWorkDirectory();

            Kernel.LogMessage("<strong>📱 " + PackageName.ToUpper() + " " + PackageManager.GetPackageInfo(PackageName, 0).VersionName.ToUpper() + "</strong>");

            // Assimilate services managers //
            connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            telephonyManager = (TelephonyManager)GetSystemService(TelephonyService);
            wifiManager = (WifiManager)GetSystemService(WifiService);
            locationManager = (LocationManager)GetSystemService(LocationService);
            notificationManager = (NotificationManager)GetSystemService(NotificationService);
            powerManager = (PowerManager)GetSystemService(PowerService);
            audioManager = (AudioManager)GetSystemService(AudioService);

            // Instantiate and register receivers and callbacks //
            connectivityManager.RegisterDefaultNetworkCallback(networkStateReceiver);
            RegisterReceiver(phoneStateReceiver, new IntentFilter(TelephonyManager.ActionPhoneStateChanged));
            RegisterReceiver(phoneStateReceiver, new IntentFilter(Intent.ActionNewOutgoingCall));
            RegisterReceiver(userStateReceiver, new IntentFilter(Intent.ActionScreenOn));
            RegisterReceiver(userStateReceiver, new IntentFilter(Intent.ActionPowerConnected));
            RegisterReceiver(userStateReceiver, new IntentFilter(Intent.ActionPowerDisconnected));
            RegisterReceiver(userStateReceiver, new IntentFilter(Intent.ActionBatteryLow));
            RegisterReceiver(userStateReceiver, new IntentFilter(Intent.ActionShutdown));
            RegisterReceiver(userStateReceiver, new IntentFilter(Intent.ActionReboot));
            RegisterReceiver(userStateReceiver, new IntentFilter(Intent.ActionScreenOff));
            RegisterReceiver(userStateReceiver, new IntentFilter(Intent.ActionAirplaneModeChanged));
            RegisterReceiver(userStateReceiver, new IntentFilter(LocationManager.ModeChangedAction));
            RegisterReceiver(timeStateReceiver, new IntentFilter(Intent.ActionTimeTick));

            ResumeState();

            Beep();
        }

        public static void Beep() => beepPlayer.Play();

        public static string GetBatteryLevel()
        {
            IntentFilter filter = new IntentFilter(Intent.ActionBatteryChanged);
            Intent battery = applicationContext.RegisterReceiver(null, filter);
            int level = battery.GetIntExtra(BatteryManager.ExtraLevel, -1);
            int scale = battery.GetIntExtra(BatteryManager.ExtraScale, -1);

            return (int)Math.Floor(level * 100D / scale) + "%";
        }

        public static void ResumeState()
        {
            // Manual audio recording;
            try { File.Delete(Kernel.AUTO_LOCK_FILE); } catch (IOException) { }
            try { File.Delete(Kernel.PHONECALL_LOCK_FILE); } catch (IOException) { }
            if (AudioRecorder.IsHardwareStillLocked()) AudioRecorder.Start(AudioRecorder.RecorderIntent.Manual);
        }

        public static void ShowToast(string message)
        {
            Toast.MakeText(applicationContext, message, ToastLength.Long).Show();
            Beep();
        }

        public static string GetFormattedDateTime() => DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss");

        public static void PerformMemoryMaintenance()
        {
            TimeStateReceiver.memoryMaintenancTicker++;
            if (TimeStateReceiver.memoryMaintenancTicker == 120)
            {
                // If 2 hours (60 * 2 = 120 minutes) have passed;
                TimeStateReceiver.memoryMaintenancTicker = 0;

                Kernel.LogMessage("PERFORMING MEMORY MAINTENANCE");

                int totalHeldNotifications = Kernel.notificationArrayList.Count;
                Kernel.notificationArrayList.Clear();
                if (Kernel.notificationArrayList.Count == 0) Kernel.LogMessage("There were " + totalHeldNotifications + " items in <i>notificationArrayList</i>");

                /// Experimental garbage collection
                //long totalMemoryA = GC.GetTotalMemory(false);
                //GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, false, true);
                //long totalMemoryB = GC.GetTotalMemory(false);
                //Kernel.LogMessage("Previous Allocated Memory: " + totalMemoryA);
                //Kernel.LogMessage("Freed Memory: " + (totalMemoryA - totalMemoryB));
                //Kernel.LogMessage("Actual Allocated Memory: " + totalMemoryB);

                Kernel.LogMessage("MEMORY MAINTENANCE DONE");
            }
        }
    }
}