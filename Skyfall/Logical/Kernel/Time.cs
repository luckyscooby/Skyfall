/// 1.5 Revision: OK
using Android.Content;
using System;

namespace Skyfall
{
    [BroadcastReceiver]
    public class Time : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //AudioRecorder.Schedule();
            NetworkDriver.ReportNetworkInterface();
            NotificationDriver.ClearFilter();
        }
    }
}