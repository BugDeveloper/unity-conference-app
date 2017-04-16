using UnityEngine;
using UnityEngine.EventSystems;

public class OpenExtension : MonoBehaviour, IPointerClickHandler
{
    public ProgrammeExpansion ProgrammeExpansion { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        ProgrammeExpansion.Open();
    }
}
