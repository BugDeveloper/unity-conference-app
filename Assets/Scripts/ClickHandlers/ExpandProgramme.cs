using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExpandProgramme : MonoBehaviour, IPointerClickHandler
{
    private bool _expanded;

    [SerializeField] private GameObject _expansion;

    private RectTransform _rt;
    private VerticalLayoutGroup _vlg;
    private CanvasGroup _cg;

    private AnchorLayoutGroup alg;

    private bool _lock;

    // Use this for initialization
    void Start()
    {
        _rt = _expansion.GetComponent<RectTransform>();
        _vlg = _expansion.GetComponent<VerticalLayoutGroup>();
        _cg = _expansion.GetComponent<CanvasGroup>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _expandOrNarrow();
    }

    private void _expandOrNarrow()
    {
       // LayoutRebuilder.MarkLayoutForRebuild(transform.parent.parent.parent as RectTransform);
        if (_expanded)
            _narrow();
        else
            _expand();
    }

    private void _narrow()
    {
        _expanded = false;

        AnimationAssistant.ExpandAndFade(_rt, new Vector2(_rt.sizeDelta.x, 0), _cg, 0f, false);
    }

    private void _expand()
    {
        _expanded = true;

        AnimationAssistant.ExpandAndFade(_rt, new Vector2(_rt.sizeDelta.x, _vlg.preferredHeight), _cg, 1f, true);
    }
}