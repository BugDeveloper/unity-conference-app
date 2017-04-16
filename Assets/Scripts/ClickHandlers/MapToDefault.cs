using UnityEngine;
using UnityEngine.EventSystems;

public class MapToDefault : MonoBehaviour, IPointerClickHandler
{
    private EventStorage _eventStorage;

    // Use this for initialization
    void Start()
    {
        _eventStorage = EventStorage.Instance;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _eventStorage.MapToDefault.Invoke();
        AnimationAssistant.LocalRotateZ180(transform);
    }
}