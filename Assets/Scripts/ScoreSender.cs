using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSender : MonoBehaviour
{
    [SerializeField] private Sprite _activeStar, _inactiveStar;

    private const string FileName = "scores.dat";

    private CanvasGroup _cg;

    private Queue<Score> _scores;

    private List<int> _datesForVoting;

    void Awake()
    {
        _scores = new Queue<Score>();

        _cg = GetComponent<CanvasGroup>();

        _loadCachedAndDelete();
    }

    private void _loadCachedAndDelete()
    {
        if (!LocalStorage.FileExists(FileName)) return;

        _scores = (Queue<Score>) LocalStorage.Load(FileName);
        LocalStorage.DeleteFile(FileName);
    }

    void Start()
    {
        _initialize();

        if (_scores.Count != 0)
        {
            _sendAllCachedScores();
        }
    }

    private void _initialize()
    {
        var dt = DateTime.Now;

        if (dt.Hour >= 19 && PlayerPrefs.GetInt(dt.Year + "." + dt.Month + "." + dt.Day) == 0) return;

        _hideScore();
    }

    private void _hideScore()
    {
        _cg.blocksRaycasts = false;
        _cg.alpha = 0;
    }

    public void SendScore(int value)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Image>().sprite = _inactiveStar;
        }

        for (var i = 0; i < value; i++)
        {
            transform.GetChild(i).GetComponent<Image>().sprite = _activeStar;
        }

        var score = new Score(value, InfoStorage.DateTimeToUnixTimeStamp(DateTime.Now.Date));

        var dtnow = DateTime.Now;

        PlayerPrefs.SetInt(dtnow.Year + "." + dtnow.Month + "." + dtnow.Day, value);
        AnimationAssistant.Hide(_cg);

        StartCoroutine(_sendScoreCoroutine(score));
    }

    private void _sendAllCachedScores()
    {
        StartCoroutine(_sendAllCachedScoresCoroutine());
    }

    private IEnumerator _sendAllCachedScoresCoroutine()
    {
        while (_scores.Count != 0)
        {
            yield return StartCoroutine(_sendScoreCoroutine(_scores.Dequeue()));
        }
    }

    private IEnumerator _sendScoreCoroutine(Score score)
    {
        var wwwForm = new WWWForm();

        wwwForm.AddField("value", score.Value);
        wwwForm.AddField("date", score.Date);

        var www = new WWW(InfoStorage.Server + InfoStorage.ScoreApi, wwwForm);

        yield return www;

        Debug.Log("Server returned: " + www.text);

        if (www.error == null) yield break;

        _scores.Enqueue(score);
        EventStorage.Instance.ConnectionEstablished.AddListener(_sendAllCachedScores);
    }

    void OnApplicationQuit()
    {
        _saveScores();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            _loadCachedAndDelete();
        }
        else
        {
            _saveScores();
        }
    }

    private void _saveScores()
    {
        if (_scores == null) _scores = new Queue<Score>();

        if (_scores.Count != 0)
            LocalStorage.Save(FileName, _scores);
    }

    [Serializable]
    private class Score
    {
        public Score(int value, int date)
        {
            Value = value;
            Date = date;
        }

        public int Value { get; private set; }
        public int Date { get; private set; }
    }
}