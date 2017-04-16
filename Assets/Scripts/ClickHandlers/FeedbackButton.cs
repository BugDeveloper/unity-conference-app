using UnityEngine;
using UnityEngine.EventSystems;

public class FeedbackButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Panel _feedbackPanel;

    private void Start()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _feedbackPanel.Open();
    }
}