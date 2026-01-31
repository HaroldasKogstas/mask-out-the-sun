using System.Collections.Generic;
using UnityEngine;

public sealed class ResourceManager : MonoBehaviour
{
    private const string SaveKey = "ResourceManager.PlayerResources";

    public static ResourceManager Instance { get; private set; }

    [SerializeField]
    private ResourceBundle _resources = new ResourceBundle();

    public ResourceBundle Resources => _resources;
    
    [SerializeField]
    private bool _resetOnAwake = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            UnityEngine.Object.Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_resetOnAwake)
        {
            _resources = new ResourceBundle();
            Save();
        }
        
        Load();
    }

    public void AddResource(ResourceData data)
    {
        if (data == null)
        {
            return;
        }

        int amount = Mathf.Max(0, data.Amount);
        if (amount == 0)
        {
            return;
        }

        _resources[data.Type] = _resources[data.Type] + amount;
        Save();
    }

    public bool TryRemoveResource(ResourceData data)
    {
        if (data == null)
        {
            return false;
        }

        int amount = Mathf.Max(0, data.Amount);
        if (amount == 0)
        {
            return true;
        }

        ResourceType type = data.Type;
        int current = _resources[type];

        if (current < amount)
        {
            return false;
        }

        _resources[type] = current - amount;
        Save();
        return true;
    }

    public void AddResources(List<ResourceData> datas)
    {
        if (datas == null || datas.Count == 0)
        {
            return;
        }

        for (int i = 0; i < datas.Count; i++)
        {
            ResourceData data = datas[i];
            if (data == null)
            {
                continue;
            }

            int amount = Mathf.Max(0, data.Amount);
            if (amount == 0)
            {
                continue;
            }

            _resources[data.Type] = _resources[data.Type] + amount;
        }

        Save();
    }

    public bool TryRemoveResources(List<ResourceData> datas)
    {
        if (datas == null || datas.Count == 0)
        {
            return true;
        }

        int ironCost = 0;
        int tungstenCost = 0;
        int coalCost = 0;

        for (int i = 0; i < datas.Count; i++)
        {
            ResourceData data = datas[i];
            if (data == null)
            {
                continue;
            }

            int amount = Mathf.Max(0, data.Amount);
            if (amount == 0)
            {
                continue;
            }

            ResourceType type = data.Type;
            if (type == ResourceType.Iron)
            {
                ironCost += amount;
            }
            else if (type == ResourceType.Tungsten)
            {
                tungstenCost += amount;
            }
            else if (type == ResourceType.Coal)
            {
                coalCost += amount;
            }
        }

        if (_resources[ResourceType.Iron] < ironCost)
        {
            return false;
        }

        if (_resources[ResourceType.Tungsten] < tungstenCost)
        {
            return false;
        }

        if (_resources[ResourceType.Coal] < coalCost)
        {
            return false;
        }

        _resources[ResourceType.Iron] = _resources[ResourceType.Iron] - ironCost;
        _resources[ResourceType.Tungsten] = _resources[ResourceType.Tungsten] - tungstenCost;
        _resources[ResourceType.Coal] = _resources[ResourceType.Coal] - coalCost;

        Save();
        return true;
    }

    public void AddResourceBundle(ResourceBundle bundle)
    {
        if (bundle == null)
        {
            return;
        }

        _resources.Add(bundle);
        Save();
    }

    public bool TryRemoveResourceBundle(ResourceBundle bundle)
    {
        if (bundle == null)
        {
            return true;
        }

        bool success = _resources.TrySpend(bundle);
        if (!success)
        {
            return false;
        }

        Save();
        return true;
    }

    private void Save()
    {
        string json = JsonUtility.ToJson(_resources);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            if (_resources == null)
            {
                _resources = new ResourceBundle();
            }

            Save();
            return;
        }

        string json = PlayerPrefs.GetString(SaveKey, string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            _resources = new ResourceBundle();
            Save();
            return;
        }

        ResourceBundle loaded = JsonUtility.FromJson<ResourceBundle>(json);
        _resources = loaded ?? new ResourceBundle();
    }
}
