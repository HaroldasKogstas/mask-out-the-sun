using CheekyStork.ScriptableVariables;
using TMPro;
using UnityEngine;

public class ValueDisplayer : MonoBehaviour
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
        _textDisplay.text = _valueToDisplay.Value.ToString();
    }
}