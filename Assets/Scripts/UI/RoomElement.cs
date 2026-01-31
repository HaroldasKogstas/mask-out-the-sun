using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomElement : MonoBehaviour
{
    [SerializeField] private UiSpriteSheetAnimator uiSpriteSheetAnimator;
    [SerializeField] private Room _room;
    [SerializeField] private Sprite _upgradeEmptyBubbleSprite;
    [SerializeField] private Sprite _upgradeFilledBubbleSprite;
    [SerializeField] private Image _roomImage;
    [SerializeField] private List<Image> _resourceImages;
    [SerializeField] private List<Image> _upgradeBubbles;
    [SerializeField] private UIConfig _uiConfig;

    private void Start()
    {
        _room.Built += OnRoomBuilt;
        UpdateResourceImages();
    }

    private void OnDestroy()
    {
        _room.Built -= OnRoomBuilt;
    }

    private void UpdateResourceImages()
    {
        var underlyingElementType = _room.UnderlyingElement;
        foreach (Image image in _resourceImages)
        {
            image.sprite = _uiConfig.GetResourceIcon(underlyingElementType);
            image.color = _uiConfig.GetResourceColor(underlyingElementType);
        }
    }

    private void UpdateRoomImage(RoomType roomType)
    {
        _roomImage.sprite = _uiConfig.GetRoomIcon(roomType);
        var activeFrames = _uiConfig.GetRoomActiveFrames(roomType);
        uiSpriteSheetAnimator.SetFrames(activeFrames);
    }

    private void OnRoomBuilt(Room room)
    {
        UpdateRoomImage(room.Type);
    }
}
