using CheekyStork.Pooling;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CheekyStork
{
    public class WorldspacePopupManager : MonoBehaviour
    {
        [Title("Settings")]
        [SerializeField]
        private WorldspacePopup _worldspacePopupPrefab;

        [SerializeField]
        private WorldSpaceMultiPopup _worldspaceMultiPopupPrefab;

        [Title("Listening To")]
        [SerializeField]
        private WorldspacePopupDataEventChannelSO _popupRequestEventChannel;

        [SerializeField]
        private WorldspaceMultiPopupDataEventChannelSO _multiPopupRequestEventChannel;

        private ObjectPoolHandler<WorldspacePopup> _worldspacePopupPool;
        private ObjectPoolHandler<WorldSpaceMultiPopup> _worldspaceMultiPopupPool;

        private void Awake()
        {
            CreatePopupPool();

            _popupRequestEventChannel.OnEventRaised += SpawnPopup;
            _multiPopupRequestEventChannel.OnEventRaised += SpawnMultiPopup;
        }

        private void OnDestroy()
        {
            _popupRequestEventChannel.OnEventRaised -= SpawnPopup;
            _multiPopupRequestEventChannel.OnEventRaised -= SpawnMultiPopup;
        }

        private void CreatePopupPool()
        {
            _worldspacePopupPool = new ObjectPoolHandler<WorldspacePopup>(
                prefabToPool: _worldspacePopupPrefab,
                parentTransform: transform,
                poolCapacity: 50,
                preInstantiatePool: true);

            _worldspaceMultiPopupPool = new ObjectPoolHandler<WorldSpaceMultiPopup>(
                prefabToPool: _worldspaceMultiPopupPrefab,
                parentTransform: transform,
                poolCapacity: 50,
                preInstantiatePool: true);
        }

        private void SpawnPopup(WorldspacePopupData popupData)
        {
            float randomOffsetX = Random.Range(-2f, 2f);

            // the random offset should be applied to the x axis of the screen space, not world space

            //Vector3 screenPosition = Camera.main.WorldToScreenPoint(popupData.Transform.position);
            //screenPosition.x += randomOffsetX;
            //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            Vector3 worldPosition = popupData.Position + new Vector3(randomOffsetX * 0.01f, 0f, 0f);

            WorldspacePopup popupInstance = _worldspacePopupPool.Get();

            popupInstance.transform.SetPositionAndRotation(worldPosition, Quaternion.identity);
            popupInstance.Initialize(popupData);
        }

        private void SpawnMultiPopup(WorldspaceMultiPopupData multiPopupData)
        {
            float randomOffsetX = Random.Range(-2f, 2f);

            Vector3 worldPosition = multiPopupData.Position + new Vector3(randomOffsetX * 0.01f, 0f, 0f);

            WorldSpaceMultiPopup multiPopupInstance = _worldspaceMultiPopupPool.Get();

            multiPopupInstance.transform.SetPositionAndRotation(worldPosition, Quaternion.identity);
            multiPopupInstance.Initialize(multiPopupData.Popups[0]);

            for (int i = 0; i < multiPopupData.Popups.Count; i++)
            {
                WorldspacePopup popupInstance = _worldspacePopupPool.Get();

                popupInstance.transform.SetPositionAndRotation(worldPosition, Quaternion.identity);
                popupInstance.transform.SetParent(multiPopupInstance.transform);
                popupInstance.Initialize(multiPopupData.Popups[i]);
            }
        }
    }
}