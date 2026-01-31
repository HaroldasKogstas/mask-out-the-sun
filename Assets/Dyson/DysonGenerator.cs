using System;
using CheekyStork.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class DysonGenerator : MonoBehaviour 
{
    [SerializeField]
    FloatSO _progressSO;

    private float Progress => Math.Clamp(_progressSO.Value, 0, 1);

    [field: SerializeField]
    protected float Radius {get;private set;} = 1f;

    [field: SerializeField]
    protected int NumPoints {get;private set;} = 50;

    public int Step => Mathf.CeilToInt(NumPoints * Progress);

    [Button("Regenerate")]
    public abstract void Regenerate();

    float _previousProgress;

    protected virtual void Update()
    {
        if(Progress != _previousProgress)
        {
            _previousProgress = Progress;
            OnProgressChanged(Progress);
        }
    }

    protected virtual void OnProgressChanged(float progress)
    {
        Regenerate();
    }
}