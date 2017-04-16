using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using SimpleJSON;
using UnityEngine.Events;
using UnityEngine.UI;

public class GoogleMap : MonoBehaviour
{
    public enum MapType
    {
        RoadMap,
        Satellite,
        Terrain,
        Hybrid
    }

    [SerializeField] private CanvasGroup _plus, _minus;

    private UnityEvent _textureBufferUpdated;

    private const string FileName = "addresses.dat";

    private int _zoom = 14;
    private const int MinZoom = 12;
    private const int MaxZoom = 16;

    private bool _userLocUsing;

    private const bool DoubleResolution = true;
    private int _width;
    private int _height;

    private Texture2D _texture;

    private bool _updateLock;

    private Image _imageComp;

    public static readonly string MapDir = "maps";

    private MapType _mapType;
    private List<GoogleMapMarker> _markers;
    private GoogleMapMarker _userLocation;
    private List<GoogleMapPath> _paths;
    private GoogleMapMarker _centerLocation = GoogleMapMarker.Empty();
    private EventStorage _eventStorage;

    private UserNotificator _notificator;
    public static GoogleMap Instance { get; private set; }

    void Awake()
    {
        _height = 640;
        _width = (int) (Camera.main.aspect * _height);
        Instance = this;
        _paths = new List<GoogleMapPath>();
        _markers = new List<GoogleMapMarker>();
        _mapType = MapType.Terrain;
        _imageComp = GetComponent<Image>();

        _reinitializeTextureBuffer();

        _textureBufferUpdated = new UnityEvent();
        _textureBufferUpdated.AddListener(_assignNewSprite);
    }

    public void IncreaseZoomAndUpdate()
    {
        if (_updateLock) return;

        _updateLock = true;

        if (_zoom == MaxZoom - 1)
        {
            AnimationAssistant.Hide(_plus);
        }

        if (!AnimationAssistant.IsShown(_minus))
        {
            AnimationAssistant.Show(_minus);
        }

        _zoom++;

        _refreshMap();
    }

    public void DecreaseZoomAndUpdate()
    {
        if (_updateLock) return;

        _updateLock = true;

        if (_zoom == MinZoom + 1)
        {
            AnimationAssistant.Hide(_minus);
        }

        if (!AnimationAssistant.IsShown(_plus))
        {
            AnimationAssistant.Show(_plus);
        }

        _zoom--;
        _refreshMap();
    }

    void Start()
    {
        _notificator = UserNotificator.Instance;
        _eventStorage = EventStorage.Instance;
        _eventStorage.DropUserLoc.AddListener(_dropUserLocation);
        _eventStorage.LocationClicked.AddListener(_centralizeAt);
        _eventStorage.MapToDefault.AddListener(_dropCenterAndRefresh);
        _eventStorage.UserLocationDetermined.AddListener(_addUserLocationMarkerAndRefresh);
        _eventStorage.MapToUser.AddListener(_centerToUserAndRefresh);
        _eventStorage.OpenMaps.AddListener(_openGoogleMapsWithCurrentCenter);
        _initialize();
    }

    private void _dropUserLocation()
    {
        _userLocation = null;
        _refreshMap();
    }

    private void _openGoogleMapsWithCurrentCenter()
    {
#if UNITY_ANDROID || UNITY_IPHONE

        if (!_centerLocation.Location.Specified()) return;

        var query = "?q=";
        if (_centerLocation.Location.Address != "")
        {
            query += WWW.EscapeURL(_centerLocation.Location.Address);
        }
        else
        {
            query += WWW.EscapeURL(_centerLocation.Location.Latitude + "," + _centerLocation.Location.Longitude);
        }

        Application.OpenURL(InfoStorage.GoogleMapsApi + query);

#endif
    }

    private void _centerToUserAndRefresh()
    {
        if (_userLocation == null) return;

        _centerLocation.Location = _userLocation.Location;
        StartCoroutine(_refreshMapOnUserLocCoroutine());
    }

