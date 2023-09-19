using Android.App;
using Android.App.Admin;
using Android.Content;

namespace Skyfall
{
    [BroadcastReceiver(Permission = "android.permission.BIND_DEVICE_ADMIN")]
    [MetaData("android.app.device_admin", Resource = "@xml/deviceadminconfig")]
    [IntentFilter(new[] { "android.app.action.DEVICE_ADMIN_ENABLED", Intent.ActionMain })]
    public class Admin : DeviceAdminReceiver
    {
        public static DevicePolicyManager policyManager = null;

        public override void OnEnabled(Context context, Intent intent)
        {
            //context.PackageManager.SetComponentEnabledSetting(new ComponentName(context, Java.Lang.Class.FromType(typeof(ActivationActivity))), Android.Content.PM.ComponentEnabledState.Disabled, Android.Content.PM.ComponentEnableOption.DontKillApp);

            base.OnEnabled(context, intent);
        }
    }
}