using UnityEngine;

public class FloatBasedYPosition : FloatSOBasedComponent
{
    [SerializeField]
    private RectTransform _rectTransform;

    protected override void OnFloatValueChanged()
    {
        // Lerp between startingY and targetY based on the float value (assumed to be between 0 and 1)
        float newY = Mathf.Lerp(MinValue, MaxValue, FloatToRespondTo.Value);
        Vector2 anchoredPosition = _rectTransform.anchoredPosition;
        anchoredPosition.y = newY;
        _rectTransform.anchoredPosition = anchoredPosition;
    }
}