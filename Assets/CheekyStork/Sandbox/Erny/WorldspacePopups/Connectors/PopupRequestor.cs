using Sirenix.OdinInspector;
using UnityEngine;

namespace CheekyStork
{
    public abstract class PopupRequestor : MonoBehaviour
    {
        [Title("Broadcasting On")]
        [SerializeField]
        private WorldspacePopupDataEventChannelSO _popupRequestChannel;

        protected void RequestPopup(WorldspacePopupData popupData)
        {
            _popupRequestChannel.RaiseEvent(popupData);
        }
    }
}