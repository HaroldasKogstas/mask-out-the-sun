using System.Collections.Generic;
using UnityEngine;

namespace CheekyStork
{
    public class WorldspacePopupData
    {
        public Vector3 Position ;
        public Sprite Icon;
        public string Text;
        public Color IconColor;
        public float Duration;

        public WorldspacePopupData(Vector3 position, Sprite icon, string text, Color iconColor, float duration = 5f)
        {
            Position = position;
            Icon = icon;
            Text = text;
            IconColor = iconColor;
            Duration = duration;
        }
    }

    public class WorldspaceMultiPopupData
    {
        public Vector3 Position;
        public List<WorldspacePopupData> Popups = new List<WorldspacePopupData>();

        public WorldspaceMultiPopupData(Vector3 position, List<WorldspacePopupData> popups)
        {
            Position = position;
            Popups = popups;
        }
    }
}