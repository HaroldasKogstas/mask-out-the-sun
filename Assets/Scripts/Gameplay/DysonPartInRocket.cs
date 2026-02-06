using System;
using System.Collections;
using CheekyStork.ScriptableChannels;
using CheekyStork.ScriptableVariables;
using UnityEngine;

/// <summary>
/// Second stage rocket that flies from launch position to a Dyson sphere part position,
/// avoiding obstacles and matching orbital velocity before arrival.
/// </summary>
public class DysonPartInRocket : MonoBehaviour
{
    [Header("Target & Events")]
    [SerializeField] private TransformSO _nextDysonPartTransformSO;
    [SerializeField] private VoidEventChannelSO _dysonPartJourneyStartChannelSO;
    [SerializeField] private VoidEventChannelSO _onRocketArrivedChannelSO;

    [Header("Trajectory")]
    [SerializeField] private float _travelTime = 5f;
    [SerializeField] private AnimationCurve _easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField, Range(0, 1), Tooltip("How far out control points extend (0=straight, 1=wide curve)")]
    private float _curveAmount = 0.4f;
    
    [Header("Obstacle Avoidance")]
    [SerializeField] private Vector3 _obstacleCenter = Vector3.zero;
    [SerializeField, Tooltip("Control points will be placed this far from obstacle")]
    private float _obstacleAvoidanceDistance = 2f;

    [Header("Visual Effects")]
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private Transform _sphereMesh;
    [SerializeField] private AnimationCurve _squishCurve = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField, Range(0, 1)] private float _finalScale = 0.1f;

    [Header("Orientation")]
    [SerializeField] private bool _orientTowardsVelocity = true;
    [SerializeField] private float _rotationSpeed = 5f;

    [Header("Lifecycle")]
    [SerializeField] private float _destroyDelay = 2f;

    // State
    private Transform _target;
    private bool _isActive;
    private bool _hasArrived;
    private float _elapsedTime;
    private float _arrivalTime;
    private Coroutine _coroutine;

    // Bezier curve control points
    private Vector3 _p0; // Start position (fixed)
    private Vector3 _p1; // First control point (fixed)
    private Vector3 _p2; // Second control point (updates with target)
    private Vector3 _p3; // End position (updates with target)

    // Curve length tracking for consistent speed
    private float _initialCurveLength;
    private float _distanceTraveled;

    // Velocity tracking for orientation
    private Vector3 _previousPosition;

    // Visual effect caching
    private Vector3 _initialSphereScale;
    private float _initialTrailTime;
    private Color _initialTrailStartColor;
    private Color _initialTrailEndColor;

    public void Initialize()
    {
        _isActive = true;
        _previousPosition = transform.position;
        _elapsedTime = 0f;
        _target = _nextDysonPartTransformSO.Value;

        // Calculate fixed Bezier control points
        CalculateBezierControlPoints();
        
        // Calculate initial curve length for speed consistency
        _initialCurveLength = EstimateCurveLength();
        _distanceTraveled = 0f;

        // Store initial sphere scale for squish effect
        if (_sphereMesh != null)
        {
            _initialSphereScale = _sphereMesh.localScale;
        }

        // Store initial trail properties for fading
        if (_trail != null)
        {
            _initialTrailTime = _trail.time;
            _initialTrailStartColor = _trail.startColor;
            _initialTrailEndColor = _trail.endColor;
        }

        _dysonPartJourneyStartChannelSO.RaiseEvent();
    }

    private void CalculateBezierControlPoints()
    {
        _p0 = transform.position; // Start (fixed)
        Vector3 targetPos = _target.position;
        
        float distance = Vector3.Distance(_p0, targetPos);
        Vector3 toTarget = (targetPos - _p0).normalized;
        
        // Calculate direction away from obstacle
        Vector3 toObstacle = (_obstacleCenter - _p0).normalized;
        Vector3 awayFromObstacle = -toObstacle;
        
        // P1: Control point extends from start, away from obstacle
        Vector3 p1Direction = (toTarget + awayFromObstacle).normalized;
        _p1 = _p0 + p1Direction * (distance * _curveAmount);
        
        // Ensure P1 is outside obstacle avoidance zone
        if (Vector3.Distance(_p1, _obstacleCenter) < _obstacleAvoidanceDistance)
        {
            Vector3 fromObstacle = (_p1 - _obstacleCenter).normalized;
            _p1 = _obstacleCenter + fromObstacle * _obstacleAvoidanceDistance;
        }
    }
    
    private void UpdateDynamicBezierPoints()
    {
        // P3 always tracks target position
        _p3 = _target.position;
        
        float distance = Vector3.Distance(_p0, _p3);
        Vector3 toTarget = (_p3 - _p0).normalized;
        Vector3 toObstacle = (_obstacleCenter - _p3).normalized;
        Vector3 awayFromObstacle = -toObstacle;
        
        // P2: Control point extends back from target, away from obstacle  
        Vector3 p2Direction = (-toTarget + awayFromObstacle).normalized;
        _p2 = _p3 + p2Direction * (distance * _curveAmount);
        
        // Ensure P2 is outside obstacle avoidance zone
        if (Vector3.Distance(_p2, _obstacleCenter) < _obstacleAvoidanceDistance)
        {
            Vector3 fromObstacle = (_p2 - _obstacleCenter).normalized;
            _p2 = _obstacleCenter + fromObstacle * _obstacleAvoidanceDistance;
        }
    }

    void Update()
    {
        if (!_isActive) return;

        _elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(_elapsedTime / _travelTime);

        if (_hasArrived)
        {
            UpdatePostArrival();
            return;
        }

        if (progress >= 1f)
        {
            // Ensure we're exactly at target before marking as arrived
            transform.position = _target.position;
            OnArrived();
            return;
        }

        UpdateFlightTrajectory(progress);
    }

    private void UpdatePostArrival()
    {
        // Follow target as it moves in orbit
        transform.position = _target.position;
        transform.rotation = _target.rotation;
        
        float fadeProgress = Mathf.Clamp01((Time.time - _arrivalTime) / _destroyDelay);
        
        UpdateTrailFade(fadeProgress);
        UpdateSphereMeshSquish(fadeProgress);
    }

    private void UpdateFlightTrajectory(float t)
    {
        // Update control points to follow moving target
        UpdateDynamicBezierPoints();

        // Calculate desired speed based on initial curve length
        float desiredSpeed = _initialCurveLength / _travelTime;
        
        // Calculate position on Bezier curve
        float easedT = _easingCurve.Evaluate(t);
        Vector3 newPosition = CalculateCubicBezier(_p0, _p1, _p2, _p3, easedT);
        
        // Limit movement to maintain consistent speed
        Vector3 movement = newPosition - transform.position;
        float frameMoveDistance = movement.magnitude;
        float maxFrameDistance = desiredSpeed * Time.deltaTime * 2f; // 2x for safety margin
        
        if (frameMoveDistance > maxFrameDistance)
        {
            // Cap the movement to prevent sudden speed ups
            newPosition = transform.position + movement.normalized * maxFrameDistance;
        }

        // Orient rocket toward movement direction
        UpdateOrientation(newPosition);

        // Apply final position
        _previousPosition = transform.position;
        transform.position = newPosition;
    }

    private Vector3 CalculateCubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        // Cubic Bezier formula: B(t) = (1-t)³P0 + 3(1-t)²tP1 + 3(1-t)t²P2 + t³P3
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0; // (1-t)³ * P0
        point += 3f * uu * t * p1; // 3(1-t)² * t * P1
        point += 3f * u * tt * p2; // 3(1-t) * t² * P2
        point += ttt * p3; // t³ * P3

        return point;
    }

    private float EstimateCurveLength()
    {
        // Approximate curve length by sampling points
        int samples = 20;
        float totalLength = 0f;
        Vector3 previousPoint = _p0;
        
        for (int i = 1; i <= samples; i++)
        {
            float t = (float)i / samples;
            Vector3 currentPoint = CalculateCubicBezier(_p0, _p1, _p2, _p3, t);
            totalLength += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
        
        return totalLength;
    }

    private void UpdateOrientation(Vector3 newPosition)
    {
        if (!_orientTowardsVelocity) return;
        
        Vector3 velocity = newPosition - _previousPosition;
        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateTrailFade(float fadeProgress)
    {
        if (_trail == null || _destroyDelay <= 0) return;
        
        Color startColor = _initialTrailStartColor;
        startColor.a = Mathf.Lerp(_initialTrailStartColor.a, 0, fadeProgress);
        
        Color endColor = _initialTrailEndColor;
        endColor.a = Mathf.Lerp(_initialTrailEndColor.a, 0, fadeProgress);
        
        _trail.startColor = startColor;
        _trail.endColor = endColor;
        _trail.time = Mathf.Lerp(_initialTrailTime, 0, fadeProgress);
    }

    private void UpdateSphereMeshSquish(float fadeProgress)
    {
        if (_sphereMesh == null || _destroyDelay <= 0) return;
        
        float squishProgress = _squishCurve.Evaluate(fadeProgress);
        float scaleMultiplier = Mathf.Lerp(1f, _finalScale, squishProgress);
        _sphereMesh.localScale = _initialSphereScale * scaleMultiplier;
    }

    private void OnDestroy()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
    }

    private void OnArrived()
    {
        if(_hasArrived)
            return;
        
        _arrivalTime = Time.time;
        _onRocketArrivedChannelSO.RaiseEvent();

        _coroutine = StartCoroutine(DestroyCoroutine());

        _hasArrived = true;
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(_destroyDelay);

        _isActive = false;
        Destroy(gameObject);
    }
}
