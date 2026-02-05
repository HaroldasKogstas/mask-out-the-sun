using CheekyStork;
using CheekyStork.Pooling;
using UnityEngine;
using UnityEngine.Events;

public class WorldSpaceMultiPopup : MonoBehaviour, IPoolable<WorldSpaceMultiPopup>
{
    private float _timeUntilReturnToPool;

    public event UnityAction<WorldSpaceMultiPopup> ReturnToPool;

    private void Update()
    {
        _timeUntilReturnToPool -= Time.deltaTime;

        if (_timeUntilReturnToPool <= 0f)
        {
            ReturnToPool?.Invoke(this);
        }
    }

    public void Initialize(WorldspacePopupData popupData)
    {
        _timeUntilReturnToPool = popupData.Duration;
    }

    public void ResetObject()
    {
        _timeUntilReturnToPool = 1f;

        transform.DetachChildren();
    }
}