using CheekyStork.ScriptableVariables;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class FloatControlledAudioFilter : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField]
    private AudioLowPassFilter _audioFilter;

    [SerializeField]
    private float _minValue = 500f;

    [SerializeField]
    private float _maxValue = 5000f;

    [SerializeField]
    private FloatSO _floatToRespondTo;

    [SerializeField]
    private bool _invertResponse = false;

    private bool _startupComplete = false;

    private void Awake()
    {
        StartCoroutine(StartupCoroutine());

        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        _floatToRespondTo.OnValueChanged += OnFloatValueChanged;
    }

    private void UnsubscribeFromEvents()
    {
        _floatToRespondTo.OnValueChanged -= OnFloatValueChanged;
    }

    private void OnFloatValueChanged()
    {
        if (!_startupComplete)
            return;

        // set volume proportional to float value depending on invert response
        float normalizedValue = Mathf.Clamp01(_floatToRespondTo.Value);
        float targetFrequency = _invertResponse ? Mathf.Lerp(_maxValue, _minValue, normalizedValue) : Mathf.Lerp(_minValue, _maxValue, normalizedValue);

        UpdateAudioSourceVolume(_audioFilter, targetFrequency);
    }

    private void UpdateAudioSourceVolume(AudioLowPassFilter filter, float targetFrequency)
    {
        filter.cutoffFrequency = targetFrequency;
    }

    private IEnumerator StartupCoroutine()
    {
        float targetFrequency = _invertResponse ? _minValue : _maxValue;
        float elapsedTime = 0f;
        float transitionLength = 0.5f;

        while (elapsedTime < transitionLength)
        {
            elapsedTime += Time.deltaTime;
            _audioFilter.cutoffFrequency = Mathf.Lerp(0f, targetFrequency, elapsedTime / transitionLength);
            yield return null;
        }

        _audioFilter.cutoffFrequency = targetFrequency;
        _startupComplete = true;
    }
}