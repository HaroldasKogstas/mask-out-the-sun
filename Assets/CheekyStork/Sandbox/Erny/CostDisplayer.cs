using System.Linq;
using TMPro;
using UnityEngine;

public class CostDisplayer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _roomSurveyCostDisplay;

    [SerializeField]
    private TextMeshProUGUI _roomBuildCostDisplay;

    [SerializeField]
    private RoomBalanceConfig _roomBalanceConfig;

    [SerializeField]
    private RoomUnlockManager _roomUnlockManager;

    private void Update()
    {
        UpdateCostDisplays();
    }

    private void UpdateCostDisplays()
    {
        string roomSurveyCost = GetCurrentRoomSurveyCost().ToString();
        string roomBuildCost = GetCurrentRoomBuildCost().ToString();

        _roomSurveyCostDisplay.text = roomSurveyCost;
        _roomBuildCostDisplay.text = roomBuildCost;
    }

    private int GetCurrentRoomSurveyCost()
    {
        return _roomBalanceConfig.GetUnlockCostSurveyData(_roomUnlockManager.UnlockedRoomsCount);
    }

    private int GetCurrentRoomBuildCost()
    {
        var minerBalance = _roomBalanceConfig.Balances.FirstOrDefault(b => b.Type == RoomType.Miner);
        var minerBuildCost = minerBalance.BuildSteelPlateCost;
        return _roomBalanceConfig.GetBuildCostSteelPlates(minerBuildCost, _roomUnlockManager);
    }
}