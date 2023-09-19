/// 1.5 Revision: OK
using Android.Content;
using Android.Net.Wifi;
using Android.Telephony;

namespace Skyfall
{
    public static class NetworkDriver
    {
        private enum NetworkState
        {
            Unknown,
            Enabled,
            Disabled
        }
        private static NetworkState networkState = NetworkState.Unknown;

        public static void ReportNetworkInterface()
        {
            if (IsNetworkAvailable() && networkState != NetworkState.Enabled)
            {
                Disk.LogMessage(ref Disk.stateWriter, "<span style=\"color:seagreen\">📡 NETWORK ENABLED</span>");
                networkState = NetworkState.Enabled;
            }
            else if (!IsNetworkAvailable() && networkState != NetworkState.Disabled)
            {
                Disk.LogMessage(ref Disk.stateWriter, "<span style=\"color:indianred\">📡 NETWORK DISABLED</span>");
                networkState = NetworkState.Disabled;
            }
        }

        private static bool IsNetworkAvailable()
        {
            WifiManager wifiManager = (WifiManager)Kernel.applicationContext.GetSystemService(Context.WifiService);
            TelephonyManager telephonyManager = (TelephonyManager)Kernel.applicationContext.GetSystemService(Context.TelephonyService);

            if (!wifiManager.IsWifiEnabled && !telephonyManager.DataEnabled) return false; else return true;
        }
    }
}