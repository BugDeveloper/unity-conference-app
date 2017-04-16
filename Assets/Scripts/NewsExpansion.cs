using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewsExpansion : MonoBehaviour, IPointerClickHandler
{
    private bool _expanded;

    public void InitializeExpansion()
    {
        if (_text.preferredHeight < InfoStorage.MaxDefaultTextSize)
        {
            _rt.sizeDelta = new Vector2(_rt.sizeDelta.x, _text.preferredHeight);
            gameObject.SetActive(false);
        }
        else
        {
            _rt.sizeDelta = new Vector2(_rt.sizeDelta.x, InfoStorage.MaxDefaultTextSize);
        }
    }

    [SerializeField] private GameObject _info;

    private Text _text;
    private RectTransform _rt;

    public void OnPointerClick(PointerEventData eventData)
    {
        _expandOrNarrow();
    }

    void Awake()
    {
        _rt = _info.GetComponent<RectTransform>();
        _text = _info.GetComponent<Text>();
    }

    void Start()
    {
    }

    private void _expandOrNarrow()
    {
        if (!_expanded)
        {
            _expandText();
        }
        else
        {
            _narrowText();
        }
    }

    private void _narrowText()
    {
        AnimationAssistant.ExpandRect(_rt, new Vector2(_rt.sizeDelta.x, InfoStorage.MaxDefaultTextSize));
        _expanded = false;
    }

    private void _expandText()
    {
        AnimationAssistant.ExpandRect(_rt, new Vector2(_rt.sizeDelta.x, _text.preferredHeight));
        _expanded = true;
    }
}