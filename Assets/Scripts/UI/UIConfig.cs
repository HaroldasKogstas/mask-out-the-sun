using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIConfig", menuName = "Scriptable Objects/UIConfig")]
public class UIConfig : ScriptableObject
{
    [Serializable]
    public class RoomTypeUISet
    {
        [SerializeField] private RoomType _roomType;
        [SerializeField] private Sprite _roomSpriteDefault;
        [SerializeField] private Sprite _roomIcon;
        [SerializeField] private List<Sprite> _roomActiveFrames = new();
        
        public RoomType RoomType => _roomType;
        public Sprite RoomSpriteDefault => _roomSpriteDefault;
        public Sprite RoomIcon => _roomIcon;
        public List<Sprite> RoomActiveFrames => _roomActiveFrames;
    }

    [Serializable]
    public class ResourceTypeUISet
    {
        [SerializeField] private ResourceType _resourceType;
        [SerializeField] private Sprite _resourceIcon;
        [SerializeField] private Color _resourceColor;
        
        public ResourceType ResourceType => _resourceType;
        public Sprite ResourceIcon => _resourceIcon;
        public Color ResourceColor => _resourceColor;
    }
    
    [SerializeField] private List<RoomTypeUISet> _roomTypeUISets = new();
    [SerializeField] private List<ResourceTypeUISet> _resourceTypeUISets = new();
    
    public Sprite GetRoomSpriteDefault(RoomType roomType)
    {
        var set = _roomTypeUISets.Find(x => x.RoomType == roomType);
        return set != null ? set.RoomSpriteDefault : null;
    }

    public Sprite GetRoomIcon(RoomType roomType)
    {
        var set = _roomTypeUISets.Find(x => x.RoomType == roomType);
        return set != null ? set.RoomIcon : null;
    }
    
    public List<Sprite> GetRoomActiveFrames(RoomType roomType)
    {
        var set = _roomTypeUISets.Find(x => x.RoomType == roomType);
        return set != null ? set.RoomActiveFrames : new List<Sprite>();
    }
    
    public Sprite GetResourceIcon(ResourceType resourceType)
    {
        var set = _resourceTypeUISets.Find(x => x.ResourceType == resourceType);
        return set != null ? set.ResourceIcon : null;
    }
    
    public Color GetResourceColor(ResourceType resourceType)
    {
        var set = _resourceTypeUISets.Find(x => x.ResourceType == resourceType);
        return set != null ? set.ResourceColor : Color.white;
    }
}
