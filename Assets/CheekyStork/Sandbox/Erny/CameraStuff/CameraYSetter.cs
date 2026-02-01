using UnityEngine;

public class CameraYSetter : FloatSOBasedComponent
{
    [SerializeField]
    private Transform _cameraToMove;

    protected override void OnFloatValueChanged()
    {
        // set the camera's y position to the FloatSO value relative to minvalue and maxvalue
        Vector3 position = _cameraToMove.position;
        position.y = Mathf.Lerp(MinValue, MaxValue, FloatToRespondTo.Value);
        _cameraToMove.position = position;
    }
}