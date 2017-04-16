using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewsController : ContentUpdater
{
    [SerializeField] private GameObject _newsList, _newsItem, _listEnd, _image, _newsEnd;

    private List<NewsItem> _news;

    public static Sprite Temp { get; private set; }
    private CanvasGroup _cg;

    private const string FileName = "news.dat";

    public override UnityEvent UpdateStart { get; protected set; }
    public override UnityEvent UpdateEnd { get; protected set; }

    void Awake()
    {
        LocalStorage.Initialize();
        Temp = Resources.Load<Sprite>("Sprites/Temp/feed_photo_01");
        _cg = GetComponent<CanvasGroup>();
        _news = new List<NewsItem>();
        UpdateEnd = new UnityEvent();
        UpdateStart = new UnityEvent();
    }

    // Use this for initialization
    void Start()
    {
        _downloadAndInitialize();

        _cg.alpha = 0f;
        _cg.blocksRaycasts = false;
    }

    private void _saveLocal()
    {
        LocalStorage.Save(FileName, _news);
    }

    private void _loadLocal()
    {
        if (!LocalStorage.FileExists(FileName)) return;

        _news = (List<NewsItem>) LocalStorage.Load(FileName);
        _initializeNews();
    }

    private void _downloadAndInitialize()
    {
        StartCoroutine(_downloadAndInitializeNewsCoroutine());
    }

    private IEnumerator _updateCoroutine()
    {
        UpdateStart.Invoke();
        yield return StartCoroutine(_downloadAndInitializeNewsCoroutine());
        UpdateEnd.Invoke();
    }

    private IEnumerator _downloadAndInitializeNewsCoroutine()
    {
        Debug.Log("Started update news...");
        var www = new WWW(InfoStorage.Server + InfoStorage.NewsApi + "getAll?api");

        yield return www;

        if (_newsList.transform.childCount != 0)
        {
            _destroyTransformChildren(_newsList.transform);
        }

        if (_news.Count != 0)
        {
            _news.Clear();
        }

        if (www.error == null)
        {
            var json = JSON.Parse(www.text);

            foreach (JSONNode jsonNode in json.AsArray)
            {
                var date = int.Parse(jsonNode["date"]);
                string id = jsonNode["id"];
                string info = jsonNode["info"];
                string imageLink = jsonNode["image"];

                info = InfoStorage.ProcessTags(info);

                if (imageLink == null)
                {
                    imageLink = "";
                }

                var newsItem = new NewsItem(id, date, info, imageLink);

                _news.Add(newsItem);
            }

            _news.Sort((x, y) => y.Date.CompareTo(x.Date));

            _initializeNews();
            _saveLocal();
            _initializeListEnd();

        }
        else
        {
            _loadLocal();
        }
    }

    private void _initializeNews()
    {
        foreach (var newsItem in _news)
        {
            _initializeNewsItem(newsItem);
        }
    }

    private string[] _removeLast(string[] arr)
    {
        var res = new string[arr.Length - 1];
        for (var i = 0; i < arr.Length - 1; i++)
        {
            res[i] = arr[i];
        }

        return res;
    }

    private void _initializeListEnd()
    {
        Instantiate(_newsEnd, _newsList.transform, false);
    }

    private void _initializeNewsItem(NewsItem newsItem)
    {
        var item = Instantiate(_newsItem, _newsList.transform, false);

        item.transform.Find("Date").GetComponent<Text>().text =
            InfoStorage.UnixTimeStampToDateTime(newsItem.Date).ToLongDateString() + " " + InfoStorage
                .UnixTimeStampToDateTime(newsItem.Date)
                .ToLongTimeString();
        item.transform.Find("Text").GetComponent<Text>().text = newsItem.Info;
        item.transform.Find("More").GetComponent<NewsExpansion>().InitializeExpansion();

        var imageContainer = item.transform.Find("ScrollSnap");
        var parentRt = imageContainer.GetComponent<RectTransform>();

        var image = newsItem.Image;

        if (image == "")
        {
            var imageObj = Instantiate(_image, imageContainer, false);
            imageObj.GetComponent<RectTransform>().sizeDelta = new Vector2(parentRt.sizeDelta.x, parentRt.sizeDelta.y);

            imageObj.GetComponent<Image>().sprite = Temp;
        }
        else
        {
            var imageObj = Instantiate(_image, imageContainer, false);
            imageObj.GetComponent<RectTransform>().sizeDelta = new Vector2(parentRt.sizeDelta.x, parentRt.sizeDelta.y);

            StartCoroutine(imageObj.GetComponent<ImageDownloader>().DownloadImage(newsItem.Id, image));
        }
    }

    public override void ContentUpdate()
    {
        StartCoroutine(_updateCoroutine());
    }
}