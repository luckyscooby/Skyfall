using Android.Content;
using Android.Locations;
using Android.Provider;
using System;

namespace Skyfall
{
    [BroadcastReceiver]
    public class UserStateReceiver : BroadcastReceiver
    {
        public enum ScreenState
        {
            Off,
            On
        }
        public static ScreenState screenState = ScreenState.On; // Assume screen is on at boot.
        private static int previousGPSState = 4; // 4 = Unknown;
        public static bool shutdownSignal = false;

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals(Intent.ActionScreenOn))
            {
                if (screenState == ScreenState.Off)
                {
                    screenState = ScreenState.On;
                    Kernel.LogMessage("<span style=\"color:dodgerblue\">👁‍🗨 ON</span>");
                }
            }
            else if (intent.Action.Equals(Intent.ActionScreenOff))
            {
                if (screenState == ScreenState.On)
                {
                    screenState = ScreenState.Off;
                    Kernel.LogMessage("<span style=\"color:seashell\">💤 OFF</span>");
                }
            }
            else if (intent.Action.Equals(Intent.ActionPowerConnected))
            {
                Kernel.LogMessage("<span style=\"color:gainsboro\">⚡ CHARGING @ " + Kernel.GetBatteryLevel() + "</span>");
            }
            else if (intent.Action.Equals(Intent.ActionPowerDisconnected))
            {
                Kernel.LogMessage("<span style=\"color:gainsboro\">⚡ DISCHARGING @ " + Kernel.GetBatteryLevel() + "</span>");
            }
            else if (intent.Action.Equals(Intent.ActionBatteryLow))
            {
                Kernel.LogMessage("<span style=\"color:orange\">⚡ BATTERY LOW/SAVING MODE @ " + Kernel.GetBatteryLevel() + "</span>");
            }
            else if (intent.Action.Equals(Intent.ActionShutdown))
            {
                AudioRecorder.Pause();
                Kernel.LogMessage("<strong><span style=\"color:black\">♻ SHUTDOWN @ " + Kernel.GetBatteryLevel() + "</span></strong>");
                shutdownSignal = true;
            }
            else if (intent.Action.Equals(Intent.ActionReboot))
            {
                AudioRecorder.Pause();
                Kernel.LogMessage("<strong><span style=\"color:black\">♻ REBOOT @ " + Kernel.GetBatteryLevel() + "</span></strong>");
                shutdownSignal = true;
            }
            else if (intent.Action.Equals(Intent.ActionAirplaneModeChanged))
            {
                Kernel.LogMessage("<span style=\"color:indianred\">🛑 AIRPLANE MODE [" + AirplaneModeState() + "]</span>");
            }
            else if (intent.Action.Equals(LocationManager.ModeChangedAction))
            {
                if (GPSProviderState() != previousGPSState)
                {
                    Kernel.LogMessage("<span style=\"color:indianred\">🛑 GPS PROVIDER [" + GPSProviderState() + "]</span>");
                    previousGPSState = GPSProviderState();
                }
            }
        }

        public static int AirplaneModeState() => Settings.Global.GetInt(Kernel.applicationContext.ContentResolver, Settings.Global.AirplaneModeOn);

        //public static bool GPSProviderState() => Kernel.locationManager.IsProviderEnabled(LocationManager.GpsProvider);
#pragma warning disable CS0618 // O tipo ou membro é obsoleto
        public static int GPSProviderState() => Settings.Secure.GetInt(Kernel.applicationContext.ContentResolver, Settings.Secure.LocationMode);
#pragma warning restore CS0618 // O tipo ou membro é obsoleto
    }
}