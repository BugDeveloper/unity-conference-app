using System;
using UnityEngine;

public static class AndroidAppLauncher
{
    public static void LaunchAndroidAppWithBundleId(string bundleId)
    {
        var fail = false;
        var up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        var packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

        AndroidJavaObject launchIntent = null;

        try
        {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
        }
        catch (Exception e)
        {
            fail = true;
        }

        if (fail)
        {
            //open app in store
            Application.OpenURL("market://details?id=" + bundleId);
        }
        else //open the app
            ca.Call("startActivity", launchIntent);

        up.Dispose();
        ca.Dispose();
        packageManager.Dispose();
        launchIntent.Dispose();
    }
}