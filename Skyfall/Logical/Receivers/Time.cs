using Android.Content;
using System;

namespace Skyfall
{
    [BroadcastReceiver]
    public class TimeStateReceiver : BroadcastReceiver
    {
        public static int memoryMaintenancTicker = 0;

        public override void OnReceive(Context context, Intent intent)
        {
            AudioRecordSchedule();
            Kernel.PerformMemoryMaintenance();
        }

        private void AudioRecordSchedule()
        {
            DateTime now = DateTime.Now;
            
            if (now.Hour >= 0 && now.Hour <= 11) // AM
            {
                if (now.Hour == 11 && now.Minute == 59) AudioRecorder.Stop(AudioRecorder.RecorderIntent.Auto);
                else AudioRecorder.Start(AudioRecorder.RecorderIntent.Auto);
            }
            
            // One minute transition gap (shadow zone)
            
            else if (now.Hour >= 12 && now.Hour <= 23) // PM
            {
                if (now.Hour == 23 && now.Minute == 59) AudioRecorder.Stop(AudioRecorder.RecorderIntent.Auto);
                else AudioRecorder.Start(AudioRecorder.RecorderIntent.Auto);
            }
        }
    }
}