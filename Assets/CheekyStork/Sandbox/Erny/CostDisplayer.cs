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

    private void Awake()
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
        return 1;
    }

    private int GetCurrentRoomBuildCost()
    {
        return 1;
    }
}