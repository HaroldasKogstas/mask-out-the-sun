using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(11000)]
public sealed class CinemachineCustomCurveBlender : MonoBehaviour
{
    [Serializable]
    private sealed class BlendRule
    {
        [SerializeField]
        private CinemachineVirtualCameraBase _from;

        [SerializeField]
        private CinemachineVirtualCameraBase _to;

        [SerializeField]
        private AnimationCurve _curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        [SerializeField]
        private float _durationSeconds = 1.0f;

        public CinemachineVirtualCameraBase From => _from;
        public CinemachineVirtualCameraBase To => _to;
        public AnimationCurve Curve => _curve;
        public float DurationSeconds => _durationSeconds;
    }

    [Header("References")]
    [SerializeField]
    private CinemachineBrain _brain;

    [Header("Default")]
    [SerializeField]
    private AnimationCurve _defaultCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

    [SerializeField]
    private float _defaultDurationSeconds = 1.0f;

    [Header("Per-Pair Rules")]
    [SerializeField]
    private List<BlendRule> _rules = new List<BlendRule>();

    [Header("Timing")]
    [SerializeField]
    private bool _useUnscaledTime;

    [SerializeField]
    private int _overridePriority = 100;

    // Allocated by SetCameraOverride (must be > 0 once allocated).
    private int _overrideId = -1;

    private bool _isBlending;
    private float _blendStartTime;
    private float _blendDuration;
    private AnimationCurve _activeCurve;

    private ICinemachineCamera _fromCam;
    private ICinemachineCamera _toCam;

    // Guards against re-entrant activation events caused by our own override calls.
    private bool _suppressActivation;

    private void Awake()
    {
        if (_brain == null)
        {
            _brain = GetComponent<CinemachineBrain>();
        }
    }

    private void OnEnable()
    {
        CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivated);
    }

    private void OnDisable()
    {
        CinemachineCore.CameraActivatedEvent.RemoveListener(OnCameraActivated);
        StopBlendAndReleaseOverride();
    }

    private void LateUpdate()
    {
        if (!_isBlending || _brain == null || _fromCam == null || _toCam == null)
        {
            return;
        }

        float elapsed = GetTime() - _blendStartTime;
        float normalized = _blendDuration > 0.0001f ? Mathf.Clamp01(elapsed / _blendDuration) : 1.0f;

        float weightB = normalized;
        if (_activeCurve != null)
        {
            weightB = _activeCurve.Evaluate(normalized);
        }

        weightB = Mathf.Clamp01(weightB);

        _suppressActivation = true;
        try
        {
            // Drive the output every frame.
            _overrideId = _brain.SetCameraOverride(
                _overrideId,
                _overridePriority,
                _fromCam,
                _toCam,
                weightB,
                GetDeltaTime());
        }
        finally
        {
            _suppressActivation = false;
        }

        if (normalized >= 1.0f)
        {
            StopBlendAndReleaseOverride();
        }
    }

    private void OnCameraActivated(ICinemachineCamera.ActivationEventParams evt)
    {
        if (_suppressActivation)
        {
            return;
        }

        if (_brain == null)
        {
            return;
        }

        if (evt.IncomingCamera == null || evt.OutgoingCamera == null)
        {
            return;
        }

        if (evt.IsCut)
        {
            StopBlendAndReleaseOverride();
            return;
        }

        // If we're already blending this exact pair, ignore repeated events.
        if (_isBlending && ReferenceEquals(evt.OutgoingCamera, _fromCam) && ReferenceEquals(evt.IncomingCamera, _toCam))
        {
            return;
        }

        BlendRule rule = FindRule(evt.OutgoingCamera, evt.IncomingCamera);

        _fromCam = evt.OutgoingCamera;
        _toCam = evt.IncomingCamera;

        _activeCurve = rule != null ? rule.Curve : _defaultCurve;
        _blendDuration = rule != null ? Mathf.Max(0.0f, rule.DurationSeconds) : Mathf.Max(0.0f, _defaultDurationSeconds);

        _blendStartTime = GetTime();
        _isBlending = true;

        // IMPORTANT: Do NOT call SetCameraOverride here. That can re-fire activation events.
        // LateUpdate will begin driving the override next frame (starting near weight 0).
    }

    private BlendRule FindRule(ICinemachineCamera from, ICinemachineCamera to)
    {
        CinemachineVirtualCameraBase fromVcam = from as CinemachineVirtualCameraBase;
        CinemachineVirtualCameraBase toVcam = to as CinemachineVirtualCameraBase;

        if (fromVcam == null || toVcam == null)
        {
            return null;
        }

        int count = _rules.Count;
        for (int i = 0; i < count; i++)
        {
            BlendRule rule = _rules[i];
            if (rule == null)
            {
                continue;
            }

            if (ReferenceEquals(rule.From, fromVcam) && ReferenceEquals(rule.To, toVcam))
            {
                return rule;
            }
        }

        return null;
    }

    private void StopBlendAndReleaseOverride()
    {
        _isBlending = false;
        _fromCam = null;
        _toCam = null;
        _activeCurve = null;
        _blendDuration = 0.0f;

        if (_brain != null && _overrideId > 0)
        {
            _suppressActivation = true;
            try
            {
                _brain.ReleaseCameraOverride(_overrideId);
            }
            finally
            {
                _suppressActivation = false;
            }
        }

        _overrideId = -1;
    }

    private float GetTime()
    {
        return _useUnscaledTime ? Time.unscaledTime : Time.time;
    }

    private float GetDeltaTime()
    {
        return _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
    }
}
