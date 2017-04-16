using UnityEngine;

public class SectionSwitcher : Switcher
{
    [SerializeField] private GameObject _activeSection, _navigationSection;

    [SerializeField] private Transform _programmeCase, _newsCase, _navigationCase;

    [SerializeField] private CanvasGroup _gradient;

    private EventStorage _eventStorage;

    void Start()
    {
        Canvas.ForceUpdateCanvases();
        _initializeFirst();
        _eventStorage = EventStorage.Instance;
        _eventStorage.LocationClicked.AddListener(_switchToNavigation);
    }

    private void _initializeFirst()
    {
        _changeCaseToActive(_programmeCase);
        AnimationAssistant.Show(_activeSection.GetComponent<CanvasGroup>());
    }

    private void _switchToNavigation(string useless)
    {
        SwitchToSection(_navigationSection);
    }

    public void SwitchToSection(GameObject section)
    {
        if (AnimationAssistant.IsShown(section.GetComponent<CanvasGroup>()))
        {
            return;
        }

        _deactivateCurrentSection();

        AnimationAssistant.SwitchFromTo(_activeSection.GetComponent<CanvasGroup>(),
            section.GetComponent<CanvasGroup>());

        _activeSection = section;

        switch (section.name)
        {
            case "ProgrammeView":
                _changeCaseToActive(_programmeCase);
                break;

            case "NewsView":
                _changeCaseToActive(_newsCase);
                break;

            case "NavigationView":
                _changeCaseToActive(_navigationCase);
                _gradient.alpha = 0f;
                break;
        }
    }

    private void _deactivateCurrentSection()
    {
        switch (_activeSection.name)
        {
            case "ProgrammeView":
                _changeCaseToInactive(_programmeCase);
                break;

            case "NewsView":
                _changeCaseToInactive(_newsCase);
                break;

            case "NavigationView":
                _changeCaseToInactive(_navigationCase);
                _gradient.alpha = 1f;
                break;
        }
    }
}