using System.Globalization;
using System.Threading;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    void Awake()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");

        if (PlayerPrefs.GetFloat("height") == 0)
            PlayerPrefs.SetFloat("height", Screen.height);

        if (PlayerPrefs.GetFloat("width") == 0)
            PlayerPrefs.SetFloat("width", Screen.width);

        Application.runInBackground = true;

    }

    // Use this for initialization
    void Start()
    {
        InfoStorage.OpenedPanelPosY = transform.position.y;
        InfoStorage.ClosedPanelPosY = transform.position.y - Screen.height - 20f;
        Screen.fullScreen = false;
    }
}