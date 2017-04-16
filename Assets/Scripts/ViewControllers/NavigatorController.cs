using UnityEngine;

public class NavigatorController : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        _initializeNavigatorSection();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void _initializeNavigatorSection()
    {
        var cg = GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
    }
}