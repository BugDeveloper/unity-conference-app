using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionSender : MonoBehaviour
{

    [SerializeField] private CanvasGroup _success, _failure, _form;

    private const string FileName = "questions.dat";

    private Queue<string> _questions;

    void Awake()
    {
        _questions = new Queue<string>();
        _loadCachedAndDelete();
    }

    void Start()
    {
        if (_questions.Count != 0)
        {
            _sendAllCachedQuestions();
        }
    }

    private void _loadCachedAndDelete()
    {
        if (!LocalStorage.FileExists(FileName)) return;
        _questions = (Queue<string>) LocalStorage.Load(FileName);
        LocalStorage.DeleteFile(FileName);
    }

    private void _sendAllCachedQuestions()
    {
        StartCoroutine(_sendAllCachedQuestionsCoroutine());
    }

    private IEnumerator _sendAllCachedQuestionsCoroutine()
    {
        while (_questions.Count != 0)
        {
            yield return StartCoroutine(_sendQuestionCoroutine(_questions.Dequeue()));
        }
    }

    public void SendQuestion(Text text)
    {
        if (text.text.Equals("")) return;

        if (text.text.Equals("impulse101"))
        {
            LocalStorage.ClearCache();
            return;
        }

        StartCoroutine(_sendQuestionCoroutine(text.text));
    }

    private IEnumerator _sendQuestionCoroutine(string text)
    {
        var wwwForm = new WWWForm();

        wwwForm.AddField("text", text);
        wwwForm.AddField("email", PlayerPrefs.GetString("email"));

        var www = new WWW(InfoStorage.Server + InfoStorage.QuestionApi, wwwForm);

        yield return www;

        if (www.error == null)
        {
            AnimationAssistant.QuestionAnimation(_form, _success);
        }
        else
        {
            _questions.Enqueue(text);
            EventStorage.Instance.ConnectionEstablished.AddListener(_sendAllCachedQuestions);
            AnimationAssistant.QuestionAnimation(_form, _failure);
        }
    }

    private void _saveQuestions()
    {
        if (_questions == null) _questions = new Queue<string>();

        if (_questions.Count != 0)
        {
            LocalStorage.Save(FileName, _questions);
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            _loadCachedAndDelete();
        }
        else
        {
            _saveQuestions();
        }
    }

    void OnApplicationQuit()
    {
        _saveQuestions();
    }

}
