using System;
using CheekyStork.Logging;
using UnityEngine;
using UnityEngine.UI;

public class ModuleBuildElement : MonoBehaviour
{
    [SerializeField] private RoomType _roomType;
    [SerializeField] private UIConfig _uiConfig;
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    [SerializeField] private Room _room;
    
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
        _room.TryBuild(_roomType);
    }
    
    [Sirenix.OdinInspector.Button]
    public void UpdateReference()
    {
        if (_uiConfig == null)
            return;
        
        var sprite = _uiConfig.GetRoomIcon(_roomType);
        if (sprite == null)
        {
            this.LogError($"No sprite found for room type {_roomType} in UIConfig {_uiConfig.name}.", LogTag.UI);
            return;
        }
        
        _image.sprite = sprite;
    }
}
