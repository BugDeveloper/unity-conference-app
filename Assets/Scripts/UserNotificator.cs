using UnityEngine;
using UnityEngine.UI;

public class UserNotificator : MonoBehaviour
{
    [SerializeField] private Text _text;

    public static UserNotificator Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void Notify(string notification)
    {
        _text.text = notification;

        AnimationAssistant.FadeText(_text, 1);
    }

    public void StopCurrent()
    {
        _text.text = "";
        AnimationAssistant.FadeText(_text, 0);
    }
}