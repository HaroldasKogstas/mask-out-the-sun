using UnityEngine;

public sealed class RoomUnlockManager : MonoBehaviour
{
    private const string SaveKey = "RoomUnlockManager.UnlockedCount";

    public static RoomUnlockManager Instance { get; private set; }

    [SerializeField]
    private RoomBalanceConfig _balanceConfig;

    [SerializeField]
    [Min(0)]
    private int _unlockedRoomsCount;

    public int UnlockedRoomsCount => _unlockedRoomsCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            UnityEngine.Object.Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    public int GetNextUnlockCostSurveyData()
    {
        if (_balanceConfig == null)
        {
            return 0;
        }

        return _balanceConfig.GetUnlockCostSurveyData(_unlockedRoomsCount);
    }

    public void RegisterUnlockConsumed()
    {
        _unlockedRoomsCount++;
        Save();
    }

    private void Save()
    {
        PlayerPrefs.SetInt(SaveKey, _unlockedRoomsCount);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        _unlockedRoomsCount = PlayerPrefs.GetInt(SaveKey, 0);
    }
}