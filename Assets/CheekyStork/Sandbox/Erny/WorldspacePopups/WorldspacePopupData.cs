using UnityEngine;

namespace CheekyStork
{
    public class WorldspacePopupData
    {
        public Transform Transform;
        public string Text;
        public float Duration;

        public WorldspacePopupData(Transform transform, string text, float duration = 5f)
        {
            Transform = transform;
            Text = text;
            Duration = duration;
        }
    }
}