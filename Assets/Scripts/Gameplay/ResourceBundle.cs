using System;
using UnityEngine;

[Serializable]
public sealed class ResourceBundle
{
    [SerializeField]
    private ResourceData _iron = new ResourceData(ResourceType.Iron, 0);

    [SerializeField]
    private ResourceData _tungsten = new ResourceData(ResourceType.Tungsten, 0);

    [SerializeField]
    private ResourceData _coal = new ResourceData(ResourceType.Coal, 0);

    [SerializeField]
    private ResourceData _tungstenPlate = new ResourceData(ResourceType.TungstenPlate, 0);

    [SerializeField]
    private ResourceData _steelPlate = new ResourceData(ResourceType.SteelPlate, 0);

    [SerializeField]
    private ResourceData _researchData = new ResourceData(ResourceType.ResearchData, 0);

    [SerializeField]
    private ResourceData _surveyData = new ResourceData(ResourceType.SurveyData, 0);

    public int this[ResourceType type]
    {
        get
        {
            EnsureInitialized();
            return GetEntry(type).Amount;
        }
        set
        {
            EnsureInitialized();
            GetEntry(type).Amount = value;
        }
    }

    public bool CanAfford(ResourceBundle cost)
    {
        EnsureInitialized();
        cost.EnsureInitialized();

        return _iron.Amount >= cost._iron.Amount
            && _tungsten.Amount >= cost._tungsten.Amount
            && _coal.Amount >= cost._coal.Amount
            && _tungstenPlate.Amount >= cost._tungstenPlate.Amount
            && _steelPlate.Amount >= cost._steelPlate.Amount
            && _researchData.Amount >= cost._researchData.Amount
            && _surveyData.Amount >= cost._surveyData.Amount;
    }

    public void Add(ResourceBundle other)
    {
        EnsureInitialized();
        other.EnsureInitialized();

        _iron.Amount += other._iron.Amount;
        _tungsten.Amount += other._tungsten.Amount;
        _coal.Amount += other._coal.Amount;
        _tungstenPlate.Amount += other._tungstenPlate.Amount;
        _steelPlate.Amount += other._steelPlate.Amount;
        _researchData.Amount += other._researchData.Amount;
        _surveyData.Amount += other._surveyData.Amount;
    }

    public bool TrySpend(ResourceBundle cost)
    {
        EnsureInitialized();
        cost.EnsureInitialized();

        if (!CanAfford(cost))
        {
            return false;
        }

        _iron.Amount -= cost._iron.Amount;
        _tungsten.Amount -= cost._tungsten.Amount;
        _coal.Amount -= cost._coal.Amount;
        _tungstenPlate.Amount -= cost._tungstenPlate.Amount;
        _steelPlate.Amount -= cost._steelPlate.Amount;
        _researchData.Amount -= cost._researchData.Amount;
        _surveyData.Amount -= cost._surveyData.Amount;
        return true;
    }

    public ResourceData GetResourceData(ResourceType type)
    {
        EnsureInitialized();
        return GetEntry(type);
    }

    public override string ToString()
    {
        EnsureInitialized();
        return $"Iron: {_iron.Amount}, Tungsten: {_tungsten.Amount}, Coal: {_coal.Amount}, TungstenPlate: {_tungstenPlate.Amount}, SteelPlate: {_steelPlate.Amount}, ResearchData: {_researchData.Amount}, SurveyData: {_surveyData.Amount}";
    }

    private void EnsureInitialized()
    {
        if (_iron == null) _iron = new ResourceData(ResourceType.Iron, 0);
        if (_tungsten == null) _tungsten = new ResourceData(ResourceType.Tungsten, 0);
        if (_coal == null) _coal = new ResourceData(ResourceType.Coal, 0);
        if (_tungstenPlate == null) _tungstenPlate = new ResourceData(ResourceType.TungstenPlate, 0);
        if (_steelPlate == null) _steelPlate = new ResourceData(ResourceType.SteelPlate, 0);
        if (_researchData == null) _researchData = new ResourceData(ResourceType.ResearchData, 0);
        if (_surveyData == null) _surveyData = new ResourceData(ResourceType.SurveyData, 0);

        _iron.ForceType(ResourceType.Iron);
        _tungsten.ForceType(ResourceType.Tungsten);
        _coal.ForceType(ResourceType.Coal);
        _tungstenPlate.ForceType(ResourceType.TungstenPlate);
        _steelPlate.ForceType(ResourceType.SteelPlate);
        _researchData.ForceType(ResourceType.ResearchData);
        _surveyData.ForceType(ResourceType.SurveyData);
    }

    private ResourceData GetEntry(ResourceType type)
    {
        return type switch
        {
            ResourceType.Iron => _iron,
            ResourceType.Tungsten => _tungsten,
            ResourceType.Coal => _coal,
            ResourceType.TungstenPlate => _tungstenPlate,
            ResourceType.SteelPlate => _steelPlate,
            ResourceType.ResearchData => _researchData,
            ResourceType.SurveyData => _surveyData,
            _ => _coal
        };
    }
}
