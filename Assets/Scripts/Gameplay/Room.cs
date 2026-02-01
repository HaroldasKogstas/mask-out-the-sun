using System;
using UnityEngine;

public sealed class Room : MonoBehaviour
{
    public static event Action<Room> RocketLaunched;

    public event Action<Room, RoomActionResult> ActionSucceeded;
    public event Action<Room, ResourceBundle> ActionFailed;

    public event Action<Room> Unlocked;
    public event Action<Room> Built;
    public event Action<Room, int> Upgraded;

    [Header("Config")]
    [SerializeField]
    private RoomBalanceConfig _balanceConfig;

    [Header("Slot data (always present)")]
    [SerializeField]
    private ResourceType _underlyingElement = ResourceType.Iron; // only Iron/Tungsten/Coal expected

    [Header("Room state")]
    [SerializeField]
    private bool _isLocked;

    [SerializeField]
    private bool _isBuilt;

    [SerializeField]
    private RoomType _roomType;

    [SerializeField]
    [Range(0, 3)]
    private int _tierIndex;

    [Header("Smelter UI selection")]
    [SerializeField]
    private SmelterRecipe _smelterRecipe = SmelterRecipe.SteelPlate;

    [SerializeField]
    private bool _isRunning = true;

    [SerializeField]
    private float _elapsedSeconds;

    public ResourceType UnderlyingElement => _underlyingElement;
    public bool IsLocked => _isLocked;
    public bool IsBuilt => _isBuilt;
    public RoomType Type => _roomType;
    public int TierIndex => _tierIndex;

    public SmelterRecipe CurrentSmelterRecipe => _smelterRecipe;

    public bool ForceLockState(bool locked)
    {
        _isLocked = locked;
        
        if (locked)
        {
            _elapsedSeconds = 0f;
        }

        if (!locked)
        {
            Unlocked?.Invoke(this);
        }
        
        return _isLocked;
    }
    
    public float Progress01
    {
        get
        {
            if (_isLocked || !_isBuilt || !_isRunning)
            {
                return 0f;
            }

            float duration = GetCurrentActionDurationSeconds();
            if (duration <= 0.0001f)
            {
                return 1f;
            }

            return Mathf.Clamp01(_elapsedSeconds / duration);
        }
    }

    private void Update()
    {
        if (_isLocked || !_isBuilt || !_isRunning)
        {
            return;
        }

        if (_balanceConfig == null)
        {
            return;
        }

        float duration = GetCurrentActionDurationSeconds();
        if (duration <= 0.0001f)
        {
            return;
        }

        _elapsedSeconds += Time.deltaTime;

        int loops = 0;
        while (_elapsedSeconds >= duration)
        {
            _elapsedSeconds -= duration;
            PerformActionOnce();

            loops++;
            if (loops >= 5)
            {
                break;
            }
        }
    }

    public void SetRunning(bool running)
    {
        _isRunning = running;
        if (!running)
        {
            _elapsedSeconds = 0f;
        }
    }

    public void SetSmelterRecipe(SmelterRecipe recipe)
    {
        _smelterRecipe = recipe;
    }

    public int GetUnlockCostSurveyData()
    {
        if (_balanceConfig == null)
        {
            return 0;
        }

        if (!_isLocked)
        {
            return 0;
        }

        RoomUnlockManager unlockManager = RoomUnlockManager.Instance;
        if (unlockManager == null)
        {
            return 0;
        }

        return unlockManager.GetNextUnlockCostSurveyData();
    }

    public bool TryUnlock()
    {
        if (_balanceConfig == null)
        {
            return false;
        }

        if (!_isLocked)
        {
            return false;
        }

        RoomUnlockManager unlockManager = RoomUnlockManager.Instance;
        if (unlockManager == null)
        {
            return false;
        }

        ResourceManager resourceManager = ResourceManager.Instance;
        if (resourceManager == null)
        {
            return false;
        }

        int costAmount = unlockManager.GetNextUnlockCostSurveyData();
        if (costAmount <= 0)
        {
            _isLocked = false;
            _elapsedSeconds = 0f;
            Unlocked?.Invoke(this);
            return true;
        }

        ResourceBundle cost = new ResourceBundle();
        cost[ResourceType.SurveyData] = costAmount;

        if (!resourceManager.TryRemoveResourceBundle(cost))
        {
            ResourceBundle lacking = BuildLackingBundle(cost, resourceManager.Resources);
            ActionFailed?.Invoke(this, lacking);
            return false;
        }

        unlockManager.RegisterUnlockConsumed();
        _isLocked = false;
        _elapsedSeconds = 0f;

        Unlocked?.Invoke(this);
        return true;
    }

