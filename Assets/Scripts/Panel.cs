using UnityEngine;

public class Panel : MonoBehaviour
{

    private bool _opened;

    private InputHelper _helper;

    // Use this for initialization
    void Start ()
    {
        _helper = InputHelper.Instance;
        transform.position = new Vector2(transform.position.x, InfoStorage.ClosedPanelPosY);
    }

    void Update()
    {
        if (_opened)
        {
            _readInput();
        }
    }

    public void Open()
    {
        if (_opened) return;

        _helper.Increase();

        _opened = true;
        AnimationAssistant.MoveY(transform, InfoStorage.OpenedPanelPosY);
    }

    private void _readInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void Close()
    {
        if (!_opened) return;

        _helper.PanelClose();
        _opened = false;
        AnimationAssistant.MoveY(transform, InfoStorage.ClosedPanelPosY);
    }
}