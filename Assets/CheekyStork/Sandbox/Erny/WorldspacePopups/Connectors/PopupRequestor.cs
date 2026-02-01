using Sirenix.OdinInspector;
using UnityEngine;

namespace CheekyStork
{
    public abstract class PopupRequestor : MonoBehaviour
    {
        [Title("Broadcasting On")]
        [SerializeField]
        private WorldspacePopupDataEventChannelSO _popupRequestChannel;

        [SerializeField]
        private WorldspaceMultiPopupDataEventChannelSO _multiPopupRequestChannel;

        protected void RequestPopup(WorldspacePopupData popupData)
        {
            _popupRequestChannel.RaiseEvent(popupData);
        }

        protected void RequestMultiPopup(WorldspaceMultiPopupData multiPopupData)
        {
            _multiPopupRequestChannel.RaiseEvent(multiPopupData);
        }
    }
}