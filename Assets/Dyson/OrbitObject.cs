using UnityEngine;

public class OrbitObject : MonoBehaviour
{
    float _speed;
    Vector3 _orbitAxis;
    Transform _orbitCenter;

    public void Initialize(float speed, Vector3 orbitAxis, Transform orbitCenter = null)
    {
        _speed = speed;
        _orbitAxis = orbitAxis.normalized;
        _orbitCenter = orbitCenter;
    }

    void Update()
    {
        if (_orbitCenter == null)
            return;

        transform.RotateAround(_orbitCenter.position, _orbitAxis, _speed * Time.deltaTime);
    }
}
