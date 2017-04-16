using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddressMapper : MonoBehaviour, IPointerClickHandler {
	[SerializeField]
	private Text _address;

    [SerializeField] private ProgrammeExpansion _pe;

	private EventStorage _eventStorage;

	void Start() {
		_eventStorage = EventStorage.Instance;
	}

	public void OnPointerClick (PointerEventData eventData)
	{

		_eventStorage.LocationClicked.Invoke (_address.text);
	}
}
