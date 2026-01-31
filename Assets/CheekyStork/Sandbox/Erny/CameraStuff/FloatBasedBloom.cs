using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class FloatBasedBloom : FloatSOBasedComponent
{
    [Title("Settings")]
    [SerializeField]
    private Volume _volume;

    protected override void OnFloatValueChanged()
    {
        float targetValue = InvertResponse ?
            Mathf.Lerp(MaxValue, MinValue, FloatToRespondTo.Value) :
            Mathf.Lerp(MinValue, MaxValue, FloatToRespondTo.Value);

        if (_volume.profile.TryGet(out Bloom bloom))
        {
            bloom.intensity.value = targetValue;
        }
    }
}