    private void _dropCenterAndRefresh()
    {
        _centerLocation.Color = GoogleMapColor.Green;
        _centerLocation = GoogleMapMarker.Empty();
        _getEventAdressesAndSetToMap();
    }

    private void _addUserLocationMarkerAndRefresh(GoogleMapMarker marker)
    {
        _userLocation = marker;
        _refreshMap();
    }

    private void _getEventAdressesAndSetToMap()
    {
        StartCoroutine(_getAdressesAndSetToMapCoroutine());
    }

    private void _initialize()
    {
        StartCoroutine(_initializeCoroutine());
    }

    private IEnumerator _initializeCoroutine()
    {
        if (_updateLock) yield break;

        _updateLock = true;
        _notificator.Notify("Получение адресов");
        yield return StartCoroutine(_getAdresses());
        _notificator.StopCurrent();
        _notificator.Notify("Кэширование карт");
        yield return StartCoroutine(_cacheAll());
        _notificator.StopCurrent();
        _notificator.Notify("Обновление карты");
        yield return StartCoroutine(_refreshMapCoroutine());
        _notificator.StopCurrent();
    }

    private IEnumerator _getAdressesAndSetToMapCoroutine()
    {
        if (_updateLock) yield break;

        _notificator.Notify("Обновление карт...");
        _updateLock = true;

        yield return StartCoroutine(_getAdresses());
        yield return StartCoroutine(_refreshMapCoroutine());
        _notificator.StopCurrent();
    }

    private void _centralizeAt(string address)
    {
        if (_updateLock) return;

        if (address.Equals("")) return;

        if (_centerLocation.Location.Specified())
            _centerLocation.Color = GoogleMapColor.Green;

        foreach (var marker in _markers)
        {
            if (!marker.Location.Address.Equals(address)) continue;

            _centerLocation = marker;
        }

        _centerLocation.Color = GoogleMapColor.Red;

        _refreshMap();
    }

    private IEnumerator _getAdresses()
    {
        var www = new WWW(InfoStorage.Server + InfoStorage.EventsApi + "getAll?api=address");
        yield return www;

        if (www.error == null)
        {
            _markers.Clear();
            var json = JSON.Parse(www.text);

            foreach (JSONNode jsonNode in json.AsArray)
            {
                string address = jsonNode["address"];

                if (address == null || address.Equals(""))
                    continue;

                var gml = new GoogleMapLocation(address);
                var gmm = new GoogleMapMarker(gml);

                _markers.Add(gmm);
            }

            LocalStorage.Save(FileName, _markers);
        }
        else
        {
            if (LocalStorage.FileExists(FileName))
            {
                _markers = (List<GoogleMapMarker>) LocalStorage.Load(FileName);
            }
        }
    }

    private void _refreshMap()
    {
        StartCoroutine(_refreshMapCoroutine());
    }

    private string _createApiQuery(GoogleMapLocation center, int zoom, bool includeUser)
    {
        var qs = "";

        if (center.Specified())
        {
            if (center.Address != "")
                qs += "center=" + WWW.UnEscapeURL(center.Address);
            else
            {
                qs += "center=" + WWW.UnEscapeURL(
                          string.Format("{0},{1}", center.Latitude, center.Longitude));
            }

            qs += "&zoom=" + zoom;
        }
        qs += "&language=ru";
        qs += "&size=" + WWW.UnEscapeURL(string.Format("{0}x{1}", _width, _height));
        qs += "&scale=" + (DoubleResolution ? "2" : "1");
        qs += "&maptype=" + _mapType.ToString().ToLower();

        foreach (var marker in _markers)
        {
            qs += "&markers=" + _markerToQuery(marker);
        }

        if (includeUser)
            qs += "&markers=" + _markerToQuery(_userLocation);

        foreach (var path in _paths)
        {
            qs += "&path=" + string.Format("weight:{0}|color:{1}", path.Weight, path.Color);
            if (path.Fill) qs += "|fillcolor:" + path.FillColor;
            foreach (var loc in path.Locations)
            {
                if (loc.Address != "")
                    qs += "|" + WWW.UnEscapeURL(loc.Address);
                else
                    qs += "|" + WWW.UnEscapeURL(string.Format("{0},{1}", loc.Latitude, loc.Longitude));
            }
        }

        return qs.Replace(" ", "%20");
    }

