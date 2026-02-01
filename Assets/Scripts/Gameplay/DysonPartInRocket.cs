using System;
using System.Collections;
using CheekyStork.ScriptableChannels;
using CheekyStork.ScriptableVariables;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class DysonPartInRocket : MonoBehaviour
{
    [SerializeField]
    private TransformSO _nextDysonPartTransformSO;

    [SerializeField]
    private VoidEventChannelSO _dysonPartJourneyStartChannelSO;

    [SerializeField]
    private VoidEventChannelSO _onRocketArrivedChannelSO;

    [SerializeField]
    private float _travelTime;

    [Range(0,1)]
    [SerializeField]
    private float _arc;

    [SerializeField]
    private float _destroyDelay = 2;

    [SerializeField]
    private Vector3 _obstacleCenter = Vector3.zero;

    [SerializeField]
    private float _obstacleRadius = 1;

    [SerializeField]
    private TrailRenderer _trail;

    private bool _isActive;
    private Vector3 _startPosition;
    private float _elapsedTime;

    private Transform _target;

    private Coroutine _coroutine;

    public void Initialize()
    {
        _isActive = true;
        _startPosition = transform.position;
        _elapsedTime = 0f;

        _target = _nextDysonPartTransformSO.Value;

        _dysonPartJourneyStartChannelSO.RaiseEvent();
    }

    void Update()
    {
        if(!_isActive)
            return;

        _elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / _travelTime);

        Vector3 targetPosition = _target.position;
        
        Vector3 linearPosition = Vector3.Lerp(_startPosition, targetPosition, t);
        
        Vector3 travelDirection = (targetPosition - _startPosition).normalized;
        
        Vector3 midPoint = (_startPosition + targetPosition) / 2f;
        Vector3 toObstacle = _obstacleCenter - midPoint;
        
        Vector3 perpendicular = Vector3.Cross(travelDirection, toObstacle.normalized);
        if (perpendicular.magnitude < 0.01f)
        {
            perpendicular = Vector3.Cross(travelDirection, Vector3.up);
            if (perpendicular.magnitude < 0.01f)
            {
                perpendicular = Vector3.Cross(travelDirection, Vector3.forward);
            }
        }
        perpendicular = perpendicular.normalized;
        
        float distanceToObstacle = Vector3.Distance(linearPosition, _obstacleCenter);
        float requiredClearance = _obstacleRadius + 0.5f; // Add safety margin
        
        float baseArcHeight = _arc * Vector3.Distance(_startPosition, targetPosition) * 0.5f;
        float clearanceArcHeight = Mathf.Max(0, requiredClearance - distanceToObstacle);
        float arcHeight = Mathf.Max(baseArcHeight, clearanceArcHeight) * Mathf.Sin(t * Mathf.PI);
        
        Vector3 arcOffset = perpendicular * arcHeight;
        transform.position = linearPosition + arcOffset;

        // Check if arrived
        if (t >= 1f)
        {
            OnArrived();
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(_coroutine);
    }

    private void OnArrived()
    {
        _onRocketArrivedChannelSO.RaiseEvent();

        _isActive = false;

        _coroutine = StartCoroutine(DestroyCoroutine());
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(_destroyDelay);

        Destroy(gameObject);
    }
}
