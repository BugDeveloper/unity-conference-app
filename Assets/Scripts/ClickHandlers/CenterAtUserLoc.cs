using UnityEngine;
using UnityEngine.EventSystems;

public class CenterAtUserLoc : MonoBehaviour, IPointerClickHandler
{

    private EventStorage _eventStorage;

    void Start()
    {
        _eventStorage = EventStorage.Instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _eventStorage.MapToUser.Invoke();
        AnimationAssistant.ButtonEffect(transform);
    }
}
