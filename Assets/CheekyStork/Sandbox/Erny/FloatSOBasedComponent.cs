using CheekyStork.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class FloatSOBasedComponent : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField]
    protected FloatSO FloatToRespondTo;

    [SerializeField]
    protected bool InvertResponse = false;

    [SerializeField]
    protected float MinValue = 0f;

    [SerializeField]
    protected float MaxValue = 1f;

    private void Awake()
    {
        SubscribeToEvents();

        OnFloatValueChanged();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        FloatToRespondTo.OnValueChanged += OnFloatValueChanged;
    }

    private void UnsubscribeFromEvents()
    {
        FloatToRespondTo.OnValueChanged -= OnFloatValueChanged;
    }

    protected abstract void OnFloatValueChanged();
}