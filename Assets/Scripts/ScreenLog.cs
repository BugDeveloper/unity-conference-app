using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLog : MonoBehaviour
{
    private UnityEngine.UI.Text _logText;
    public int maxLogLength = 10000;

    public bool errorsOnly = false;
    public bool logStackTrace = false;

    private const string FileName = "bugs.dat";

    private Queue<string> _bugs;

    void Awake()
    {
        _bugs = new Queue<string>();
        _loadCachedAndDelete();
    }

/*    private IEnumerator _sendFps()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            var avgFrameRate = Time.frameCount / Time.time;

            var wwwForm = new WWWForm();

            wwwForm.AddField("text", "Average fps: " + avgFrameRate);
            wwwForm.AddField("email", "bug@report.com");

            var www = new WWW(InfoStorage.Server + InfoStorage.QuestionApi, wwwForm);

            yield return www;

            yield return new WaitForSeconds(40f);
        }

    }*/

    private void _loadCachedAndDelete()
    {
        if (!LocalStorage.FileExists(FileName)) return;
        _bugs = (Queue<string>) LocalStorage.Load(FileName);
        LocalStorage.DeleteFile(FileName);
    }

    private IEnumerator _sendBug(string text)
    {
        if (string.IsNullOrEmpty(text)) yield break;

        var wwwForm = new WWWForm();

        wwwForm.AddField("text", text);
        wwwForm.AddField("email", "bug@report.com");

        var www = new WWW(InfoStorage.Server + InfoStorage.QuestionApi, wwwForm);

        yield return www;

        if (www.error == null) yield break;

        _bugs.Enqueue(text);

        EventStorage.Instance.ConnectionEstablished.AddListener(_sendAllCachedQuestions);
    }

    private void _sendAllCachedQuestions()
    {
        StartCoroutine(_sendAllCachedQuestionsCoroutine());
    }


    private IEnumerator _sendAllCachedQuestionsCoroutine()
    {
        while (_bugs.Count != 0)
        {
            yield return StartCoroutine(_sendBug(_bugs.Dequeue()));
        }
    }

    private void _saveBugs()
    {
        if (_bugs == null)
            _bugs = new Queue<string>();
        if (_bugs.Count != 0)
        {
            LocalStorage.Save(FileName, _bugs);
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
            _saveBugs();
        }
    }

    void OnApplicationQuit()
    {
        _saveBugs();
    }

    void Start()
    {
        //StartCoroutine(_sendFps());

        if (_bugs.Count != 0)
        {
            _sendAllCachedQuestions();
        }

        {
            _logText = GetComponent<UnityEngine.UI.Text>();
            Application.logMessageReceived += (message, stackTrace, messageType) =>
            {
                var currentLog = _logText.text;
                var newLog = "";
                if (!errorsOnly)
                    newLog =
                        string.Format("{0} {1}",
                            (messageType == LogType.Log
                                ? "Log: "
                                : messageType == LogType.Warning
                                    ? "Warning: "
                                    : messageType == LogType.Error
                                        ? "Error: "
                                        : "Exception: "),
                            message + System.Environment.NewLine
                        );
                else
                {
                    if (messageType == LogType.Error)
                        newLog = "Error: " + message + System.Environment.NewLine;
                    else if (messageType == LogType.Exception)
                        newLog = "Exception: " + message + System.Environment.NewLine;
                }
                if (logStackTrace)
                    newLog += stackTrace + System.Environment.NewLine;
                newLog += currentLog;
                if (newLog.Length > maxLogLength)
                    newLog = newLog.Substring(0, maxLogLength);

                _logText.text = newLog;

                //StartCoroutine(_sendBug(newLog));
            };
        }
    }
}