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
    [SerializeField] private List<ResourceElement> _resourceElements;
    [SerializeField] private List<Image> _upgradeBubbles;
    [SerializeField] private UIConfig _uiConfig;
    [SerializeField] private CanvasGroup _lockedCanvasGroup;
    [SerializeField] private CanvasGroup _unlockedCanvasGroup;
    [SerializeField] private CanvasGroup _builtCanvasGroup;
    [SerializeField] private TextMeshProUGUI _roomNameText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _destroyButton;

    private void Start()
    {
        _room.Built += OnRoomBuilt;
        _room.Unlocked += OnRoomUnlocked;
        _room.Upgraded += OnRoomUpgraded;
        _upgradeButton.onClick.AddListener(TryUpgradeRoom);
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

    private void UpdateState()
    {
        if (_room.IsLocked)
        {
            _lockedCanvasGroup.alpha = 1;
            _lockedCanvasGroup.blocksRaycasts = true;
            _lockedCanvasGroup.interactable = true;
            _unlockedCanvasGroup.alpha = 0;
            _unlockedCanvasGroup.blocksRaycasts = false;
            _unlockedCanvasGroup.interactable = false;
            _builtCanvasGroup.alpha = 0;
            _builtCanvasGroup.blocksRaycasts = false;
            _builtCanvasGroup.interactable = false;
        }
        else if (_room.IsBuilt)
        {
            _lockedCanvasGroup.alpha = 0;
            _lockedCanvasGroup.blocksRaycasts = false;
            _lockedCanvasGroup.interactable = false;
            _unlockedCanvasGroup.alpha = 0;
            _unlockedCanvasGroup.blocksRaycasts = false;
            _unlockedCanvasGroup.interactable = false;
            _builtCanvasGroup.alpha = 1;
            _builtCanvasGroup.blocksRaycasts = true;
            _builtCanvasGroup.interactable = true;
        }
        else
        {
            _lockedCanvasGroup.alpha = 0;
            _lockedCanvasGroup.blocksRaycasts = false;
            _lockedCanvasGroup.interactable = false;
            _unlockedCanvasGroup.alpha = 1;
            _unlockedCanvasGroup.blocksRaycasts = true;
            _unlockedCanvasGroup.interactable = true;
            _builtCanvasGroup.alpha = 0;
            _builtCanvasGroup.blocksRaycasts = false;
            _builtCanvasGroup.interactable = false;
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
