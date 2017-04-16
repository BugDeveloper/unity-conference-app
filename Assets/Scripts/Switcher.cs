using UnityEngine;
using UnityEngine.UI;

public class Switcher : MonoBehaviour {

    protected readonly Color32 InactiveTextColor = new Color32(88, 95, 99, 255);
    protected readonly Color32 ActiveTextColor = new Color32(255, 255, 255, 255);

    protected readonly Color32 ActiveColor = new Color32(165, 204, 72, 255);
    protected readonly Color32 InactivetColor = new Color32(200, 200, 200, 155);

    protected void _changeCaseToInactive(Transform caseObj)
    {
        AnimationAssistant.ChangeTextColor(caseObj.FindChild("Text").GetComponent<Text>(), InactiveTextColor);
        AnimationAssistant.ImageColor(caseObj.GetComponent<Image>(), InactivetColor);
    }

    protected void _changeCaseToActive(Transform caseObj)
    {
        AnimationAssistant.ChangeTextColor(caseObj.FindChild("Text").GetComponent<Text>(), ActiveTextColor);
        AnimationAssistant.ImageColor(caseObj.GetComponent<Image>(), ActiveColor);
    }

}
