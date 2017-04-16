using UnityEngine;
using UnityEngine.EventSystems;

public class ToInstagram : MonoBehaviour, IPointerClickHandler
{
    private void _openInstagram(string tag)
    {
#if UNITY_EDITOR
        Debug.Log("To instagram");

#elif UNITY_ANDROID
	    AndroidAppLauncher.LaunchAndroidAppWithBundleId("com.instagram.android");

#elif UNITY_IOS
        Application.OpenURL("https://www.instagram.com/");
#endif
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _openInstagram("#SNS25");
    }
}