using UnityEngine;
using UnityEngine.EventSystems;

public class ToGoogleMaps : MonoBehaviour, IPointerClickHandler
{
    private EventStorage _es;

    void Start()
    {
        _es = EventStorage.Instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AnimationAssistant.ButtonEffect(transform);
        _es.OpenMaps.Invoke();

    }
}
