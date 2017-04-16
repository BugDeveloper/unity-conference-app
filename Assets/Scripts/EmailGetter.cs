using UnityEngine;
using UnityEngine.UI;

public class EmailGetter : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    GetComponent<InputField>().text = PlayerPrefs.GetString("email");
	}

}
