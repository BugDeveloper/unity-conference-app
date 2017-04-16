using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GeolocationController : MonoBehaviour
{
    private const float IntervalUpdate = 1f;
    private const float WaitForGeo = 4f;
    private const bool AutoUpdate = true;

//    [SerializeField] private Text _text;

    private EventStorage _eventStorage;

    // Use this for initialization
    void Start()
    {
#if UNITY_ANDROID || UNITY_IPHONE

        _eventStorage = EventStorage.Instance;
        _eventStorage.ConnectionEstablished.AddListener(_startGeolocation);
        _eventStorage.ConnectionTerminated.AddListener(_stopGeolocation);
#endif
    }

    private void _stopGeolocation()
    {
        StopCoroutine("_geolocationCoroutine");
    }

    private void _startGeolocation()
    {
        StartCoroutine("_geolocationCoroutine");
    }

    private IEnumerator _geolocationCoroutine()
    {
        while (!Input.location.isEnabledByUser)
        {
            Debug.Log("Waiting for geolocation");
//            _text.text = "Location not enabled";
            yield return new WaitForSeconds(WaitForGeo);
        }

        Input.location.Start();

        while (Input.location.status == LocationServiceStatus.Initializing)
        {
//            _text.text = "Waiting for initializing";
            yield return new WaitForSeconds(2f);
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
//            _text.text = "LocationServiceStatus is Failed";
            yield break;
        }

//        _text.text = "LocationServiceStatus is Ready";

        do
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                var usrLoc = new GoogleMapMarker(GoogleMapMarker.GoogleMapMarkerSize.Mid,
                    GoogleMapColor.Blue, "U",
                    new GoogleMapLocation(Input.location.lastData.latitude, Input.location.lastData.longitude));

                _eventStorage.UserLocationDetermined.Invoke(usrLoc);

/*                _text.text = "Latitude: " + Input.location.lastData.latitude + ", Longitude: " +
                             Input.location.lastData.longitude;*/
            }
            yield return new WaitForSeconds(IntervalUpdate);
        } while (AutoUpdate);
    }
}