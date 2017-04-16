using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContentUpdateTrigger : MonoBehaviour, IEndDragHandler
{
    private ScrollRect _sr;
    private ContentUpdater _cu;
    private EventStorage _eventStorage;

    private bool _lock;

    private void Awake()
    {
        _sr = GetComponent<ScrollRect>();
        _cu = GetComponent<ContentUpdater>();
    }

    // Use this for initialization
    void Start()
    {
        _cu.UpdateEnd.AddListener(_unlockList);
    }

    void Update()
    {
        _locking();
    }

    private void _locking()
    {
        if (_lock)
            _sr.content.anchoredPosition = new Vector2(_sr.content.anchoredPosition.x, -200f);
    }

    private void _checkContentUpdate()
    {
        if (_sr.content.anchoredPosition.y > -200f && !_lock) return;

        _lockList();

        _cu.ContentUpdate();
    }

    private void _lockList()
    {
        _lock = true;
        _sr.movementType = ScrollRect.MovementType.Unrestricted;
    }

    private void _unlockList()
    {
        _lock = false;
        _sr.movementType = ScrollRect.MovementType.Elastic;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _checkContentUpdate();
    }
}