    private IEnumerator _cacheAll()
    {
        yield return StartCoroutine(_cacheLocationCoroutine(GoogleMapLocation.Empty()));

        foreach (var marker in _markers)
        {
            if (marker.Location.Address.Equals("")) continue;

            marker.Color = GoogleMapColor.Red;
            yield return StartCoroutine(_cacheLocationCoroutine(marker.Location));
            marker.Color = GoogleMapColor.Green;
        }
    }

    private IEnumerator _cacheLocationCoroutine(GoogleMapLocation location)
    {
        for (var i = MinZoom; i <= MaxZoom; i++)
        {
            var qs = _createApiQuery(location, i, false);

            if (LocalStorage.FileExists(MapDir + "/" + qs.GetHashCode()))
            {
                continue;
            }

            var query = _wrapQueryWithLinks(qs);

            ServicePointManager.ServerCertificateValidationCallback = _remoteCertificateValidationCallback;
            var webAsync = new WebAsync();
            var webRequest = HttpWebRequest.Create(query);

            webRequest.Method = "GET";

            yield return webAsync.GetResponse(webRequest);

            if (webAsync.requestState.errorMessage != null)
            {
                Debug.Log("Error:" + webAsync.requestState.errorMessage);
                yield break;
            }

            var reader = new BinaryReader(webAsync.requestState.webResponse.GetResponseStream());

            var bytesResponce = reader.ReadBytes(1 * 1024 * 1024 * 10);

            Destroy(_texture);
            _reinitializeTextureBuffer();
            _texture.LoadImage(bytesResponce);
            _textureBufferUpdated.Invoke();

            LocalStorage.Save(MapDir + "/" + qs.GetHashCode(), _texture.EncodeToJPG());
        }
    }

    private void _reinitializeTextureBuffer()
    {
        _texture = new Texture2D(2, 2, TextureFormat.PVRTC_RGBA4, false);
    }

    private IEnumerator _refreshMapCoroutine()
    {
        _userLocUsing = _userLocation != null;
        var qs = _createApiQuery(_centerLocation.Location, _zoom, _userLocUsing);
        var sensor = false;

        if (LocalStorage.FileExists(MapDir + "/" + qs.GetHashCode()))
        {
            var rawTexture = (byte[]) LocalStorage.Load(MapDir + "/" + qs.GetHashCode());

            _texture.LoadImage(rawTexture);
            _textureBufferUpdated.Invoke();
        }
        else
        {
            yield return StartCoroutine(_getTextureFromServer(_wrapQueryWithLinks(qs)));
        }

        _updateLock = false;
    }

