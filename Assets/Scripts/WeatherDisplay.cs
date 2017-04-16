using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class WeatherDisplay : MonoBehaviour
{
	private string _temperature, _code = "3200";

	private Text _tempText;
	private Image _tempPic;
	private Sprite[] _icons;

	private const string Woeid = "582935";
	private const bool Celsius = true;
	private const int WeatherCodes = 49;
	private const string CelsiusChar = "°C";
	private const float WaitUpdate = 600f;
	private const bool AutoUpdate = true;

    private EventStorage _eventStorage;
	
	void Awake()
	{
		_initialize();
		_setWeatherLocal();

	}

	// Use this for initialization
	void Start ()
	{
        _eventStorage = EventStorage.Instance;
        _eventStorage.ConnectionEstablished.AddListener(_startWeatherUpdate);
        _eventStorage.ConnectionTerminated.AddListener(_stopWeatherUpdate);
		Debug.Log("Weather subscribed.");
	}
	
	// Update is called once per frame
	void Update () {

	}

	private void _startWeatherUpdate()
	{
		Debug.Log("Started weather update...");
		StartCoroutine(_updateWeatherCoroutine());
	}

	private void _stopWeatherUpdate()
	{
		StopCoroutine(_updateWeatherCoroutine());
	}

	private IEnumerator _updateWeatherCoroutine()
	{
		do
		{
			yield return StartCoroutine(_getWeatherDataFromYahooCoroutine());
			_updateWeatherGui();
			yield return new WaitForSeconds(WaitUpdate);
		} while (AutoUpdate);
	}

	private IEnumerator _getWeatherDataFromYahooCoroutine()
	{
		XmlDocument wData = new XmlDocument();

		var query = "select%20item.condition%20from%20weather.forecast%20where%20woeid%20%3D%20" + Woeid;

		if (Celsius)
		{
			query += "%20and%20u=%27c%27";
		}

		ServicePointManager.ServerCertificateValidationCallback = _myRemoteCertificateValidationCallback;

		var www = new WWW("https://query.yahooapis.com/v1/public/yql?q=" + query
+ "&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");

		yield return www;

		wData.LoadXml(www.text);

		XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
		manager.AddNamespace("yweather", @"http://xml.weather.yahoo.com/ns/rss/1.0");

		XmlNode nod = wData.SelectSingleNode("/query/results/channel", manager);
		_temperature = nod.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value;
		_code = nod.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["code"].Value;

		Debug.Log("Weather info from Yahoo updated. T: " + _temperature + ", Code: " + _code + ".");

	}

	private void _initializeWeatherIcons()
	{
		_icons = new Sprite[WeatherCodes];

		for (var i = 0; i < _icons.Length; i++)
		{
			_icons[i] = Resources.Load<Sprite>("Sprites/Weather/" + i);
		}
	}

	private void _initialize()
	{
		_tempText = transform.Find("TempText").gameObject.GetComponent<Text>();
		_tempPic = transform.Find("TempPic").gameObject.GetComponent<Image>();
		_initializeWeatherIcons();
	}

	private void _setWeatherLocal()
	{
		_tempPic.sprite = _icons[48];
		_tempText.text = "";
	}

	private void _updateWeatherGui()
	{
		_tempText.text = _temperature + CelsiusChar;

		var iconId = int.Parse(_code);

		if (iconId == 3200)
			iconId = 48;

		_tempPic.sprite = _icons[iconId];

		Debug.Log("Weather GUI updated.");
	}

	private bool _myRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		var isOk = true;
		// If there are errors in the certificate chain, look at each error to determine the cause.
		if (sslPolicyErrors == SslPolicyErrors.None) return isOk;
		foreach (X509ChainStatus x509cs in chain.ChainStatus)
		{
			if (x509cs.Status == X509ChainStatusFlags.RevocationStatusUnknown) continue;
			chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
			chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
			chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
			chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
			var chainIsValid = chain.Build((X509Certificate2)certificate);

			if (!chainIsValid)
			{
				isOk = false;
			}
		}
		return isOk;
	}
}
