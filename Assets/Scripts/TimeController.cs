using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
	public static TimeController Instance;
	public UnityEvent GotTimeFromInternet { get; private set; }
	public DateTime CurrentTime { get; private set; }
    public bool TimeUpdated { get; private set; }

    private Text _timeText;

	private string _region;

    private EventStorage _eventStorage;

	void Awake()
	{
		Instance = this;
		GotTimeFromInternet = new UnityEvent();
		_timeText = gameObject.GetComponent<Text>();
		_setTimeLocal();
	}

	void Start()
	{
        _eventStorage = EventStorage.Instance;
        _eventStorage.ConnectionEstablished.AddListener(_updateTimeFromInternet);
		StartCoroutine(_localUpdateTimeCoroutine());
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void _updateTimeFromInternet()
	{
		_setTimeFromInternet();
		_updateTimeGui();
		GotTimeFromInternet.Invoke();
	    TimeUpdated = true;
        _eventStorage.ConnectionEstablished.RemoveListener(_updateTimeFromInternet);
	}

	private IEnumerator _localUpdateTimeCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(60f);
			CurrentTime = CurrentTime.AddMinutes(1);
			_timeText.text = CurrentTime.TimeOfDay.ToString().Substring(0, 5) + _region;
		}
	}

	private void _setTimeLocal()
	{
		CurrentTime = DateTime.Now;
		_region = "";
		_updateTimeGui();
	}

	private void _updateTimeGui()
	{
		_timeText.text = CurrentTime.TimeOfDay.ToString().Substring(0, 5) + _region;
	}

	private void _setTimeFromInternet()
	{
		//TODO
		DateTime utcTime = _getNistTime();
		CurrentTime = utcTime.AddHours(1);
		_region = "(Кнн)";
	}

	private DateTime _getNistTime()
	{
		DateTime dateTime = DateTime.MinValue;

		HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://nist.time.gov/actualtime.cgi?lzbc=siqm9b");
		request.Method = "GET";
		request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore); //No caching
		HttpWebResponse response = (HttpWebResponse) request.GetResponse();

		if (response.StatusCode == HttpStatusCode.OK)
		{
			StreamReader stream = new StreamReader(response.GetResponseStream());
			string html = stream.ReadToEnd(); //<timestamp time=\"1395772696469995\" delay=\"1395772696469995\"/>
			string time = Regex.Match(html, @"(?<=\btime="")[^""]*").Value;
			double milliseconds = Convert.ToInt64(time) / 1000.0;
			dateTime = new DateTime(1970, 1, 1).AddMilliseconds(milliseconds);
		}

		return dateTime;
	}
}