using UnityEngine;

public class InputHelper : MonoBehaviour
{
    public static InputHelper Instance;

    private bool windows;
    private bool _panelOpened;

    void Awake()
    {
        Instance = this;
    }

    public void Increase()
    {
        _panelOpened = true;
        windows = true;
    }

    public void PanelClose()
    {
        _panelOpened = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (!windows && !_panelOpened) {

        var activity =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call<bool>("moveTaskToBack", true);
} else if(windows) {
    windows = false;
}

#endif
    }
}