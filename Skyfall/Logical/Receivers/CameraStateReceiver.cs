using Android.Hardware.Camera2;

namespace Skyfall
{
    class CameraStateReceiver : CameraManager.AvailabilityCallback
    {
        /// Face unlock/detection features must be disabled on device for this not to conflit and to properly work!

        public override void OnCameraAvailable(string cameraId)
        {
            //Disk.LogMessage("<span style=\"color:blueviolet\">📷 CAMERA [" + cameraId + "] AVAILABLE</span>");
            //AudioRecorder.Resume();

            base.OnCameraAvailable(cameraId);
        }

        public override void OnCameraUnavailable(string cameraId)
        {
            //Disk.LogMessage("<span style=\"color:blueviolet\">📷 CAMERA [" + cameraId + "] UNAVAILABLE</span>");
            //AudioRecorder.Pause();

            base.OnCameraUnavailable(cameraId);
        }
    }
}