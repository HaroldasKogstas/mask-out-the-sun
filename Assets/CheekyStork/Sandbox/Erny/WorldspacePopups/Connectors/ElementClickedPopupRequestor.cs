using CheekyStork;
using UnityEngine;

public class ElementClickedPopupRequestor : PopupRequestor
{
    [SerializeField]
    private ResourceElement _clickableElement;

    [SerializeField]
    private UIConfig _uiConfig;

    private void Awake()
    {
        _clickableElement.OnElementClicked += OnElementClicked;
    }

    private void OnDestroy()
    {
        _clickableElement.OnElementClicked -= OnElementClicked;
    }

    public void OnElementClicked(ResourceType resourceType, int value)
    {
        Vector3 positionData = _clickableElement.transform.position;

        WorldspacePopupData popupData = new WorldspacePopupData(
            position: positionData,
            icon: _uiConfig.GetResourceIcon(resourceType),
            text: "+" + value.ToString(),
            iconColor: _uiConfig.GetResourceColor(resourceType));

        RequestPopup(popupData);
    }
}