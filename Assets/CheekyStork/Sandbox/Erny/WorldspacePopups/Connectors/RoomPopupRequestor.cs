using CheekyStork;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using static Room;

public class RoomPopupRequestor : PopupRequestor
{
    [SerializeField]
    private Room _room;

    [SerializeField]
    private UIConfig _uiConfig;

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

        foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType)))
        {
            if (result.Consumed[resourceType] > 0)
            {
                WorldspacePopupData popupData = CreatePopupDataConsumed(result, resourceType);
                RequestPopup(popupData);
            }
            if (result.Produced[resourceType] > 0)
            {
                WorldspacePopupData popupData = CreatePopupDataProduced(result, resourceType);
                RequestPopup(popupData);
            }
        }
    }

    private WorldspacePopupData CreatePopupDataConsumed(RoomActionResult result, ResourceType resourceType)
    {
        int valueConsumed = result.Consumed[resourceType];

        WorldspacePopupData popupData = new WorldspacePopupData(
            transform: transform,
            icon: _uiConfig.GetResourceIcon(resourceType),
            text: "-" + valueConsumed.ToString(),
            iconColor: Color.darkRed);

        return popupData;
    }

    private WorldspacePopupData CreatePopupDataProduced(RoomActionResult result, ResourceType resourceType)
    {
        int valueProduced = result.Produced[resourceType];

        WorldspacePopupData popupData = new WorldspacePopupData(
            transform: transform,
            icon: _uiConfig.GetResourceIcon(resourceType),
            text: "+" + valueProduced.ToString(),
            iconColor: _uiConfig.GetResourceColor(resourceType));

        return popupData;
    }
}