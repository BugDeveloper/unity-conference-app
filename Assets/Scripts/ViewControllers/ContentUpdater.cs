using UnityEngine;
using UnityEngine.Events;

public abstract class ContentUpdater : MonoBehaviour
{
    public abstract UnityEvent UpdateStart { get; protected set; }
    public abstract UnityEvent UpdateEnd { get; protected set; }

    public abstract void ContentUpdate();

    protected void _destroyTransformChildren(Transform transform)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);

        }
    }
}
