using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExpandTextChange : MonoBehaviour, IPointerClickHandler
{
	private bool _expanded;

	private const string NotExpanded = "Ещё";
	private const string Expanded = "Скрыть";

    [SerializeField]
    private Text _text;

	public void OnPointerClick(PointerEventData eventData)
	{
		_changeText();
	}

	private void _changeText()
	{
		if (_expanded)
		{
			_expanded = false;
		    _text.text = NotExpanded;
		}
		else
		{
			_expanded = true;
		    _text.text = Expanded;
		}
	}

}
