using UnityEngine;
using UnityEngine.EventSystems;

public class ToVk : MonoBehaviour, IPointerClickHandler {

    private void _openVk()
    {
#if UNITY_EDITOR
        Debug.Log("To vk");
#elif UNITY_ANDROID
	    AndroidAppLauncher.LaunchAndroidAppWithBundleId("com.vkontakte.android");
#elif UNITY_IOS
        Application.OpenURL("https://www.vk.com");
#endif
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _openVk();
    }
}
