using CheekyStork.ScriptableVariables;
using UnityEngine;
using UnityEngine.VFX;

public class Sun : MonoBehaviour
{
    [SerializeField]
    FloatSO _progress;

    [SerializeField]
    Gradient _lightGradient;

    [SerializeField]
    VisualEffect _effect;

    [SerializeField]
    Light _pointLight;

    void Update()
    {
        float progress = Mathf.Clamp01(_progress.Value);
        Color color = _lightGradient.Evaluate(progress);

        _pointLight.color = color;
        _effect.SetFloat("Turbulence", 1-progress);
    }
}
