using Sirenix.OdinInspector;
using UnityEngine;

public class FloatBasedCanvasAlpha : FloatSOBasedComponent
{
    [Title("Settings")]
    [SerializeField]
    private CanvasGroup canvasGroup;

    protected override void OnFloatValueChanged()
    {
        float targetValue = InvertResponse ?
            Mathf.Lerp(MaxValue, MinValue, FloatToRespondTo.Value) :
            Mathf.Lerp(MinValue, MaxValue, FloatToRespondTo.Value);

        canvasGroup.alpha = targetValue;
    }
}