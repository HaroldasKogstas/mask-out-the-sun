using CheekyStork;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class RoomPopupRequestor : PopupRequestor
{
    //[SerializeField]
    //private ResourceGeneratingRoom asd;

    private void Awake()
    {
        StartCoroutine(PopupRequest(1f));
    }

    private void OnDestroy()
    {
        
    }


    public void OnThingHappened(int value)
    {
        WorldspacePopupData popupData = new WorldspacePopupData(transform, "+" + value.ToString());

        RequestPopup(popupData);
    }

    private IEnumerator PopupRequest(float delay)
    {
        yield return new WaitForSeconds(delay);

        int randomValue = Random.Range(2, 15);

        OnThingHappened(randomValue);

        StartCoroutine(PopupRequest(Random.Range(5f, 10f)));
    }
}