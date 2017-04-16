using UnityEngine;

public class MapFunctionalAppearanceController : MonoBehaviour
{
    private EventStorage _eventStorage;
    private CanvasGroup _canvasGroup;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        _eventStorage = EventStorage.Instance;
        _eventStorage.UserLocationDetermined.AddListener(_pleaseWork);
        _eventStorage.LocationClicked.AddListener(_show);
        _eventStorage.MapToDefault.AddListener(_hide);
    }

    private void _pleaseWork(GoogleMapMarker gmm)
    {
        _eventStorage.MapToUser.AddListener(_show);
    }

    private void _hide()
    {
        AnimationAssistant.Hide(_canvasGroup);
    }

    private void _show(string address)
    {
        if (!address.Equals(""))
            AnimationAssistant.Show(_canvasGroup);
    }

    private void _show()
    {
        AnimationAssistant.Show(_canvasGroup);
    }
}