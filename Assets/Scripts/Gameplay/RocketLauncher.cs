using System;
using System.Collections.Generic;
using CheekyStork.ScriptableChannels;
using CheekyStork.ScriptableVariables;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    [SerializeField]
    private VoidEventChannelSO _launchRocket;

    [SerializeField]
    private Rocket _rocketPrefab;

    [SerializeField]
    private List<Transform> _launchSites;

    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private IntSO _rocketsLaunchedSO;

    
    [SerializeField] private bool _autoLaunch = true;
    private float _launchInterval = 0.1f; // 10 rockets per second
    private float _timer = 0f;
    
    void Awake()
    {
        _rocketsLaunchedSO.Value = 0;        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            _timer += Time.deltaTime;
            while (_timer >= _launchInterval)
            {
                TriggerEvent();
                _timer -= _launchInterval;
            }
        }
    }

    void OnEnable()
    {
        _launchRocket.OnEventRaised += LaunchRocket;        
    }

    void OnDisable()
    {
        _launchRocket.OnEventRaised -= LaunchRocket;        
    }

    [Button("Launch rocket")]
    void TriggerEvent()
    {
        _launchRocket.RaiseEvent();
    }
    
    void LaunchRocket()
    {
        Transform launchSite = _launchSites[UnityEngine.Random.Range(0, _launchSites.Count)];

        Rocket rocket = Instantiate(_rocketPrefab, launchSite.position, Quaternion.identity);
        rocket.Initialize(_speed);
        _rocketsLaunchedSO.Value +=1;
    }
}
