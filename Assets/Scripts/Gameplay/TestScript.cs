using System;
using System.Collections.Generic;
using CheekyStork.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private List<ResourceData> _resources;
    [SerializeField] private ResourceBundle _resourceBundle;
    [SerializeField] private Room _room;

    [Button]
    private void BuildRoom()
    {
        bool wasSuccess = _room.TryBuild(RoomType.Miner);
        Debug.Log($"Room build was successful: {wasSuccess}");
    }

    private void Awake()
    {
        _room.Built += OnRoomBuilt;
        _room.ActionSucceeded += RoomActionSucceeded;
        _room.ActionFailed += RoomActionFailed;
    }

    private void OnDestroy()
    {
        _room.Built -= OnRoomBuilt;
        _room.ActionSucceeded -= RoomActionSucceeded;
        _room.ActionFailed -= RoomActionFailed;
    }

    private void RoomActionFailed(Room arg1, ResourceBundle arg2)
    {
        int amountOfIronConsumed = arg2[ResourceType.Iron];
        Debug.Log($"RoomActionFailed. Iron consumed: {amountOfIronConsumed}");
    }

    private void RoomActionSucceeded(Room arg1, Room.RoomActionResult arg2)
    {
        int amountOfIronConsumed = arg2.Consumed[ResourceType.Iron];
        Debug.Log($"RoomActionSucceeded. Iron consumed: {amountOfIronConsumed}");
    }

    private void OnRoomBuilt(Room obj)
    {
        int amountOfIronRemaining = ResourceManager.Instance.Resources[ResourceType.Iron];
        Debug.Log($"RoomBuild. Iron remaining: {amountOfIronRemaining}");
    }
}
