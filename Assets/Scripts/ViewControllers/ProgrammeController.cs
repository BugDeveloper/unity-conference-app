using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgrammeController : ContentUpdater
{
    [SerializeField] private GameObject _programmeItemNewScreen,
        _programmeItemSpoiler,
        _progammeExtension,
        _listEnd,
        _extensionList;

    [SerializeField] private List<GameObject> _programmeLists;

    [SerializeField] private bool _newScreenExtension;

    private List<List<ProgrammeItem>> _programme;
    private int _activeEventIndex = -1;

    private EventStorage _eventStorage;

    private const string FormatDateTime = "hh:mm";
    private const string FileName = "prog.dat";

    public override UnityEvent UpdateStart { get; protected set; }
    public override UnityEvent UpdateEnd { get; protected set; }

    void Awake()
    {
        _programme = new List<List<ProgrammeItem>>
        {
            new List<ProgrammeItem>(),
            new List<ProgrammeItem>(),
            new List<ProgrammeItem>(),
            new List<ProgrammeItem>()
        };

        UpdateEnd = new UnityEvent();
        UpdateStart = new UnityEvent();
    }

    // Use this for initialization
    void Start()
    {
        _eventStorage = EventStorage.Instance;

        _downloadAndInitializeEvents();

        _startEventPointerUpdate();
    }

    private void _loadLocal()
    {
        if (!LocalStorage.FileExists(FileName)) return;

        _programme = (List<List<ProgrammeItem>>) LocalStorage.Load(FileName);
        _initializeEvents();
    }

    private void _saveLocal()
    {
        LocalStorage.Save(FileName, _programme);
    }

    private void _downloadAndInitializeEvents()
    {
        StartCoroutine(_downloadAndInitializeEventsCoroutine());
    }

    private IEnumerator _downloadAndInitializeEventsCoroutine()
    {
        Debug.Log("Started update events on " + InfoStorage.Server + InfoStorage.EventsApi + "getAll?api");
        var www = new WWW(InfoStorage.Server + InfoStorage.EventsApi + "getAll?api");

        yield return www;

        foreach (var list in _programme)
        {
            if (list.Count != 0)
                list.Clear();
        }

        foreach (var programmeList in _programmeLists)
        {
            if (programmeList.transform.childCount != 0)
            {
                _destroyTransformChildren(programmeList.transform);
            }
        }

        if (www.error == null)
        {
            var json = JSON.Parse(www.text);

            foreach (JSONNode jsonNode in json.AsArray)
            {
                var start = int.Parse(jsonNode["start"]);
                var end = int.Parse(jsonNode["end"]);
                string title = jsonNode["title"];
                string info = jsonNode["info"];
                string address = jsonNode["address"];

                info = InfoStorage.ProcessTags(info);

                var item = new ProgrammeItem(start, end, title, address, info);

                if (!InfoStorage.IsDateValid(item.Start))
                    continue;

                _addToProgramme(item);
            }

            foreach (var programmeList in _programme)
            {
                programmeList.Sort((x, y) => x.Start.CompareTo(y.Start));
            }
            _initializeEvents();
            _saveLocal();
        }
        else
        {
            _loadLocal();
        }

    }

    private void _initializeEvents()
    {
        foreach (var list in _programme)
        {
            foreach (var programmeItem in list)
            {
                if (_newScreenExtension)
                {
                    _initializeEventNewScreenExtension(programmeItem);
                }
                else
                {
                    _initializeEventSpoilerExtension(programmeItem);
                }
            }
        }
        _initializeListEndings();
    }

    private GameObject _getRelativeGameObject(DateTime dt)
    {
        if (dt.Month != 4 && dt.Year != 2017) return null;

        switch (dt.Day)
        {
            case 12:
                return _programmeLists[0];
            case 13:
                return _programmeLists[1];
            case 14:
                return _programmeLists[2];
            case 15:
                return _programmeLists[3];
            default:
                return null;
        }
    }

    private List<ProgrammeItem> _getRelativeList(DateTime dt)
    {
        if (dt.Month != 4 && dt.Year != 2017) return null;

        switch (dt.Day)
        {
            case 12:
                return _programme[0];
            case 13:
                return _programme[1];
            case 14:
                return _programme[2];
            case 15:
                return _programme[3];
            default:
                return null;
        }
    }

    private void _addToProgramme(ProgrammeItem item)
    {
        var list = _getRelativeList(InfoStorage.UnixTimeStampToDateTime(item.Start));
        if (list != null)
            list.Add(item);
    }

    private void _startEventPointerUpdate()
    {
        StartCoroutine(_eventPointerLauncher());
        Debug.Log("Started event pointer update.");
    }

    private void _setProgrammeEventActive(Transform programmeEvent, bool active)
    {
        var on = programmeEvent.FindChild("Item/Square/On").GetComponent<Image>();

        if (active)
            _setImageAlpha(on, 1);
        else
            _setImageAlpha(on, 0);
    }

    private void _setImageAlpha(Image image, float alpha)
    {
        var newAlpha = image.color;
        newAlpha.a = alpha;
        image.color = newAlpha;
    }

    private IEnumerator _eventPointerLauncher()
    {
        while(true)
        {
            _eventPointerUpdate();
            yield return new WaitForSeconds(2f);
        }
    }

    private void _eventPointerUpdate()
    {
            Debug.Log("Pointer update");

            var dt = new DateTime(2017, 4, 14, 20, 30, 0);
            var list = _getRelativeList(dt);

            if (list == null)
            {
                Debug.Log("Pointer update on null list break");
                return;
            }

            var currentEventIndex = _binarySearchOnProgramme(dt, 0,
                list.Count - 1, list);

            Debug.Log("Current event id: " + currentEventIndex);

            var gameObjList = _getRelativeGameObject(dt);

            if (gameObjList == null) return;

            if (currentEventIndex != -1 && currentEventIndex != _activeEventIndex)
            {
                var currentProgrammeEvent = gameObjList.transform.GetChild(currentEventIndex);

                _setProgrammeEventActive(currentProgrammeEvent, true);

                if (_activeEventIndex != -1)
                {
                    var activeProgrammeEvent = gameObjList.transform.GetChild(_activeEventIndex);
                    _setProgrammeEventActive(activeProgrammeEvent, false);
                }

                _activeEventIndex = currentEventIndex;
            }
    }

    private int _binarySearchOnProgramme(DateTime time, int low, int high, List<ProgrammeItem> list)
    {
        if (list.Count == 0) return -1;

        if (low > high)
            return -1;

        var mid = low + (high - low) / 2;

        switch (_compareProgrammeItemByDate(list[mid], time))
        {
            case -1:
                return _binarySearchOnProgramme(time, mid + 1, high, list);
            case 1:
                return _binarySearchOnProgramme(time, low, mid - 1, list);
            default:
                return mid;
        }
    }

    private int _compareProgrammeItemByDate(ProgrammeItem pi, DateTime time)
    {
        if (InfoStorage.UnixTimeStampToDateTime(pi.Start).CompareTo(time) == -1 && InfoStorage.UnixTimeStampToDateTime(pi.End).CompareTo(time) == -1)
            return -1;

        if (InfoStorage.UnixTimeStampToDateTime(pi.Start).CompareTo(time) == 1 && InfoStorage.UnixTimeStampToDateTime(pi.End).CompareTo(time) == 1)
            return 1;

        return 0;
    }

    private void _initializeEventSpoilerExtension(ProgrammeItem programmeItem)
    {
        var gameObjList = _getRelativeGameObject(InfoStorage.UnixTimeStampToDateTime(programmeItem.Start));

        if (gameObjList == null) return;

        var item = Instantiate(_programmeItemSpoiler, gameObjList.transform, false);

        item.transform.Find("Item/Square/Time/Start").GetComponent<Text>().text =
            InfoStorage.UnixTimeStampToDateTime(programmeItem.Start).ToShortTimeString();
        item.transform.Find("Item/Square/Time/End").GetComponent<Text>().text =
            InfoStorage.UnixTimeStampToDateTime(programmeItem.End).ToShortTimeString();
        item.transform.Find("Item/Information/Title").GetComponent<Text>().text = programmeItem.Title;
        item.transform.Find("Item/Information/Address").GetComponent<Text>().text = programmeItem.Address;

        if (!string.IsNullOrEmpty(programmeItem.Info))
        {
            item.transform.Find("Expansion/Info/Text").GetComponent<Text>().text = programmeItem.Info;
        }
        else
        {
            item.transform.Find("Expansion/Info").gameObject.SetActive(false);
        }

        item.transform.Find("Expansion/Time/Text").GetComponent<Text>().text =
            InfoStorage.UnixTimeStampToDateTime(programmeItem.Start).ToLongDateString() + " " + InfoStorage
                .UnixTimeStampToDateTime(programmeItem.Start).ToShortTimeString() + "-" +
            InfoStorage.UnixTimeStampToDateTime(programmeItem.End).ToShortTimeString();

        if (!string.IsNullOrEmpty(programmeItem.Address))
        {
            item.transform.Find("Expansion/Location/Text").GetComponent<Text>().text = programmeItem.Address;
        }
        else
        {
            item.transform.Find("Expansion/Location").gameObject.SetActive(false);
        }
    }

    private void _initializeEventNewScreenExtension(ProgrammeItem programmeItem)
    {
        var gameObjList = _getRelativeGameObject(InfoStorage.UnixTimeStampToDateTime(programmeItem.Start));

        if (gameObjList == null) return;

        var item = Instantiate(_programmeItemNewScreen, gameObjList.transform, false);
        var extension = Instantiate(_progammeExtension, _extensionList.transform, false);

        item.transform.Find("Item/Information").GetComponent<OpenExtension>().ProgrammeExpansion =
            extension.GetComponent<ProgrammeExpansion>();

        item.transform.Find("Item/Square/Time/Start").GetComponent<Text>().text =
            InfoStorage.UnixTimeStampToDateTime(programmeItem.Start).ToShortDateString();
        item.transform.Find("Item/Square/Time/End").GetComponent<Text>().text =
            InfoStorage.UnixTimeStampToDateTime(programmeItem.End).ToShortTimeString();
        item.transform.Find("Item/Information/Title").GetComponent<Text>().text = programmeItem.Title;
        item.transform.Find("Item/Information/Address").GetComponent<Text>().text = programmeItem.Address;

        extension.transform.Find("Expansion/Info/Text").GetComponent<Text>().text = programmeItem.Info;
        extension.transform.Find("Expansion/Time/Text").GetComponent<Text>().text =
            InfoStorage.UnixTimeStampToDateTime(programmeItem.Start).ToLongDateString() + " " + InfoStorage
                .UnixTimeStampToDateTime(programmeItem.Start)
                .ToString(FormatDateTime) + "-" +
            InfoStorage.UnixTimeStampToDateTime(programmeItem.End).ToString(FormatDateTime);

        extension.transform.Find("Expansion/Location/Text").GetComponent<Text>().text = programmeItem.Address;
    }

    private void _initializeListEndings()
    {
        foreach (var list in _programmeLists)
        {
            Instantiate(_listEnd, list.transform, false);
        }
    }

    private IEnumerator _updateCoroutine()
    {
        UpdateStart.Invoke();
        yield return StartCoroutine(_downloadAndInitializeEventsCoroutine());
        UpdateEnd.Invoke();
    }

    public override void ContentUpdate()
    {
        StartCoroutine(_updateCoroutine());
    }
}