    private void _assignNewSprite()
    {
        _imageComp.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width,
            _texture.height), new Vector2(0.5f, 0.5f), 100F, 0, SpriteMeshType.FullRect);
        _imageComp.preserveAspect = true;
    }

    private bool _remoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
        var isOk = true;

        if (sslPolicyErrors == SslPolicyErrors.None) return isOk;
        foreach (var chainStatus in chain.ChainStatus)
        {
            if (chainStatus.Status == X509ChainStatusFlags.RevocationStatusUnknown) continue;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan (0, 1, 0);
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
            var chainIsValid = chain.Build ((X509Certificate2)certificate);

            if (!chainIsValid) {
                isOk = false;
            }
        }
        return isOk;
    }

    private IEnumerator _getTextureFromServer(string query)
    {
        ServicePointManager.ServerCertificateValidationCallback = _remoteCertificateValidationCallback;
        var webAsync = new WebAsync();
        var webRequest = HttpWebRequest.Create(query);

        webRequest.Method = "GET";

        yield return webAsync.GetResponse(webRequest);

        if (webAsync.requestState.errorMessage != null)
        {
            Debug.Log("Error:" + webAsync.requestState.errorMessage);
            
            if (_userLocUsing)
            {
                _userLocation = null;
                _refreshMap();
            }

            yield break;
        }

        var reader = new BinaryReader(webAsync.requestState.webResponse.GetResponseStream());

        var bytesResponce = reader.ReadBytes(1 * 1024 * 1024 * 10);

        Destroy(_texture);
        _reinitializeTextureBuffer();
        _texture.LoadImage(bytesResponce);
         _textureBufferUpdated.Invoke();
    }

    private string _wrapQueryWithLinks(string qs)
    {
        return InfoStorage.GoogleStaticMapsApi + "?" + qs + "&key=" + InfoStorage.GoogleMapApiKey;
    }

    private IEnumerator _refreshMapOnUserLocCoroutine()
    {
        if (_userLocation == null) yield break;

        var qs = _createApiQuery(_userLocation.Location, _zoom, true);

        yield return StartCoroutine(_getTextureFromServer(_wrapQueryWithLinks(qs)));
    }

    private string _markerToQuery(GoogleMapMarker marker)
    {
        var query = string.Format("size:{0}|color:{1}|label:{2}", marker.Size.ToString().ToLower(),
            marker.Color.ToString().ToLower(),
            marker.Label.ToUpper());

        if (marker.Location.Address != "")
            query += "|" + WWW.UnEscapeURL(marker.Location.Address);
        else
            query += "|" + WWW.UnEscapeURL(
                         string.Format("{0},{1}", marker.Location.Latitude, marker.Location.Longitude));
        return query;
    }
}

public enum GoogleMapColor
{
    Black,
    Brown,
    Green,
    Purple,
    Yellow,
    Blue,
    Gray,
    Orange,
    Red,
    White
}

[Serializable]
public class GoogleMapLocation
{
    private string _address = "";

    public GoogleMapLocation(string address)
    {
        _address = address;
    }

    public static GoogleMapLocation Empty()
    {
        return new GoogleMapLocation();
    }

    private GoogleMapLocation()
    {
    }

    public GoogleMapLocation(float latitude, float longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public bool Specified()
    {
        return !(Address.Equals("") && (Latitude == 0 || Longitude == 0));
    }

    public void Drop()
    {
        _address = "";
        Latitude = 0;
        Longitude = 0;
    }

    public string Address
    {
        get { return _address; }
        private set { _address = value; }
    }

    public float Latitude { get; private set; }
    public float Longitude { get; private set; }
}

[Serializable]
public class GoogleMapMarker
{
    private GoogleMapMarker()
    {
        Location = GoogleMapLocation.Empty();
    }

    public static GoogleMapMarker Empty()
    {
        return new GoogleMapMarker();
    }

    public GoogleMapMarker(GoogleMapMarkerSize size, GoogleMapColor color, string label, GoogleMapLocation location)
    {
        Location = location;
        _size = size;
        _color = color;
        _label = label;
    }

    public enum GoogleMapMarkerSize
    {
        Tiny,
        Small,
        Mid
    }

    private GoogleMapMarkerSize _size = GoogleMapMarkerSize.Mid;
    private GoogleMapColor _color = GoogleMapColor.Green;
    private string _label = "";

    public GoogleMapLocation Location { get; set; }

    public GoogleMapMarker(GoogleMapLocation location)
    {
        Location = location;
    }

    public GoogleMapColor Color
    {
        get { return _color; }
        set { _color = value; }
    }

    public string Label
    {
        get { return _label; }
        set { _label = value; }
    }

    public GoogleMapMarkerSize Size
    {
        get { return _size; }
        set { _size = value; }
    }
}

[Serializable]
public class GoogleMapPath
{
    private int _weight = 5;
    public GoogleMapColor Color { get; set; }
    private bool _fill = false;
    public GoogleMapColor FillColor { get; set; }
    public GoogleMapLocation[] Locations { get; set; }

    public int Weight
    {
        get { return _weight; }
        set { _weight = value; }
    }

    public bool Fill
    {
        get { return _fill; }
        set { _fill = value; }
    }
}