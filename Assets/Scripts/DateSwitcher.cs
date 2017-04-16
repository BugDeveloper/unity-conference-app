using UnityEngine;
using UnityEngine.UI;

public class DateSwitcher : Switcher
{
    [SerializeField] private Transform _date12, _date13, _date14, _date15, _activeDateList;

    [SerializeField] private ScrollRect _mainScrollRect;
    [SerializeField] private UpdateIndicator _updateIndicator;

    void Start()
    {
        SwitchToCase(_activeDateList);
    }

    public void SwitchToCase(Transform _dateList)
    {
        if (AnimationAssistant.IsShown(_dateList.GetComponent<CanvasGroup>())) return;

        _deactivateCurrentCase();

        AnimationAssistant.SwitchFromTo(_activeDateList.GetComponent<CanvasGroup>(), _dateList.GetComponent<CanvasGroup>());

        _activeDateList = _dateList;

        _mainScrollRect.content = _dateList as RectTransform;
        _updateIndicator.SetList(_dateList);

        switch (_dateList.name)
        {
            case "Programme12Date":
                _changeCaseToActive(_date12);
                break;

            case "Programme13Date":
                _changeCaseToActive(_date13);
                break;

            case "Programme14Date":
                _changeCaseToActive(_date14);
                break;

            case "Programme15Date":
                _changeCaseToActive(_date15);
                break;
        }
    }

    private void _deactivateCurrentCase()
    {
        switch (_activeDateList.name)
        {
            case "Programme12Date":
                _changeCaseToInactive(_date12);
                break;

            case "Programme13Date":
                _changeCaseToInactive(_date13);
                break;

            case "Programme14Date":
                _changeCaseToInactive(_date14);
                break;

            case "Programme15Date":
                _changeCaseToInactive(_date15);
                break;
        }
    }
}