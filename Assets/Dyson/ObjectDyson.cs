using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
            point = transform.TransformPoint(point);
            var spawnedObject = Instantiate(_objectToSpawn, point, Quaternion.identity, transform);

            spawnedObject.transform.up = (point - transform.position).normalized;

            // Set size (randomized if enabled)
            float size = _randomizeSize ? Random.Range(_minSize, _maxSize) : _objectSize;
            spawnedObject.transform.localScale = Vector3.one * size;

            if (_orbitObjects)
            {
                var orbit = spawnedObject.AddComponent<OrbitObject>();
                float randomSpeed = Random.Range(_orbitSpeedMin, _orbitSpeedMax);
                // Use a perpendicular axis for proper orbital motion around the sun
                Vector3 orbitAxis = Vector3.Cross(point.normalized, Random.onUnitSphere).normalized;
                orbit.Initialize(randomSpeed, orbitAxis, transform);
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
