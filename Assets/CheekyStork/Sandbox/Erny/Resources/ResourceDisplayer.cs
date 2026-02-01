using CheekyStork.ScriptableVariables;
using TMPro;
using UnityEngine;

public class ResourceDisplayer : MonoBehaviour
{
    [SerializeField]
    private IntSO _valueToDisplay;

    [SerializeField]
    private TextMeshProUGUI _textDisplay;

    private void Awake()
    {
        _valueToDisplay.OnValueChanged += OnValueChanged;

        OnValueChanged();
    }

    private void OnDestroy()
    {
        _valueToDisplay.OnValueChanged -= OnValueChanged;
    }

    private void OnValueChanged()
    {
        _textDisplay.text = FormatValue(_valueToDisplay.Value);
    }

    private string FormatValue(int value)
    {
        string suffix = "";

        if (value < 0) {
            suffix = "-";
            value = -value;
        }
        if (value >= 1_000_000_000)
        {
            value /= 1_000_000_000;
            suffix = "B";
        }
        else if (value >= 1_000_000)
        {
            value /= 1_000_000;
            suffix = "M";
        }
        else if (value >= 10_000)
        {
            value /= 1_000;
            suffix = "K";
        }

        return value.ToString() + suffix;
    }
}