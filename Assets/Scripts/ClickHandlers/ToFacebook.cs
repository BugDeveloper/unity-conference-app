using UnityEngine;
using UnityEngine.EventSystems;

public class ToFacebook : MonoBehaviour, IPointerClickHandler {

    private void _openVk()
    {
#if UNITY_EDITOR
        Debug.Log("To facebook");
#elif UNITY_ANDROID
	    AndroidAppLauncher.LaunchAndroidAppWithBundleId("com.facebook.katana");
#elif UNITY_IOS
        Application.OpenURL("https://www.facebook.com/");
#endif
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _openVk();
    }
}
