using System;
using CheekyStork.ScriptableChannels;
using CheekyStork.ScriptableVariables;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class DysonGenerator : MonoBehaviour 
{
    [SerializeField]
    FloatSO _progressSO;

    private float Progress => Math.Clamp(_progressSO.Value, 0, 1);

    [field: SerializeField]
    protected float Radius {get;private set;} = 1f;

    [field: SerializeField]
    protected int NumPoints {get;private set;} = 50;

    [SerializeField]
    private TransformSO _nextDysonPartTransformSO;

    [SerializeField]
    private VoidEventChannelSO _dysonPartJourneyStartChannelSO;
    [SerializeField]
    private VoidEventChannelSO _rocketArrivalChannelSO;

    public int Step => Mathf.CeilToInt(NumPoints * Progress);

    [Button("Regenerate")]
    public abstract void Regenerate();

    float _previousProgress;

    void Awake()
    {
        _progressSO.Value = 0;
    }

    void OnEnable()
    {
        _dysonPartJourneyStartChannelSO.OnEventRaised += OnDysonPartJourneyStart;
        _rocketArrivalChannelSO.OnEventRaised += OnRocketArrived;
    }

    void OnDisable()
    {
        _dysonPartJourneyStartChannelSO.OnEventRaised -= OnDysonPartJourneyStart;
        _rocketArrivalChannelSO.OnEventRaised -= OnRocketArrived;
    }

    private void IterateProgressAStep()
    {
        float stepProgress = 1f / NumPoints;
        _progressSO.Value = Math.Clamp(_progressSO.Value + stepProgress, 0, 1);
    }

    protected virtual void Update()
    {
        if(Progress != _previousProgress)
        {
            _previousProgress = Progress;
            OnProgressChanged(Progress);
        }
    }

    protected abstract void OnDysonPartJourneyStart();

    protected virtual void OnRocketArrived()
    {
        IterateProgressAStep();    
    }

    protected virtual void OnProgressChanged(float progress)
    {
        Regenerate();
    }

    protected virtual void UpdateNextDysonPartTransform(Transform transform)
    {
        _nextDysonPartTransformSO.Value = transform;
    }
}