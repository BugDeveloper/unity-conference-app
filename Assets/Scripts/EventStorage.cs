using System;
using UnityEngine;
using UnityEngine.Events;

public class EventStorage : MonoBehaviour
{
    public static EventStorage Instance { get; private set; }

    public UnityEvent ConnectionEstablished { get; private set; }
    public UnityEvent ConnectionTerminated { get; private set; }
    public StringEvent LocationClicked { get; private set; }
    public UnityEvent MapToDefault { get; private set; }
    public UnityEvent DropUserLoc { get; private set; }
    public UnityEvent MapToUser { get; private set; }
    public UnityEvent OpenMaps { get; private set; }
    public UnityEvent InstagramClicked { get; private set; }
    public UnityEvent FeedbackClicked { get; private set; }
    public GoogleMarkerEvent UserLocationDetermined { get; private set; }
    public DateTimeEvent DateClicked { get; private set; }

    void Awake()
    {
        Instance = this;
        DropUserLoc = new UnityEvent();
        ConnectionEstablished = new UnityEvent();
        ConnectionTerminated = new UnityEvent();
        LocationClicked = new StringEvent();
        MapToDefault = new UnityEvent();
        OpenMaps = new UnityEvent();
        UserLocationDetermined = new GoogleMarkerEvent();
        MapToUser = new UnityEvent();
        InstagramClicked = new UnityEvent();
        FeedbackClicked = new UnityEvent();
        DateClicked = new DateTimeEvent();
    }
}

[Serializable]
public class StringEvent : UnityEvent<string>
{
}

[Serializable]
public class GoogleMarkerEvent : UnityEvent<GoogleMapMarker>
{
}

[Serializable]
public class DateTimeEvent : UnityEvent<DateTime>
{
}