    public bool TryBuild(RoomType typeToBuild)
    {
        if (_balanceConfig == null)
        {
            return false;
        }

        if (_isLocked)
        {
            ResourceManager managerLocked = ResourceManager.Instance;
            if (managerLocked != null)
            {
                ResourceBundle lacking = new ResourceBundle();
                lacking[ResourceType.SurveyData] = Mathf.Max(0, GetUnlockCostSurveyData());
                if (lacking[ResourceType.SurveyData] > 0)
                {
                    ActionFailed?.Invoke(this, lacking);
                }
            }

            return false;
        }

        if (_isBuilt)
        {
            return false;
        }

        ResourceManager manager = ResourceManager.Instance;
        if (manager == null)
        {
            return false;
        }

        RoomBalanceConfig.RoomTypeBalance balance = _balanceConfig.GetBalance(typeToBuild);

        ResourceBundle cost = new ResourceBundle();
        cost[ResourceType.SteelPlate] = balance.BuildSteelPlateCost;

        if (!manager.TryRemoveResourceBundle(cost))
        {
            ResourceBundle lacking = BuildLackingBundle(cost, manager.Resources);
            ActionFailed?.Invoke(this, lacking);
            return false;
        }

        _isBuilt = true;
        _roomType = typeToBuild;
        _tierIndex = 0;
        _elapsedSeconds = 0f;

        Built?.Invoke(this);
        return true;
    }

    public int GetUpgradeCostResearchData()
    {
        if (_balanceConfig == null)
        {
            return 0;
        }

        if (_isLocked || !_isBuilt)
        {
            return 0;
        }

        if (_tierIndex >= 3)
        {
            return 0;
        }

        RoomBalanceConfig.RoomTypeBalance balance = _balanceConfig.GetBalance(_roomType);
        return balance.GetUpgradeResearchCostToNextTier(_tierIndex);
    }

    public bool TryUpgrade()
    {
        if (_balanceConfig == null)
        {
            return false;
        }

        if (_isLocked || !_isBuilt)
        {
            return false;
        }

        if (_tierIndex >= 3)
        {
            return false;
        }

        ResourceManager manager = ResourceManager.Instance;
        if (manager == null)
        {
            return false;
        }

        RoomBalanceConfig.RoomTypeBalance balance = _balanceConfig.GetBalance(_roomType);
        int costAmount = balance.GetUpgradeResearchCostToNextTier(_tierIndex);

        ResourceBundle cost = new ResourceBundle();
        cost[ResourceType.ResearchData] = costAmount;

        if (!manager.TryRemoveResourceBundle(cost))
        {
            ResourceBundle lacking = BuildLackingBundle(cost, manager.Resources);
            ActionFailed?.Invoke(this, lacking);
            return false;
        }

        _tierIndex++;
        _elapsedSeconds = 0f;

        Upgraded?.Invoke(this, _tierIndex);
        return true;
    }

    public float GetCurrentActionDurationSeconds()
    {
        if (_balanceConfig == null)
        {
            return 0f;
        }

        RoomBalanceConfig.RoomTypeBalance balance = _balanceConfig.GetBalance(_roomType);
        return balance.GetActionSeconds(_tierIndex);
    }

    private void PerformActionOnce()
    {
        ResourceManager manager = ResourceManager.Instance;
        if (manager == null)
        {
            return;
        }

        ResourceBundle cost = new ResourceBundle();
        ResourceBundle produced = new ResourceBundle();

        if (!BuildRecipe(cost, produced))
        {
            return;
        }

        if (!IsZeroBundle(cost))
        {
            if (!manager.TryRemoveResourceBundle(cost))
            {
                ResourceBundle lacking = BuildLackingBundle(cost, manager.Resources);
                ActionFailed?.Invoke(this, lacking);
                return;
            }
        }

        if (!IsZeroBundle(produced))
        {
            manager.AddResourceBundle(produced);
        }

        if (_roomType == RoomType.Assembler)
        {
            RocketLaunched?.Invoke(this);
        }

        RoomActionResult result = new RoomActionResult(cost, produced);
        ActionSucceeded?.Invoke(this, result);
    }

