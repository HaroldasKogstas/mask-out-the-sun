using CheekyStork.ScriptableVariables;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Title("Music")]
    [SerializeField]
    private AudioSource _musicAudioSource;

    [SerializeField]
    private AudioClip _musicClip;

    [Title("References")]
    [SerializeField]
    private FloatSO _distanceToSun;

    private float _minVolume = 0.2f;
    private float _maxVolume = 0.8f;

    private Coroutine _groundLevelChangeCoroutine;

    private void Awake()
    {
        // play music on awake
        _musicAudioSource.clip = _musicClip;
        _musicAudioSource.Play();

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
        // set volume proportional to distance to sun
        float normalizedDistance = Mathf.Clamp01(_distanceToSun.Value);
        float targetVolume = Mathf.Lerp(_maxVolume, _minVolume, normalizedDistance);
    }

    private void UpdateAudioSourceVolume(AudioSource audioSource, float targetVolume)
    {
        audioSource.volume = targetVolume;
    }
}