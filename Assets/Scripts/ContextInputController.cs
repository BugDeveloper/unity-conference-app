using System.Collections;
using UnityEngine;

public class ContextInputController : MonoBehaviour {

	private Context _context = Context.Main;
    private EventStorage _eventStorage;

	[SerializeField]
	private Transform _instagram, _feedback;

    private bool _lock;

	private enum Context
	{
		Instagram,
		Feedback,
		Main
	}

    void Start()
    {
        _eventStorage = EventStorage.Instance;
        _eventStorage.FeedbackClicked.AddListener(_setFeedback);
        _eventStorage.InstagramClicked.AddListener(_setInstagram);
    }

	private void _setInstagram()
	{
	    StartCoroutine(_lockTimer());

		_context = Context.Instagram;
	}

	private void _setFeedback()
	{
	    StartCoroutine(_lockTimer());

		_context = Context.Feedback;
	}

    private IEnumerator _lockTimer()
    {
        _lock = true;

        yield return new WaitForSeconds(1.2f);

        _lock = false;
    }

	void Update()
	{
		_readInput();
	}

	public void Escape()
	{
	    if (_lock) return;

		switch (_context)
		{
			case Context.Feedback:
                AnimationAssistant.MoveY(_feedback, _feedback.transform.position.y - PlayerPrefs.GetFloat("height"));
			    _context = Context.Main;
				break;
			
			case Context.Instagram:
			    AnimationAssistant.MoveY(_instagram, _instagram.transform.position.y - PlayerPrefs.GetFloat("height"));
				_context = Context.Main;
			break;
		}
	}

	private void _readInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			Escape();
	}

}