    private bool BuildRecipe(ResourceBundle cost, ResourceBundle produced)
    {
        if (_balanceConfig == null)
        {
            return false;
        }

        if (_isLocked || !_isBuilt)
        {
            return false;
        }

        if (_roomType == RoomType.Miner)
        {
            int output = _balanceConfig.MinerOutputAmount;
            ResourceType outputType = MapUnderlyingToRawResource(_underlyingElement);
            produced[outputType] = output;
            return true;
        }

        if (_roomType == RoomType.Smelter)
        {
            int coalCost = _balanceConfig.SmelterCoalCost;
            int metalCost = _balanceConfig.SmelterMetalCost;
            int plateOut = _balanceConfig.SmelterPlateOutput;

            cost[ResourceType.Coal] = coalCost;

            if (_smelterRecipe == SmelterRecipe.SteelPlate)
            {
                cost[ResourceType.Iron] = metalCost;
                produced[ResourceType.SteelPlate] = plateOut;
            }
            else
            {
                cost[ResourceType.Tungsten] = metalCost;
                produced[ResourceType.TungstenPlate] = plateOut;
            }

            return true;
        }

        if (_roomType == RoomType.Surveyor)
        {
            cost[ResourceType.TungstenPlate] = _balanceConfig.SurveyorTungstenPlateCost;
            produced[ResourceType.SurveyData] = _balanceConfig.SurveyorSurveyDataOutput;
            return true;
        }

        if (_roomType == RoomType.Researcher)
        {
            cost[ResourceType.SurveyData] = _balanceConfig.ResearcherSurveyDataCost;
            produced[ResourceType.ResearchData] = _balanceConfig.ResearcherResearchDataOutput;
            return true;
        }

        if (_roomType == RoomType.Assembler)
        {
            cost[ResourceType.Coal] = _balanceConfig.AssemblerCoalCost;
            cost[ResourceType.Tungsten] = _balanceConfig.AssemblerTungstenCost;
            return true;
        }

        return false;
    }

    private static ResourceType MapUnderlyingToRawResource(ResourceType underlying)
    {
        if (underlying == ResourceType.Tungsten) return ResourceType.Tungsten;
        if (underlying == ResourceType.Coal) return ResourceType.Coal;
        return ResourceType.Iron;
    }

    private static bool IsZeroBundle(ResourceBundle bundle)
    {
        return bundle[ResourceType.Iron] == 0
            && bundle[ResourceType.Tungsten] == 0
            && bundle[ResourceType.Coal] == 0
            && bundle[ResourceType.TungstenPlate] == 0
            && bundle[ResourceType.SteelPlate] == 0
            && bundle[ResourceType.ResearchData] == 0
            && bundle[ResourceType.SurveyData] == 0;
    }

    private static ResourceBundle BuildLackingBundle(ResourceBundle required, ResourceBundle available)
    {
        ResourceBundle lacking = new ResourceBundle();

        FillIfLacking(ResourceType.Iron, required, available, lacking);
        FillIfLacking(ResourceType.Tungsten, required, available, lacking);
        FillIfLacking(ResourceType.Coal, required, available, lacking);
        FillIfLacking(ResourceType.TungstenPlate, required, available, lacking);
        FillIfLacking(ResourceType.SteelPlate, required, available, lacking);
        FillIfLacking(ResourceType.ResearchData, required, available, lacking);
        FillIfLacking(ResourceType.SurveyData, required, available, lacking);

        return lacking;
    }

    private static void FillIfLacking(ResourceType type, ResourceBundle required, ResourceBundle available, ResourceBundle lacking)
    {
        int need = required[type];
        if (need <= 0)
        {
            return;
        }

        int have = available[type];
        if (have >= need)
        {
            return;
        }

        lacking[type] = need - have;
    }

    [Serializable]
    public readonly struct RoomActionResult
    {
        public ResourceBundle Consumed { get; }
        public ResourceBundle Produced { get; }

        public RoomActionResult(ResourceBundle consumed, ResourceBundle produced)
        {
            Consumed = consumed;
            Produced = produced;
        }
    }
}