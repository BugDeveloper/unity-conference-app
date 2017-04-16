using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{

    private AsyncOperation _async;
    [SerializeField]

    // Use this for initialization
	void Start ()
	{
	    SceneManager.LoadScene("Main", LoadSceneMode.Single);
	   // SceneManager.UnloadSceneAsync("Loading");
	}

    void Update()
    {
    }


/*    private IEnumerator _animateDots()
    {
        while (!_async.isDone)
        {
            for (var i = 0; i < 3; i++)
            {
                _text.text += ".";
                yield return new WaitForSeconds(0.5f);
            }

            _text.text = _text.text.Substring(0, _text.text.Length - 3);
            yield return new WaitForSeconds(0.5f);
        }
    }*/

}
