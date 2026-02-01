using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class RoomStateManager : MonoBehaviour
{
    [SerializeField] private List<Room> _rooms;
    
    [Button]
    public void GetAllReferencesInChildren()
    {
        _rooms = new List<Room>(GetComponentsInChildren<Room>());
    }

    private void Awake()
    {
        var numberOfRoomsToBeUnlocked = RoomUnlockManager.Instance.UnlockedRoomsCount;
        for (int i = 0; i < _rooms.Count; i++)
        {
            if (i < numberOfRoomsToBeUnlocked)
            {
                _rooms[i].ForceLockState(false);
            }
            else
            {
                _rooms[i].ForceLockState(true);
            }

            if (i % 3 == 0)
            {
                _rooms[i].ForceUnderlyingElement(ResourceType.Iron);
            }
            else if (i % 3 == 1)
            {
                _rooms[i].ForceUnderlyingElement(ResourceType.Coal);
            }
            else if (i % 3 == 2)
            {
                _rooms[i].ForceUnderlyingElement(ResourceType.Tungsten);
            }
        }
    }
}
