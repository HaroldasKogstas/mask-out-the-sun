using CheekyStork.ScriptableVariables;
using UnityEngine;
using UnityEngine.UI;

public class SliderSetter : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private FloatSO _distanceToSun;

    private float _minValue = 0f;
    private float _maxValue = 1f;

    private Coroutine _groundLevelChangeCoroutine;

    private void Awake()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        _distanceToSun.OnValueChanged += OnDistanceToSunChanged;
    }

    private void UnsubscribeFromEvents()
    {
        _distanceToSun.OnValueChanged -= OnDistanceToSunChanged;
    }

    private void OnDistanceToSunChanged()
    {
        float normalizedDistance = Mathf.Clamp01(_distanceToSun.Value);
        float targetValue = Mathf.Lerp(_minValue, _maxValue, normalizedDistance);
        _slider.value = targetValue;
    }
}