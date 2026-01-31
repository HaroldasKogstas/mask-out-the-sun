using UnityEngine;
using Unity.Cinemachine;
using CheekyStork.ScriptableVariables;

// this component smoothly transitions between 2 cinemachine cameras referenced in the inspector
public class CameraTransitioner : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera cameraA;

    [SerializeField]
    private CinemachineCamera cameraB;

    [SerializeField]
    private BoolSO _controllingBool;

    private void Awake()
    {
        _controllingBool.OnValueChanged += OnControllingValueChanged;

        UpdateCameraPriority();
    }

    private void OnDestroy()
    {
        _controllingBool.OnValueChanged -= OnControllingValueChanged;
    }

    private void OnControllingValueChanged()
    {
        UpdateCameraPriority();
    }

    private void UpdateCameraPriority()
    {
        if (_controllingBool.Value)
        {
            cameraA.Priority = 10;
            cameraB.Priority = 5;
        }
        else
        {
            cameraA.Priority = 5;
            cameraB.Priority = 10;
        }
    }
}