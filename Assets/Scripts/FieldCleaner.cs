using UnityEngine;
using UnityEngine.UI;

public class FieldCleaner : MonoBehaviour
{

    private InputField _field;

    void Start ()
	{
	    _field = GetComponent<InputField>();
	}

    public void Clean()
    {
        _field.text = "";
    }
}
