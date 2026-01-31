using System.Collections.Generic;
using UnityEngine;

public class VoronoiDyson : DysonGenerator
{
    List<Vector3> _points = new();

    void Awake()
    {
        Regenerate();
    }

    public override void Regenerate()
    {
        FibonacciSphere.GetSpherePoints(NumPoints, Radius);
    }

    void OnDrawGizmos()
    {
        foreach(var point in _points)
        {
            Gizmos.DrawSphere(point, 0.1f);
        }
    }

    protected override void OnDysonPartJourneyStart()
    {
        throw new System.NotImplementedException();
    }
}
