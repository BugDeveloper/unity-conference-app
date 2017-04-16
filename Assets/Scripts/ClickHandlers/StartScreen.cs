using System.Collections;
using UnityEngine;

public class StartScreen : MonoBehaviour
{

    [SerializeField] private Transform _loginForm, _loginPanel;

    void Start()
    {
        StartCoroutine(_loginFormUpCoroutine());
    }

    private IEnumerator _loginFormUpCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        AnimationAssistant.Hide(gameObject.GetComponent<CanvasGroup>());

        if (PlayerPrefs.GetInt("auntificated") != 1)
            AnimationAssistant.MoveLocalY(_loginForm, 0);
        else
            AnimationAssistant.MoveY(_loginPanel, InfoStorage.ClosedPanelPosY);
    }

}
