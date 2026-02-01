using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomElement : MonoBehaviour
{
    public enum RoomElementState
    {
        locked,
        unlocked,
        built
    }
    
    [SerializeField] private UiSpriteSheetAnimator uiSpriteSheetAnimator;
    [SerializeField] private Room _room;
    [SerializeField] private Sprite _upgradeEmptyBubbleSprite;
    [SerializeField] private Sprite _upgradeFilledBubbleSprite;
    [SerializeField] private Image _roomImage;
    [SerializeField] private Image _progressImage;
    [SerializeField] private List<ResourceElement> _resourceElements;
    [SerializeField] private List<Image> _upgradeBubbles;
    [SerializeField] private UIConfig _uiConfig;
    [SerializeField] private CanvasGroup _lockedCanvasGroup;
    [SerializeField] private CanvasGroup _unlockedCanvasGroup;
    [SerializeField] private CanvasGroup _builtCanvasGroup;
    [SerializeField] private TextMeshProUGUI _roomNameText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _destroyButton;

    private void Awake()
    {
        _room.Built += OnRoomBuilt;
        _room.Unlocked += OnRoomUnlocked;
        _room.Upgraded += OnRoomUpgraded;
        _upgradeButton.onClick.AddListener(TryUpgradeRoom);
    }

    private void Start()
    {
        UpdateResourceImages();
        UpdateState();
    }

    private void OnDestroy()
    {
        _room.Built -= OnRoomBuilt;
        _room.Unlocked -= OnRoomUnlocked;
        _room.Upgraded -= OnRoomUpgraded;
        _upgradeButton.onClick.RemoveListener(TryUpgradeRoom);
    }

    private void Update()
    {
        if (_room.IsBuilt)
        {
            float progress = _room.Progress01;
            _progressImage.fillAmount = progress;
        }
    }

    private void TryUpgradeRoom()
    {
        _room.TryUpgrade();
    }

    private void OnRoomUpgraded(Room room, int level)
    {
        for (int i = 0; i < _upgradeBubbles.Count; i++)
        {
            if (i < level)
            {
                _upgradeBubbles[i].sprite = _upgradeFilledBubbleSprite;
            }
            else
            {
                _upgradeBubbles[i].sprite = _upgradeEmptyBubbleSprite;
            }
        }
    }

    private void OnRoomUnlocked(Room obj)
    {
        UpdateState();
    }

    private void ToggleCanvasGroup(CanvasGroup canvasGroup, bool isActive)
    {
        canvasGroup.alpha = isActive ? 1 : 0;
        canvasGroup.blocksRaycasts = isActive;
        canvasGroup.interactable = isActive;
        canvasGroup.gameObject.SetActive(isActive);
    }
    
    private void UpdateState()
    {
        if (_room.IsLocked)
        {
            ToggleCanvasGroup(_lockedCanvasGroup, true);
            ToggleCanvasGroup(_unlockedCanvasGroup, false);
            ToggleCanvasGroup(_builtCanvasGroup, false);
        }
        else if (_room.IsBuilt)
        {
            ToggleCanvasGroup(_lockedCanvasGroup, false);
            ToggleCanvasGroup(_unlockedCanvasGroup, false);
            ToggleCanvasGroup(_builtCanvasGroup, true);
        }
        else
        {
            ToggleCanvasGroup(_lockedCanvasGroup, false);
            ToggleCanvasGroup(_unlockedCanvasGroup, true);
            ToggleCanvasGroup(_builtCanvasGroup, false);
        }
    }

    private void UpdateResourceImages()
    {
        var underlyingElementType = _room.UnderlyingElement;
        foreach (ResourceElement element in _resourceElements)
        {
            element.UpdateResourceType(underlyingElementType);
        }
    }

    private void UpdateRoomImage(RoomType roomType)
    {
        _roomImage.sprite = _uiConfig.GetRoomIcon(roomType);
        var activeFrames = _uiConfig.GetRoomActiveFrames(roomType);
        uiSpriteSheetAnimator.SetFrames(activeFrames);
        _roomNameText.text = _uiConfig.GetRoomName(roomType);
    }

    private void OnRoomBuilt(Room room)
    {
        UpdateRoomImage(room.Type);
        UpdateState();
    }
}
