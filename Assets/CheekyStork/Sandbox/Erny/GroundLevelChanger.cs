using CheekyStork.ScriptableVariables;
using UnityEngine;

public class GroundLevelChanger : MonoBehaviour
{
    [SerializeField]
    private BoolSO _isBelowGround;

    [SerializeField]
    private FloatSO _currentLevelOfScroll;

    [SerializeField]
    [Range(0f, 1f)]
    private float _groundLevelThreshold = 0.95f;

    private void Awake()
    {
        _currentLevelOfScroll.OnValueChanged += OnValueChanged;
    }

    private void OnDestroy()
    {
        _currentLevelOfScroll.OnValueChanged -= OnValueChanged;
    }

    private void OnValueChanged()
    {
        CheckIfBelowGround();
    }

    private void CheckIfBelowGround()
    {
        if (_currentLevelOfScroll.Value >= _groundLevelThreshold)
        {
            _isBelowGround.Value = true;
        }
        else
        {
            _isBelowGround.Value = false;
        }
    }
}