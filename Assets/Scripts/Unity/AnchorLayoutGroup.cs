using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AnchorLayoutGroup : MonoBehaviour
{
    public bool upDown = false;
    public bool zeroOffsets = false;

    private LayoutGroup lg;

    private void _someInitializationShit()
    {
        if (Application.isPlaying)
        {
            var lastChild = transform.GetChild(transform.childCount - 1) as RectTransform;
            (transform as RectTransform).anchorMax = Vector2.right;
            (transform as RectTransform).anchorMin = Vector2.zero;
            (transform as RectTransform).offsetMin = new Vector2(0, lastChild.rect.min.y);

            transform.position = transform.parent.position;
        }
    }

    void Start()
    {
        lg = GetComponent<LayoutGroup>();
      //  StartCoroutine(_updateLayoutGroupAnchorsTunedCoroutine());
    }

    private IEnumerator _updateLayoutGroupAnchorsTunedCoroutine()
    {
        while (true)
        {
            Debug.Log("Update layout working");
            _updateLayoutGroupAnchorsTuned();
            yield return new WaitForEndOfFrame();
        }

    }

    private void _updateLayoutGroupAnchorsTuned()
    {
        if (transform.childCount == 0) return;

        float lastChildYAnchor = 0;
        float lastChildY = 0;
        if (upDown)
            lastChildYAnchor = 1;

        if (!(transform.GetChild(0) is RectTransform)) return;

        var rect = transform.GetChild(0) as RectTransform;
        if (upDown)
        {
            rect.anchorMax = new Vector2(rect.anchorMax.x, lastChildYAnchor);
            lastChildYAnchor = rect.anchorMin.y;
        }
        else
        {
            rect.anchorMin = new Vector2(rect.anchorMin.x, lastChildYAnchor);
            lastChildYAnchor = rect.anchorMax.y;
        }

        if (!zeroOffsets) return;

        rect.offsetMax = Vector2.zero;
        rect.offsetMin = Vector2.zero;
    }

    private void _updateLayoutGroupAnchors()
    {
        float lastChildYAnchor = 0;
        float lastChildY = 0;
        if (upDown)
            lastChildYAnchor = 1;

        foreach (var child in transform)
            if (child is RectTransform)
            {
                var rect = child as RectTransform;
                if (upDown)
                {
                    rect.anchorMax = new Vector2(rect.anchorMax.x, lastChildYAnchor);
                    lastChildYAnchor = rect.anchorMin.y;
                }
                else
                {
                    rect.anchorMin = new Vector2(rect.anchorMin.x, lastChildYAnchor);
                    lastChildYAnchor = rect.anchorMax.y;
                }
                if (zeroOffsets)
                {
                    rect.offsetMax = Vector2.zero;
                    rect.offsetMin = Vector2.zero;
                }
            }
    }

    private void Update()
    {
       // _updateLayoutGroupAnchors();
        lg.enabled = false;
        lg.enabled = true;
    }
}