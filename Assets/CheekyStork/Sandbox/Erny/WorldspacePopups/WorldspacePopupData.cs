using UnityEngine;

namespace CheekyStork
{
    public class WorldspacePopupData
    {
        public Transform Transform;
        public Sprite Icon;
        public string Text;
        public Color IconColor;
        public float Duration;

        public WorldspacePopupData(Transform transform, Sprite icon, string text, Color iconColor, float duration = 5f)
        {
            Transform = transform;
            Icon = icon;
            Text = text;
            IconColor = iconColor;
            Duration = duration;
        }
    }
}