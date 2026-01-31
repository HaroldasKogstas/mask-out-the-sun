using CheekyStork.ScriptableVariables;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class FloatControlledAudioSource : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _audioClip;

    [SerializeField]
    private float _minValue = 0f;

    [SerializeField]
    private float _maxValue = 1f;

    [SerializeField]
    private FloatSO _floatToRespondTo;

    [SerializeField]
    private bool _invertResponse = false;

    private bool _startupComplete = false;

    private void Awake()
    {
        _audioSource.volume = 0f;
        _audioSource.clip = _audioClip;
        _audioSource.Play();

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
        float targetVolume = _invertResponse ? Mathf.Lerp(_maxValue, _minValue, normalizedValue) : Mathf.Lerp(_minValue, _maxValue, normalizedValue);

        UpdateAudioSourceVolume(_audioSource, targetVolume);
    }

    private void UpdateAudioSourceVolume(AudioSource audioSource, float targetVolume)
    {
        audioSource.volume = targetVolume;
    }

    private IEnumerator StartupCoroutine()
    {
        float targetVolume = _invertResponse ? _minValue : _maxValue;
        float elapsedTime = 0f;
        float transitionLength = 0.5f;

        while (elapsedTime < transitionLength)
        {
            elapsedTime += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(0f, targetVolume, elapsedTime / transitionLength);
            yield return null;
        }

        _audioSource.volume = targetVolume;
        _startupComplete = true;
    }
}