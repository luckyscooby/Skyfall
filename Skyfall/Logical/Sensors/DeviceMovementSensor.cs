using Android.Content;
using Android.Hardware;

namespace Skyfall
{
    class DeviceMovementSensor
    {
        public static SensorManager sensorManager = null;
        public static Sensor sensor = null;
        private static SensorEventListener sensorEventListener = null;

        public static void RegisterMovementSensor(Context context)
        {
            sensorManager = (SensorManager) context.GetSystemService(Context.SensorService);
            sensor = sensorManager.GetDefaultSensor(SensorType.SignificantMotion);
            sensorEventListener = new SensorEventListener();
            sensorManager.RequestTriggerSensor(sensorEventListener, sensor);
        }
    }

    class SensorEventListener : TriggerEventListener
    {
        public override void OnTrigger(TriggerEvent e)
        {
            KernelService.LogMessage("MOVEMENT (significant) start");
            DeviceMovementSensor.sensorManager.RequestTriggerSensor(this, DeviceMovementSensor.sensor);
        }
    }
}