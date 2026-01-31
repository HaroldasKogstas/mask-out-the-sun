using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectDyson : DysonGenerator
{
    [SerializeField]
    GameObject _objectToSpawn;

    [SerializeField]
    float _objectSize = 0.1f;

    [SerializeField]
    bool _orbitObjects;

    [ShowIf("_orbitObjects")]
    [SerializeField]
    float _orbitSpeedMin = 5f;

    [ShowIf("_orbitObjects")]
    [SerializeField]
    float _orbitSpeedMax = 15f;

    [SerializeField]
    bool _randomizeSize;

    [ShowIf("_randomizeSize")]
    [SerializeField]
    float _minSize = 0.05f;

    [ShowIf("_randomizeSize")]
    [SerializeField]
    float _maxSize = 0.15f;

    List<GameObject> _spawnedObjects = new();
    List<int> _spawnOrder = new();

    void Start()
    {
        GenerateAllObjects();
    }

    [Button("Initialize new object")]
    void GenerateAllObjects()
    {
        foreach (var obj in _spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        _spawnedObjects.Clear();

        var points = FibonacciSphere.GetSpherePoints(NumPoints, Radius);

        _spawnOrder = Enumerable.Range(0, points.Count).OrderBy(x => Random.value).ToList();

        foreach (var index in _spawnOrder)
        {
            var point = points[index];
            var spawnedObject = Instantiate(_objectToSpawn, point, Quaternion.LookRotation(point.normalized), transform);

            spawnedObject.transform.up = point.normalized;

            // Set size (randomized if enabled)
            float size = _randomizeSize ? Random.Range(_minSize, _maxSize) : _objectSize;
            spawnedObject.transform.localScale = Vector3.one * size;

            // Add orbit component if enabled
            if (_orbitObjects)
            {
                var orbit = spawnedObject.AddComponent<OrbitObject>();
                float randomSpeed = Random.Range(_orbitSpeedMin, _orbitSpeedMax);
                orbit.Initialize(randomSpeed, point.normalized, transform);
            }
            
            _spawnedObjects.Add(spawnedObject);
        }

        UpdateObjectsVisibility();
    }

    public override void Regenerate()
    {
        UpdateObjectsVisibility();
    }

    void UpdateObjectsVisibility()
    {
        for (int i = 0; i < _spawnedObjects.Count; i++)
        {
            if (_spawnedObjects[i] != null)
            {
                _spawnedObjects[i].SetActive(i < Step);
            }
        }
    }
}
