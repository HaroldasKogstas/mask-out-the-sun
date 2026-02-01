using UnityEngine;
using UnityEngine.UI;

public class ResourceElement : MonoBehaviour
{
    [SerializeField] private UIConfig _uiConfig;
    [SerializeField] private Image _elementImage;
    [SerializeField] private Button _button;
    
    private void Start()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnButtonClicked);
    }
    
    private void OnButtonClicked()
    {
        var resourceData = new ResourceData(_resourceType, 1);
        ResourceManager.Instance.AddResource(resourceData);
    }
    
    private ResourceType _resourceType;
    
    public void UpdateResourceType(ResourceType resourceType)
    {
        _resourceType = resourceType;
        _elementImage.sprite = _uiConfig.GetResourceIcon(resourceType);
        _elementImage.color = _uiConfig.GetResourceColor(resourceType);
    }
}
