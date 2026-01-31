using System.Collections;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    private float _angleMinRange = -10f;

    [SerializeField]
    private float _angleMaxRange = 10f;

    [SerializeField]
    private float _heightTargetForZRotation = 100;

    [SerializeField]
    private float _acceleration = 5f;

    [SerializeField]
    private float _destroyTime;
    
    private bool _isActive;

    private float _speed;

    public void Initialize(float speed)
    {
        float zAngle = Mathf.Atan2(_heightTargetForZRotation - transform.position.y, -transform.position.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Euler(Random.Range(_angleMinRange, _angleMaxRange), 0, zAngle);

        _speed = speed;

        StartCoroutine(DestroyCoroutine());
        _isActive = true;
    }

    void Update()
    {
        if(!_isActive)        
            return;

        _speed += _acceleration * Time.deltaTime;
        
        transform.Translate(_speed * Time.deltaTime * Vector3.up);
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(_destroyTime);

        Destroy(gameObject);
    }
}
