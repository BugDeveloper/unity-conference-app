using System.Collections;
using UnityEngine;

public class UpdateIndicator : MonoBehaviour
{
    [SerializeField] private Transform _indicator, _list;
    [SerializeField] private ContentUpdater _contentUpdater;

    private EventStorage _eventStorage;
    private bool _rotate;

    public void SetList(Transform list)
    {
        _list = list;
    }

	// Use this for initialization
	void Start () {
		_eventStorage = EventStorage.Instance;
	    _contentUpdater.UpdateStart.AddListener(_startRotation);
	    _contentUpdater.UpdateEnd.AddListener(_stopAnimate);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    transform.position =  new Vector2(transform.position.x, _list.position.y);
	}

    private void _startRotation()
    {
        _rotate = true;
        StartCoroutine(_animateIndicatorCoroutine());
    }

    private void _stopAnimate()
    {
        _rotate = false;
    }

    private IEnumerator _animateIndicatorCoroutine()
    {
        while (_rotate)
        {
            AnimationAssistant.LocalRotateZ180(_indicator);
            yield return new WaitForSeconds(AnimationAssistant.AnimationSpeedDefault);
        }
    }

}
