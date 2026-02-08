using System;
using System.Collections.Generic;
using CheekyStork.Logging;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Game/Rooms/Room Balance Config")]
public sealed class RoomBalanceConfig : ScriptableObject
{
    [Header("Unlocking (beyond first N rooms)")]
    [SerializeField]
    [Min(0)]
    private int _freeRoomsCount = 8;

    [SerializeField]
    [Min(1)]
    private int _unlockBaseSurveyCost = 100;

    [SerializeField]
    [Min(1f)]
    private float _unlockMultiplier = 1.2f;

    [Header("Building cost scaling")]
    [SerializeField]
    [Min(0f)]
    private float _buildCostMultiplier = 1.15f;

    [Header("Recipes (all adjustable)")]
    [SerializeField]
    [Min(0)]
    private int _minerOutputAmount = 10;

    [SerializeField]
    [Min(0)]
    private int _smelterCoalCost = 1;

    [SerializeField]
    [Min(0)]
    private int _smelterMetalCost = 10;

    [SerializeField]
    [Min(0)]
    private int _smelterPlateOutput = 10;

    [SerializeField]
    [Min(0)]
    private int _surveyorTungstenPlateCost = 1;

    [SerializeField]
    [Min(0)]
    private int _surveyorSurveyDataOutput = 10;

    [SerializeField]
    [Min(0)]
    private int _researcherSurveyDataCost = 1;

    [SerializeField]
    [Min(0)]
    private int _researcherResearchDataOutput = 10;

    [SerializeField]
    [Min(0)]
    private int _assemblerCoalCost = 10;

    [SerializeField]
    [Min(0)]
    private int _assemblerTungstenPlateCost = 100;

    [SerializeField]
    [Min(0)]
    private int _assemblerSteelPlateCost = 100;

    [Header("Per-room balance")]
    [SerializeField]
    private List<RoomTypeBalance> _balances = new List<RoomTypeBalance>();

    
    public List<RoomTypeBalance> Balances => _balances;
    public int FreeRoomsCount => _freeRoomsCount;

    public int GetUnlockCostSurveyData(int currentlyUnlockedRoomsCount)
    {
        int extraUnlockedCount = Mathf.Max(0, currentlyUnlockedRoomsCount - _freeRoomsCount + 1);
        if (extraUnlockedCount <= 0)
        {
            return 0;
        }

        int index = extraUnlockedCount - 1; // first extra room
        float raw = _unlockBaseSurveyCost * Mathf.Pow(_unlockMultiplier, index);
        return Mathf.RoundToInt(raw);
    }

    public int GetBuildCostSteelPlates(int baseBuildSteelPlateCost, RoomUnlockManager unlockManager)
    {
        int baseCost = Mathf.Max(0, baseBuildSteelPlateCost);
        if (baseCost == 0)
        { 
            return 0;
        }

        if (unlockManager == null)
        {
            return baseCost;
        }

        int builtRoomsCount = Mathf.Max(0, unlockManager.UnlockedRoomsCount);
        int extraBuiltCount = Mathf.Max(0, builtRoomsCount - _freeRoomsCount);
        if (extraBuiltCount <= 0)
        { 
            return baseCost;
        }

        float raw = baseCost * Mathf.Pow(_buildCostMultiplier, extraBuiltCount);
        return Mathf.Max(0, Mathf.RoundToInt(raw));
    }

    public int MinerOutputAmount => _minerOutputAmount;

    public int SmelterCoalCost => _smelterCoalCost;
    public int SmelterMetalCost => _smelterMetalCost;
    public int SmelterPlateOutput => _smelterPlateOutput;

    public int SurveyorTungstenPlateCost => _surveyorTungstenPlateCost;
    public int SurveyorSurveyDataOutput => _surveyorSurveyDataOutput;

    public int ResearcherSurveyDataCost => _researcherSurveyDataCost;
    public int ResearcherResearchDataOutput => _researcherResearchDataOutput;

    public int AssemblerCoalCost => _assemblerCoalCost;
    public int AssemblerTungstenPlateCost => _assemblerTungstenPlateCost;
    public int AssemblerSteelPlateCost => _assemblerSteelPlateCost;

    public RoomTypeBalance GetBalance(RoomType type)
    {
        for (int i = 0; i < _balances.Count; i++)
        {
            if (_balances[i].Type == type)
            {
                return _balances[i];
            }
        }

        return RoomTypeBalance.CreateDefault(type);
    }

    private void OnValidate()
    {
        if (_balances == null)
        {
            _balances = new List<RoomTypeBalance>();
        }

        RoomType[] types = (RoomType[])Enum.GetValues(typeof(RoomType));

        for (int i = 0; i < types.Length; i++)
        {
            bool found = false;

            for (int j = 0; j < _balances.Count; j++)
            {
                if (_balances[j].Type == types[i])
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                _balances.Add(RoomTypeBalance.CreateDefault(types[i]));
            }
        }

        _buildCostMultiplier = Mathf.Max(1f, _buildCostMultiplier);
    }

    [Serializable]
    public struct RoomTypeBalance
    {
        [SerializeField]
        private RoomType _type;
        
        [Header("Build cost")]
        [SerializeField]
        [Min(0)]
        private int _buildSteelPlateCost;

        [Header("Action time per tier (4 tiers)")]
        [SerializeField]
        private float[] _actionSecondsByTier;

        [Header("Upgrade cost in ResearchData for next tier (3 upgrades)")]
        [SerializeField]
        private int[] _upgradeResearchCostByStep;

        public RoomType Type => _type;
        public int BuildSteelPlateCost => _buildSteelPlateCost;

        public float GetActionSeconds(int tierIndex)
        {
            if (_actionSecondsByTier == null || _actionSecondsByTier.Length < 4)
            {
                return DefaultActionSeconds(tierIndex);
            }

            int clamped = Mathf.Clamp(tierIndex, 0, 5);
            return Mathf.Max(0.01f, _actionSecondsByTier[clamped]);
        }

        public int GetUpgradeResearchCostToNextTier(int currentTierIndex)
        {
            if (currentTierIndex < 0 || currentTierIndex >= 5)
            {
                return 0;
            }

            if (_upgradeResearchCostByStep == null || _upgradeResearchCostByStep.Length < 5)
            {
                return DefaultUpgradeCost(currentTierIndex);
            }

            return Mathf.Max(0, _upgradeResearchCostByStep[currentTierIndex]);
        }

        public static RoomTypeBalance CreateDefault(RoomType type)
        {
            RoomTypeBalance balance = new RoomTypeBalance
            {
                _type = type,
                _buildSteelPlateCost = 0,
                _actionSecondsByTier = new float[6] { 16f, 8f, 4f, 2f, 1f, 0.5f },
                _upgradeResearchCostByStep = new int[5] { 10, 50, 250, 1250, 6000 }
            };

            return balance;
        }

        private static float DefaultActionSeconds(int tierIndex)
        {
            int clamped = Mathf.Clamp(tierIndex, 0, 5);
            if (clamped == 0) return 16f;
            if (clamped == 1) return 8f;
            if (clamped == 2) return 4f;
            if (clamped == 3) return 2f;
            if (clamped == 4) return 1f;
            return 0.5f;
        }

        private static int DefaultUpgradeCost(int stepIndex)
        {
            if (stepIndex == 0) return 10;
            if (stepIndex == 1) return 50;
            if (stepIndex == 2) return 250;
            if (stepIndex == 3) return 1250;
            return 6000;
        }
    }
}
