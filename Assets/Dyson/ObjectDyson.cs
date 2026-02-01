using System.Collections.Generic;
using System.Linq;
using CheekyStork.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectDyson : DysonGenerator
{
    [SerializeField]
    DysonPart _objectToSpawn;

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

    [SerializeField]
    private IntSO _dysonSpherePartsDeployed;

    private int _currentTargetIndex;

    List<DysonPart> _spawnedObjects = new();
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

            spawnedObject.Initialize(size);

            if (_orbitObjects)
            {
                var orbit = spawnedObject.gameObject.AddComponent<OrbitObject>();
                float randomSpeed = Random.Range(_orbitSpeedMin, _orbitSpeedMax);
                // Use a perpendicular axis for proper orbital motion around the sun
                Vector3 orbitAxis = Vector3.Cross(point.normalized, Random.onUnitSphere).normalized;
                orbit.Initialize(randomSpeed, orbitAxis, transform);
            }
            
            _spawnedObjects.Add(spawnedObject);
        }

        _currentTargetIndex = Step;
        
        UpdateObjectsVisibility();
        
        if (_currentTargetIndex < _spawnOrder.Count)
        {
            PrepareNextDysonPartTransform();
        }
    }

    public override void Regenerate()
    {
        UpdateObjectsVisibility();
    }

    void UpdateObjectsVisibility()
    {
        // Activate objects based on their position in spawn order
        for (int i = 0; i < _spawnedObjects.Count; i++)
        {
            if (_spawnedObjects[i] != null)
            {
                // Find if this object index appears in the first Step positions of spawn order
                int positionInSpawnOrder = _spawnOrder.IndexOf(i);
                _spawnedObjects[i].gameObject.SetActive(positionInSpawnOrder < Step && positionInSpawnOrder >= 0);
            }
        }

        _dysonSpherePartsDeployed.Value = Step;
    }

    protected override void OnDysonPartJourneyStart()
    {
        // Rocket has read current target and launched
        // Now prepare the NEXT target for the NEXT rocket
        _currentTargetIndex++;
        if (_currentTargetIndex < _spawnOrder.Count)
        {
            PrepareNextDysonPartTransform();
        }
    }
    
    private void PrepareNextDysonPartTransform()
    {
        // Validate index is within bounds
        if (_currentTargetIndex < 0 || _currentTargetIndex >= _spawnOrder.Count)
        {
            Debug.LogWarning($"Target index {_currentTargetIndex} out of range (0-{_spawnOrder.Count - 1})");
            return;
        }

        // Get actual spawned object index from spawn order
        int spawnedObjectIndex = _spawnOrder[_currentTargetIndex];
        
        if (spawnedObjectIndex < 0 || spawnedObjectIndex >= _spawnedObjects.Count)
        {
            Debug.LogWarning($"Spawned object index {spawnedObjectIndex} out of range (0-{_spawnedObjects.Count - 1})");
            return;
        }

        DysonPart targetPart = _spawnedObjects[spawnedObjectIndex];
        if (targetPart != null)
        {
            UpdateNextDysonPartTransform(targetPart.transform);
        }
        else
        {
            Debug.LogWarning($"Target DysonPart at index {spawnedObjectIndex} is null");
        }
    }
}
