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
    [SerializeField, Range(0, 1), Tooltip("Arc intensity (0=straight, 1=max curve). Internally scaled to safe values.")]
    private float _arc = 0.5f;
    [SerializeField] private AnimationCurve _easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Obstacle Avoidance")]
    [SerializeField] private Vector3 _obstacleCenter = Vector3.zero;
    [SerializeField] private float _obstacleRadius = 1f;
    [SerializeField, Tooltip("Safety clearance added to obstacle radius")]
    private float _obstacleSafetyMargin = 0.5f;

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

    // Trajectory calculation
    private Vector3 _startPosition;
    private Vector3 _targetStartPosition;
    private Vector3 _arcDirection;

    // Velocity tracking for orbital matching
    private Vector3 _previousPosition;
    private Vector3 _previousTargetPosition;
    private Vector3 _targetOrbitalVelocity;

    // Visual effect caching
    private Vector3 _initialSphereScale;
    private float _initialTrailTime;
    private Color _initialTrailStartColor;
    private Color _initialTrailEndColor;

    // Constants
    private const float MAX_ARC_RATIO = 0.3f; // Max arc is 30% of travel distance
    private const float ORBITAL_BLEND_START = 0.7f; // Start velocity matching at 70% progress
    private const float ORBITAL_BLEND_END = 0.95f; // Stop at 95% to ensure exact arrival
    private const float ORBITAL_BLEND_STRENGTH = 0.5f; // Max velocity influence

    public void Initialize()
    {
        _isActive = true;
        _startPosition = transform.position;
        _previousPosition = transform.position;
        _elapsedTime = 0f;

        _target = _nextDysonPartTransformSO.Value;

        // Track initial target position for orbital velocity
        _targetStartPosition = _target.position;
        _previousTargetPosition = _target.position;
        _targetOrbitalVelocity = Vector3.zero;

        CalculateArcDirection();

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

    private void CalculateArcDirection()
    {
        Vector3 targetPosition = _target.position;
        Vector3 travelDirection = (targetPosition - _startPosition).normalized;
        Vector3 midPoint = (_startPosition + targetPosition) / 2f;
        Vector3 toObstacle = _obstacleCenter - midPoint;
        
        _arcDirection = Vector3.Cross(travelDirection, toObstacle.normalized);
        if (_arcDirection.magnitude < 0.01f)
        {
            _arcDirection = Vector3.Cross(travelDirection, Vector3.up);
            if (_arcDirection.magnitude < 0.01f)
            {
                _arcDirection = Vector3.Cross(travelDirection, Vector3.forward);
            }
        }
        _arcDirection = _arcDirection.normalized;
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
        // Track target orbital motion
        UpdateTargetVelocity();

        // Calculate position with arc trajectory
        Vector3 newPosition = CalculateTrajectoryPosition(t);

        // Blend toward orbital velocity as we approach
        newPosition = ApplyOrbitalVelocityBlending(newPosition, t);

        // Ensure we never enter the obstacle
        newPosition = EnforceObstacleAvoidance(newPosition);

        // In final 5%, smoothly blend to exact target position to eliminate snap
        if (t > 0.95f)
        {
            float finalBlend = (t - 0.95f) / 0.05f; // 0 to 1 over last 5%
            newPosition = Vector3.Lerp(newPosition, _target.position, finalBlend);
        }

        // Orient rocket toward movement direction
        UpdateOrientation(newPosition);

        // Apply final position
        _previousPosition = transform.position;
        transform.position = newPosition;
    }

    private void UpdateTargetVelocity()
    {
        Vector3 currentTargetPosition = _target.position;
        _targetOrbitalVelocity = (currentTargetPosition - _previousTargetPosition) / Time.deltaTime;
        _previousTargetPosition = currentTargetPosition;
    }

    private Vector3 CalculateTrajectoryPosition(float t)
    {
        float easedT = _easingCurve.Evaluate(t);
        Vector3 currentTargetPosition = _target.position;
        
        // Base linear interpolation toward moving target
        Vector3 linearPosition = Vector3.Lerp(_startPosition, currentTargetPosition, easedT);
        
        // Calculate arc to avoid obstacle and add visual curve
        float arcHeight = CalculateArcHeight(t, linearPosition);
        
        return linearPosition + _arcDirection * arcHeight;
    }

    private float CalculateArcHeight(float t, Vector3 linearPosition)
    {
        float travelDistance = Vector3.Distance(_startPosition, _targetStartPosition);
        
        // Base arc for visual curve
        float baseArcHeight = _arc * travelDistance * MAX_ARC_RATIO;
        
        // Extra arc if too close to obstacle
        float distanceToObstacle = Vector3.Distance(linearPosition, _obstacleCenter);
        float requiredClearance = _obstacleRadius + _obstacleSafetyMargin;
        float clearanceArcHeight = Mathf.Max(0, requiredClearance - distanceToObstacle);
        
        // Use sine wave so arc naturally goes to 0 at arrival
        float arcMultiplier = Mathf.Sin(t * Mathf.PI);
        
        return Mathf.Max(baseArcHeight, clearanceArcHeight) * arcMultiplier;
    }

    private Vector3 ApplyOrbitalVelocityBlending(Vector3 position, float t)
    {
        // Calculate blend factor (ramps up, then down near arrival)
        float blendRange = ORBITAL_BLEND_END - ORBITAL_BLEND_START;
        float orbitalBlend = Mathf.Clamp01((t - ORBITAL_BLEND_START) / blendRange);
        orbitalBlend = Mathf.Min(orbitalBlend, 1f - (t - ORBITAL_BLEND_END) / (1f - ORBITAL_BLEND_END));
        
        if (orbitalBlend <= 0 || _targetOrbitalVelocity.sqrMagnitude < 0.001f)
            return position;

        // Blend our velocity toward target's orbital velocity
        Vector3 ourVelocity = (position - _previousPosition) / Time.deltaTime;
        Vector3 blendedVelocity = Vector3.Lerp(ourVelocity, _targetOrbitalVelocity, orbitalBlend * ORBITAL_BLEND_STRENGTH);
        
        // Apply velocity adjustment
        Vector3 velocityOffset = (blendedVelocity - ourVelocity) * Time.deltaTime * orbitalBlend;
        return position + velocityOffset;
    }

    private Vector3 EnforceObstacleAvoidance(Vector3 position)
    {
        Vector3 toPosition = position - _obstacleCenter;
        float currentDistance = toPosition.magnitude;
        float minimumDistance = _obstacleRadius + _obstacleSafetyMargin;
        
        if (currentDistance < minimumDistance)
        {
            // Push radially outward to safe distance
            return _obstacleCenter + toPosition.normalized * minimumDistance;
        }
        
        return position;
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
