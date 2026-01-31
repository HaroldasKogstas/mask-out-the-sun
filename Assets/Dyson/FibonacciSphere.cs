using System.Collections.Generic;
using UnityEngine;

public static class FibonacciSphere
{
    /// <summary>
    /// Get evenly distributed points on a sphere.
    /// </summary>
    /// <param name="numPoints"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static List<Vector3> GetSpherePoints(int numPoints = 50, float radius = 1f)
    {
        var points = new List<Vector3>();

        // Fibonacci sphere algorithm for evenly distributed points
        float goldenRatio = (1f + Mathf.Sqrt(5f)) / 2f;
        float angleIncrement = Mathf.PI * 2f * goldenRatio;

        for (int i = 0; i < numPoints; i++)
        {
            float t = (float)i / numPoints;
            float inclination = Mathf.Acos(1f - 2f * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);

            Vector3 point = new Vector3(x, y, z) * radius;
            points.Add(point);
        }

        return points;
    }
}
