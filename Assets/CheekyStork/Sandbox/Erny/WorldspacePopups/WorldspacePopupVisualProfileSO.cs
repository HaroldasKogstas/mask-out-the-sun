using UnityEngine;

namespace CheekyStork
{
    [CreateAssetMenu(fileName = "SO_NewWorldspacePopupVisualProfile", menuName = "Scriptable Objects/Worldspace Popup Visual Profile")]
    public class WorldspacePopupVisualProfileSO : ScriptableObject
    {
        public Color TextColor;
        public float FontSizeMultiplier = 1f;
    }
}