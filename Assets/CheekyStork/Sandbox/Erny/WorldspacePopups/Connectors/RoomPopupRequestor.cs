using CheekyStork;
using Sirenix.OdinInspector;
using UnityEngine;

public class RoomPopupRequestor : PopupRequestor
{
    //[SerializeField]
    //private ResourceGeneratingRoom asd;

    private void Awake()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    public void OnThingHappened()
    {
        WorldspacePopupData popupData = new WorldspacePopupData(transform, "+10");

        RequestPopup(popupData);
    }
}