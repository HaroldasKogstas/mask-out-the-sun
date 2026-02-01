using CheekyStork.ScriptableChannels;
using UnityEngine;

namespace CheekyStork
{
    [CreateAssetMenu(fileName = "SO_NewWorldspacePopupDataEventChannel", menuName = "Scriptable Objects/Channels/Worldspace Popup Data Event Channel")]
    public class WorldspacePopupDataEventChannelSO : ChannelSO<WorldspacePopupData>
    {		
    }

    [CreateAssetMenu(fileName = "SO_NewWorldspaceMultiPopupDataEventChannel", menuName = "Scriptable Objects/Channels/Worldspace Multi Popup Data Event Channel")]
    public class WorldspaceMultiPopupDataEventChannelSO : ChannelSO<WorldspaceMultiPopupData>
    {
    }
}