using System.Collections;
using UnityEngine;

public class InternetChecker : MonoBehaviour
{
    public bool Internet { get; private set; }

    private float _checkingStep = 10f;
    private const bool AutoCheck = true;
    private EventStorage _eventStorage;

    // Use this for initialization
    void Start()
    {
        _eventStorage = EventStorage.Instance;
        StartCoroutine(_checkInternetCoroutine());
    }

    private IEnumerator _checkInternetCoroutine()
    {
        do
        {
            var www = new WWW("http://small.sns.gdforge.fvds.ru/");
            yield return www;

            if (www.error == null && !string.IsNullOrEmpty(www.text) && !Internet)
            {
                Internet = true;
                _eventStorage.ConnectionEstablished.Invoke();
            } else if (Internet)
            {
                Internet = false;
                _eventStorage.ConnectionTerminated.Invoke();
            }

            yield return new WaitForSeconds(_checkingStep);
        } while (AutoCheck);
    }
}