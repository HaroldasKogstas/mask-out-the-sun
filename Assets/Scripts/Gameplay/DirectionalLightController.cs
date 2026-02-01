using CheekyStork.ScriptableVariables;
using UnityEngine;

public class DirectionalLightController : MonoBehaviour
{
    [SerializeField]
    private Light _directionalLight;

    [SerializeField]
    private FloatSO _progress;

    [Range(0, 1)]
    [SerializeField]
    private float _minIntensityMultiplier;

    private float _defaultIntensity;

    void Awake()
    {
        _defaultIntensity = _directionalLight.intensity;     
    }

    void Update()
    {
        float intensityMultiplier = _minIntensityMultiplier + (1 - _minIntensityMultiplier) * (1-_progress.Value);
        _directionalLight.intensity = _defaultIntensity*intensityMultiplier;
    }


}
