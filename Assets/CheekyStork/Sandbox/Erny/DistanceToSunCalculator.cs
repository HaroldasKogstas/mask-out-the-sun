using CheekyStork.ScriptableVariables;
using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using UnityEngine;

public class DistanceToSunCalculator : MonoBehaviour
{
    [SerializeField]
    private FloatSO _distanceToSun;

    [SerializeField]
    private float _transitionLength = 1f;

    [SerializeField]
    private Ease _easeType = Ease.Linear;

    [SerializeField]
    private BoolSO _isBelowGround;

    private float _groundLevel = 0f;
    private float _sunLevel = 1f;

    private TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> _currentTween;

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
        _isBelowGround.OnValueChanged += OnGroundLevelChanged;
    }

    private void UnsubscribeFromEvents()
    {
        _isBelowGround.OnValueChanged -= OnGroundLevelChanged;
    }

    private void OnGroundLevelChanged()
    {
        // kill any existing tween
        _currentTween?.Kill();

        float targetDistance = _isBelowGround.Value ? _groundLevel : _sunLevel;
        _currentTween = DOTween.To(() => _distanceToSun.Value, x => _distanceToSun.Value = x, targetDistance, _transitionLength).SetEase(_easeType);


    }
}