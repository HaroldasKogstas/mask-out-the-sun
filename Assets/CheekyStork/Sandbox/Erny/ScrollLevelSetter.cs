using CheekyStork.ScriptableVariables;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

public class ScrollLevelSetter : MonoBehaviour
{
    [SerializeField]
    private FloatSO _currentLevelOfScroll;

    private float _targetScrollLevel;

    private float _smoothTime = 0.3f;

    private float _step = 0.06f;

    private TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> _currentTween;

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            _targetScrollLevel += Input.mouseScrollDelta.y * _step;
            _targetScrollLevel = Mathf.Clamp01(_targetScrollLevel);

            SmoothScrollValueTween(_targetScrollLevel);
        }
    }

    private void SmoothScrollValueTween(float targetScrollLevel)
    {
        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
        }

        _currentTween = DOTween.To(() => _currentLevelOfScroll.Value, x => _currentLevelOfScroll.Value = x, targetScrollLevel, _smoothTime)
            .SetEase(Ease.Linear);
    }
}