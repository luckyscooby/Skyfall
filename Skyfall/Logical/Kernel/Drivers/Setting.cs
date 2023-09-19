/// 1.5 Revision: OK
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Provider;
using System;

namespace Skyfall
{
    [BroadcastReceiver]
    public class SettingDriver : BroadcastReceiver
    {
        public enum ScreenState
        {
            Off,
            On
        }
        public static ScreenState screenState = ScreenState.On; // Assume screen is on at boot.
        private static byte previousGPSState = 3; // Assume GPS mode is high accuracy.

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals(Intent.ActionScreenOn))
            {
                if (screenState == ScreenState.Off)
                {
                    screenState = ScreenState.On;
                    Disk.LogMessage(ref Disk.stateWriter, "<span style=\"color:dodgerblue\">👁‍🗨 ON</span>");
                }
            }
            else if (intent.Action.Equals(Intent.ActionScreenOff))
            {
                if (screenState == ScreenState.On)
                {
                    screenState = ScreenState.Off;
                    Disk.LogMessage(ref Disk.stateWriter, "<span style=\"color:seashell\">💤 OFF</span>");
                }
            }
            else if (intent.Action.Equals(Intent.ActionPowerConnected))
            {
                Disk.LogMessage(ref Disk.stateWriter, "<span style=\"color:gainsboro\">⚡ CHARGING @ " + GetBatteryLevel() + "</span>");
            }
            else if (intent.Action.Equals(Intent.ActionPowerDisconnected))
            {
                Disk.LogMessage(ref Disk.stateWriter, "<span style=\"color:gainsboro\">⚡ DISCHARGING @ " + GetBatteryLevel() + "</span>");
            }
            else if (intent.Action.Equals(Intent.ActionBatteryLow))
            {
                Disk.LogMessage(ref Disk.stateWriter, "<span style=\"color:orange\">⚡ BATTERY LOW/SAVING MODE @ " + GetBatteryLevel() + "</span>");
            }
            else if (intent.Action.Equals(Intent.ActionShutdown))
            {
                Disk.LogMessage(ref Disk.stateWriter, "<strong><span style=\"color:black\">♻ SHUTDOWN @ " + GetBatteryLevel() + "</span></strong>");
                Kernel.Terminate();
            }
            else if (intent.Action.Equals(Intent.ActionAirplaneModeChanged))
            {
                Disk.LogMessage(ref Disk.stateWriter, "<span style=\"color:indianred\">🛑 AIRPLANE MODE [" + AirplaneModeState() + "]</span>");
            }
            else if (intent.Action.Equals(LocationManager.ModeChangedAction))
            {
                if (GPSProviderState() != previousGPSState)
                {
                    Disk.LogMessage(ref Disk.stateWriter, "<span style=\"color:indianred\">🛑 GPS PROVIDER [" + GPSProviderState() + "]</span>");
                    previousGPSState = GPSProviderState();
                }
            }
        }

        private static byte AirplaneModeState() => (byte)Settings.Global.GetInt(Kernel.applicationContext.ContentResolver, Settings.Global.AirplaneModeOn);

#pragma warning disable CS0618 // O tipo ou membro é obsoleto
        private static byte GPSProviderState() => (byte)Settings.Secure.GetInt(Kernel.applicationContext.ContentResolver, Settings.Secure.LocationMode);
#pragma warning restore CS0618 // O tipo ou membro é obsoleto

        private static string GetBatteryLevel()
        {
            IntentFilter filter = new IntentFilter(Intent.ActionBatteryChanged);
            Intent battery = Kernel.applicationContext.RegisterReceiver(null, filter);
            byte level = (byte)battery.GetIntExtra(BatteryManager.ExtraLevel, -1);
            byte scale = (byte)battery.GetIntExtra(BatteryManager.ExtraScale, -1);

            return (byte)Math.Floor(level * 100D / scale) + "%";
        }

        public static string GetFormattedDateTime() => DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss");
    }
}