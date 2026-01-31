using System;
using UnityEngine;

[Serializable]
public sealed class ResourceData
{
    [SerializeField]
    [HideInInspector]
    private ResourceType _type;

    [SerializeField]
    [Min(0)]
    private int _amount;

    public ResourceType Type => _type;

    public int Amount
    {
        get => _amount;
        set => _amount = Mathf.Max(0, value);
    }

    public ResourceData(ResourceType type, int amount)
    {
        _type = type;
        _amount = Mathf.Max(0, amount);
    }

    internal void ForceType(ResourceType type)
    {
        _type = type;
    }
}