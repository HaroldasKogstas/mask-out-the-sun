using System;
using System.Collections.Generic;
using CheekyStork.ScriptableChannels;
using Sirenix.OdinInspector;
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
    }
}
