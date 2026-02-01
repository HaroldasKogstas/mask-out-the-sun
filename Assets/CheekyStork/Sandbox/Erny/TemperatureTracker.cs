using CheekyStork.ScriptableVariables;
using TMPro;
using UnityEngine;

public class TemperatureTracker : MonoBehaviour
{
    [SerializeField]
    private IntSO _dysonPartsDeployed;

    [SerializeField]
    private int _minTemperature;

    [SerializeField] 
    private int _maxTemperature;

    [SerializeField]
    private TextMeshProUGUI _textDisplay;

    private void Awake()
    {
        _dysonPartsDeployed.OnValueChanged += OnValueChanged;

        OnValueChanged();
    }

    private void OnDestroy()
    {
        _dysonPartsDeployed.OnValueChanged -= OnValueChanged;
    }

    private void OnValueChanged()
    {
        // assuming dyson parts deployed ranges from 0 to 1000, make temperature inversely proportional to parts deployed
        int partsDeployed = _dysonPartsDeployed.Value;
        float t = Mathf.InverseLerp(1000, 0, partsDeployed);
        int temperature = Mathf.RoundToInt(Mathf.Lerp(_minTemperature, _maxTemperature, t));
        _textDisplay.text = temperature.ToString();
    }
}