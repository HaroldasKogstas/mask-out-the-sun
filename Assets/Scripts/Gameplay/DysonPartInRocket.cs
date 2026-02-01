using System;
using CheekyStork.ScriptableChannels;
using CheekyStork.ScriptableVariables;
using DG.Tweening;
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

    private bool _isActive;
    private Vector3 _startPosition;
    private float _elapsedTime;

    private Transform _target;

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
        // Move towards the target Dyson part like in orbit with a collision course with the dysonPartTranfsrom position by the end of travel time with arc where 0 is straight line and 1 is spherical
        if(!_isActive)
            return;

        _elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / _travelTime);

        Vector3 targetPosition = _target.position;
        Vector3 obstacleCenter = Vector3.zero;
        float obstacleRadius = 1f;
        
        // Linear interpolation between start and target
        Vector3 linearPosition = Vector3.Lerp(_startPosition, targetPosition, t);
        
        // Calculate the travel direction and perpendicular vector
        Vector3 travelDirection = (targetPosition - _startPosition).normalized;
        
        // Get perpendicular direction away from the obstacle
        Vector3 midPoint = (_startPosition + targetPosition) / 2f;
        Vector3 toObstacle = obstacleCenter - midPoint;
        
        // Use cross product to get perpendicular direction
        Vector3 perpendicular = Vector3.Cross(travelDirection, toObstacle.normalized);
        if (perpendicular.magnitude < 0.01f)
        {
            // Fallback if vectors are parallel
            perpendicular = Vector3.Cross(travelDirection, Vector3.up);
            if (perpendicular.magnitude < 0.01f)
            {
                perpendicular = Vector3.Cross(travelDirection, Vector3.forward);
            }
        }
        perpendicular = perpendicular.normalized;
        
        // Calculate required clearance to avoid the obstacle
        float distanceToObstacle = Vector3.Distance(linearPosition, obstacleCenter);
        float requiredClearance = obstacleRadius + 0.5f; // Add safety margin
        
        // Calculate arc height - ensure it's enough to clear the obstacle
        float baseArcHeight = _arc * Vector3.Distance(_startPosition, targetPosition) * 0.5f;
        float clearanceArcHeight = Mathf.Max(0, requiredClearance - distanceToObstacle);
        float arcHeight = Mathf.Max(baseArcHeight, clearanceArcHeight) * Mathf.Sin(t * Mathf.PI);
        
        // Apply the arc offset
        Vector3 arcOffset = perpendicular * arcHeight;
        transform.position = linearPosition + arcOffset;

        // Check if arrived
        if (t >= 1f)
        {
            OnArrived();
        }
    }

    private void OnArrived()
    {
        _onRocketArrivedChannelSO.RaiseEvent();
        _isActive = false;

        Destroy(gameObject);
    }
}
