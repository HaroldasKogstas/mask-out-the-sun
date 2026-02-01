using UnityEngine;
using UnityEngine.UI;

public class IconSetterBasedOnUIData : MonoBehaviour
{
    [SerializeField]
    private ResourceType _resourceType;

    [SerializeField]
    private UIConfig _uiConfig;

    [SerializeField]
    private Image _iconDisplay;

    private void Awake()
    {
        SetupVisuals();
    }

    private void SetupVisuals()
    {
        _iconDisplay.sprite = _uiConfig.GetResourceIcon(_resourceType);
        _iconDisplay.color = _uiConfig.GetResourceColor(_resourceType);
    }
}