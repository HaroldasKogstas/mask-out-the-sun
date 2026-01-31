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
        this.LogFatal("BuildRoom STEP 0", LogTag.AI);
        
        Debug.LogError("BuildRoom STEP 1");
        bool wasSuccess = _room.TryBuild(RoomType.Assembler);
        Debug.Log($"BuildRoom STEP 2: Success = {wasSuccess}");
    }

    private void Awake()
    {
        Debug.LogError($"STEP BD");
    }

    private void Start()
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
        Debug.Log($"Amount of Iron consumed in failed action: {amountOfIronConsumed}");
    }

    private void RoomActionSucceeded(Room arg1, Room.RoomActionResult arg2)
    {
        int amountOfIronConsumed = arg2.Consumed[ResourceType.Iron];
        Debug.Log($"Amount of Iron consumed in action: {amountOfIronConsumed}");
    }

    private void OnRoomBuilt(Room obj)
    {
        int amountOfIronRemaining = ResourceManager.Instance.Resources[ResourceType.Iron];
        Debug.Log($"Amount of Iron remaining: {amountOfIronRemaining}");
    }
}
