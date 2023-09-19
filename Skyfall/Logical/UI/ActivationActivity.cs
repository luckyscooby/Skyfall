using Android.App;
using Android.OS;

namespace Skyfall
{
    [Activity/*(MainLauncher = true)*/]
    //[IntentFilter(new[] { "android.intent.category.LAUNCHER" })]
    public class ActivationActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Admin.policyManager = (DevicePolicyManager)GetSystemService(DevicePolicyService);
            //if (!Admin.policyManager.IsAdminActive(new ComponentName(this, Java.Lang.Class.FromType(typeof(Admin)))))
            //{
            //    Intent deviceAdminIntent = new Intent(DevicePolicyManager.ActionAddDeviceAdmin);
            //    deviceAdminIntent.PutExtra(DevicePolicyManager.ExtraDeviceAdmin, new ComponentName(this, Java.Lang.Class.FromType(typeof(Admin))));
            //    StartActivityForResult(deviceAdminIntent, 1);
            //}
        }
    }
}