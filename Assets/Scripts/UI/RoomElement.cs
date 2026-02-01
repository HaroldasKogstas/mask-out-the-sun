using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class RoomElement : MonoBehaviour
{
    public enum RoomElementState
    {
        locked,
        unlocked,
        built
    }

    [SerializeField]
    private UiSpriteSheetAnimator uiSpriteSheetAnimator;

    [SerializeField]
    private Room _room;

    [SerializeField]
    private Sprite _upgradeEmptyBubbleSprite;

    [SerializeField]
    private Sprite _upgradeFilledBubbleSprite;

    [SerializeField]
    private Image _roomImage;

    [SerializeField]
    private Image _progressImage;

    [SerializeField]
    private List<ResourceElement> _resourceElements;

    [SerializeField]
    private List<Image> _upgradeBubbles;

    [SerializeField]
    private UIConfig _uiConfig;

    [SerializeField]
    private CanvasGroup _lockedCanvasGroup;

    [SerializeField]
    private CanvasGroup _unlockedCanvasGroup;

    [SerializeField]
    private CanvasGroup _builtCanvasGroup;

    [SerializeField]
    private TextMeshProUGUI _roomNameText;

    [SerializeField]
    private Button _upgradeButton;

    [SerializeField]
    private Button _destroyButton;

    private void Awake()
    {
        _room.Built += OnRoomBuilt;
        _room.Unlocked += OnRoomUnlocked;
        _room.Upgraded += OnRoomUpgraded;
        _room.Destroyed += OnRoomDestroyed;

        _upgradeButton.onClick.AddListener(TryUpgradeRoom);
        _destroyButton.onClick.AddListener(DestroyRoom);
    }

    private void Start()
    {
        UpdateResourceImages();
        UpdateState();
        RefreshUpgradeBubbles();
    }

    private void OnDestroy()
    {
        _room.Built -= OnRoomBuilt;
        _room.Unlocked -= OnRoomUnlocked;
        _room.Upgraded -= OnRoomUpgraded;
        _room.Destroyed -= OnRoomDestroyed;

        _upgradeButton.onClick.RemoveListener(TryUpgradeRoom);
        _destroyButton.onClick.RemoveListener(DestroyRoom);
    }

    private void Update()
    {
        if (_room.IsBuilt)
        {
            float progress = _room.Progress01;
            _progressImage.fillAmount = progress;
        }
        else
        {
            _progressImage.fillAmount = 0f;
        }
    }

    private void TryUpgradeRoom()
    {
        _room.TryUpgrade();
    }

    private void DestroyRoom()
    {
        bool destroyed = _room.TryDestroy();
        if (!destroyed)
        {
            return;
        }
    }

    private void OnRoomUpgraded(Room room, int level)
    {
        RefreshUpgradeBubbles();
    }

    private void OnRoomUnlocked(Room obj)
    {
        UpdateState();
    }

    private void OnRoomBuilt(Room room)
    {
        UpdateRoomImage(room.Type);
        UpdateState();
        RefreshUpgradeBubbles();
    }

    private void OnRoomDestroyed(Room room)
    {
        UpdateState();
        RefreshUpgradeBubbles();

        _roomImage.sprite = null;
        uiSpriteSheetAnimator.SetFrames(null);
        _roomNameText.text = string.Empty;

        _progressImage.fillAmount = 0f;
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
        ResourceType underlyingElementType = _room.UnderlyingElement;
        for (int i = 0; i < _resourceElements.Count; i++)
        {
            _resourceElements[i].UpdateResourceType(underlyingElementType);
        }
    }

    private void UpdateRoomImage(RoomType roomType)
    {
        _roomImage.sprite = _uiConfig.GetRoomIcon(roomType);
        List<Sprite> activeFrames = _uiConfig.GetRoomActiveFrames(roomType);
        uiSpriteSheetAnimator.SetFrames(activeFrames);
        _roomNameText.text = _uiConfig.GetRoomName(roomType);
    }

    private void RefreshUpgradeBubbles()
    {
        int level = _room.IsBuilt ? _room.TierIndex : 0;

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
}
