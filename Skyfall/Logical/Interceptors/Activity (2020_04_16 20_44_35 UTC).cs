using Android.Content;
using Android.Content.PM;
using Android.Views.Accessibility;
using System;
using System.Collections.Generic;

namespace Skyfall
{
    partial class Kernel
    {
        private static string applicationLabel = "";
        private static string lastActivity = "";

        private void ProcessActivity(AccessibilityEvent ev)
        {
            if (ev.PackageName != null && ev.ClassName != null)
            {
                // Test if this is an actual activity;
                ComponentName componentName = new ComponentName(ev.PackageName, ev.ClassName);
                ActivityInfo activityInfo;
                try { activityInfo = PackageManager.GetActivityInfo(componentName, 0); }
                catch (PackageManager.NameNotFoundException) { activityInfo = null; }
                if (activityInfo != null)
                {
                    if (componentName.FlattenToShortString() != lastActivity)
                    {
                        lastActivity = componentName.FlattenToShortString();

                        // Get application friendly label;
                        ApplicationInfo applicationInfo;
                        try { applicationInfo = PackageManager.GetApplicationInfo(ev.PackageName, 0); }
                        catch (PackageManager.NameNotFoundException) { applicationInfo = null; }
                        if (applicationInfo != null) applicationLabel = PackageManager.GetApplicationLabel(applicationInfo);

                        Kernel.LogMessage("<span style=\"color:hotpink\"><strong>" + applicationLabel + "</strong> [" + componentName.ClassName + "]</span>");
                    }
                }
            }
        }
    }
}