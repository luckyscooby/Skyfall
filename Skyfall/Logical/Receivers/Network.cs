using Android.Net;
using System;

namespace Skyfall
{
    public class NetworkStateReceiver : ConnectivityManager.NetworkCallback
    {
        private enum NetworkState
        {
            Online,
            Suspended,
            Offline
        }
        private static NetworkState networkState = NetworkState.Offline; // Assume device is offline at boot.

        public override void OnCapabilitiesChanged(Network network, NetworkCapabilities networkCapabilities)
        {
            if (networkCapabilities.HasCapability(NetCapability.Validated))
            {
                if (networkState != NetworkState.Online)
                {
                    Kernel.LogMessage("<span style=\"color:seagreen\">📡 NETWORK INTERFACE ONLINE</span>");
                    networkState = NetworkState.Online;
                }
            }
            else if (!networkCapabilities.HasCapability(NetCapability.Validated))
            {
                if (networkState != NetworkState.Suspended)
                {
                    Kernel.LogMessage("<span style=\"color:orange\">📡 NETWORK INTERFACE SUSPENDED</span>");
                    networkState = NetworkState.Suspended;
                }
            }

            base.OnCapabilitiesChanged(network, networkCapabilities);
        }

        public override void OnLost(Network network)
        {
            if (networkState != NetworkState.Offline)
            {
                if (!Kernel.wifiManager.IsWifiEnabled && !Kernel.telephonyManager.DataEnabled)
                {
                    Kernel.LogMessage("<span style=\"color:indianred\">📡 NETWORK INTERFACES DISABLED</span>");
                }
                else Kernel.LogMessage("<span style=\"color:orange\">📡 NETWORK INTERFACE LOST</span>");

                networkState = NetworkState.Offline;
            }

            base.OnLost(network);
        }
    }
}