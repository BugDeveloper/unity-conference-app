using UnityEngine;

public class ProgrammeExpansion : MonoBehaviour
{

    private bool _opened;

    private static float _xOpened;
    private static float _xClosed;

	// Use this for initialization
	void Start ()
	{
	    _xOpened = transform.position.x;
	    transform.position = new Vector2(transform.position.x + Screen.width, transform.position.y);
	    _xClosed = transform.position.x;
	}

    void Update()
    {
/*        if (_opened)
        {
            _readInput();
        }*/
    }

    public void Open()
    {
        if (_opened) return;
        _opened = true;
        AnimationAssistant.MoveX(transform, _xOpened);
    }

    private void _readInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void Close()
    {
        if (!_opened) return;
        _opened = false;
        AnimationAssistant.MoveX(transform, _xClosed);
    }
}
