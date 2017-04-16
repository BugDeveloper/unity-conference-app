using UnityEngine;
using UnityEngine.EventSystems;

public class InstagramButton : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private Panel _instagramPanel;

    void Start()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       _instagramPanel.Open();
    }
}