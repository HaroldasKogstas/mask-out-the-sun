using Sirenix.OdinInspector;
using UnityEngine;

namespace CheekyStork
{
    public class WorldspacePopupManager : MonoBehaviour
    {
        [Title("Settings")]
        [SerializeField]
        private WorldspacePopup _worldspacePopupPrefab;

        [Title("Listening To")]
        [SerializeField]
        private WorldspacePopupDataEventChannelSO _popupRequestEventChannel;

        private void Awake()
        {
            _popupRequestEventChannel.OnEventRaised += SpawnPopup;
        }

        private void OnDestroy()
        {
            _popupRequestEventChannel.OnEventRaised -= SpawnPopup;
        }

        private void SpawnPopup(WorldspacePopupData popupData)
        {
            float randomOffsetX = Random.Range(-6f, 6f);

            // the random offset should be applied to the x axis of the screen space, not world space

            //Vector3 screenPosition = Camera.main.WorldToScreenPoint(popupData.Transform.position);
            //screenPosition.x += randomOffsetX;
            //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            Vector3 worldPosition = popupData.Transform.position + new Vector3(randomOffsetX * 0.01f, 0f, 0f);

            WorldspacePopup popupInstance = Instantiate(_worldspacePopupPrefab, worldPosition, Quaternion.identity);

            popupInstance.Initialize(popupData);
        }
    }
}