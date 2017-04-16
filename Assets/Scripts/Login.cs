using System.Collections;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public void CheckLoginInfo(Text info)
    {
        StartCoroutine(_sendLoginData(info.text));
    }

    private IEnumerator _sendLoginData(string email)
    {
        var wwwForm = new WWWForm();

        wwwForm.AddField("email", email);

        var www = new WWW(InfoStorage.Server + InfoStorage.Connect, wwwForm);

        yield return www;

        Debug.Log(InfoStorage.Server + InfoStorage.Connect);

        if (www.error != null)
            yield break;

        Debug.Log(www.text);

        var json = JSON.Parse(www.text);

        string answer = json["error"];

        Debug.Log(answer);

        if (!answer.Equals("ok")) yield break;

        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetInt("auntificated", 1);

        AnimationAssistant.MoveY(transform, transform.position.y - Screen.height);
    }
}