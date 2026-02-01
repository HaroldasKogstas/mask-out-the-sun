using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

public class ScrollLevelIndicator : FloatSOBasedComponent
{
    [SerializeField]
    private RectTransform _indicatorTransform;

    private float _targetY;

    private float _animationDuration = 0.1f;

    private TweenerCore<Vector2, Vector2, DG.Tweening.Plugins.Options.VectorOptions> _currentTween;

    protected override void OnFloatValueChanged()
    {
        float t = Mathf.InverseLerp(0f, 1f, FloatToRespondTo.Value);
        float newY = Mathf.Lerp(MinValue, MaxValue, t);

        //AnimateScrollIndicator(newY);

        _indicatorTransform.anchoredPosition = new Vector2(_indicatorTransform.anchoredPosition.x, newY);
    }

    private void AnimateScrollIndicator(float targetY)
    {
        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
        }

        _currentTween = _indicatorTransform.DOAnchorPosY(targetY, _animationDuration).SetEase(Ease.Linear);
    }
}