using CheekyStork;
using System.Collections.Generic;
using UnityEngine;
using static Room;

public class RoomPopupRequestor : PopupRequestor
{
    [SerializeField]
    private Room _room;

    [SerializeField]
    private UIConfig _uiConfig;

    [SerializeField]
    private Vector3 _popupOriginOffset;

    private void Awake()
    {
        _room.ActionSucceeded += OnRoomAction;
    }

    private void OnDestroy()
    {
        _room.ActionSucceeded -= OnRoomAction;
    }

    public void OnRoomAction(Room room, RoomActionResult result)
    {
        // for every resource type, check result.produced and result.consumed, and create and send popups accordingly

        List< WorldspacePopupData> popupsToRequest = new List<WorldspacePopupData>();

        foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType)))
        {
            if (result.Consumed[resourceType] > 0)
            {
                WorldspacePopupData popupData = CreatePopupDataConsumed(result, resourceType);
                popupsToRequest.Add(popupData);
            }
            if (result.Produced[resourceType] > 0)
            {
                WorldspacePopupData popupData = CreatePopupDataProduced(result, resourceType);
                popupsToRequest.Add(popupData);
            }
        }

        RequestMultiPopup(new WorldspaceMultiPopupData(transform.position + _popupOriginOffset, popupsToRequest));
    }

    private WorldspacePopupData CreatePopupDataConsumed(RoomActionResult result, ResourceType resourceType)
    {
        int valueConsumed = result.Consumed[resourceType];

        Vector3 positionData = transform.position + _popupOriginOffset;

        WorldspacePopupData popupData = new WorldspacePopupData(
            position: positionData,
            icon: _uiConfig.GetResourceIcon(resourceType),
            text: "-" + valueConsumed.ToString(),
            iconColor: _uiConfig.GetResourceColor(resourceType));

        return popupData;
    }

    private WorldspacePopupData CreatePopupDataProduced(RoomActionResult result, ResourceType resourceType)
    {
        int valueProduced = result.Produced[resourceType];

        Vector3 positionData = transform.position + _popupOriginOffset;

        WorldspacePopupData popupData = new WorldspacePopupData(
            position: positionData,
            icon: _uiConfig.GetResourceIcon(resourceType),
            text: "+" + valueProduced.ToString(),
            iconColor: _uiConfig.GetResourceColor(resourceType));

        return popupData;
    }
}