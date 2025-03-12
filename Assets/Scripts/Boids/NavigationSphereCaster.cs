using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Calculates a certain number of rays at evenly distributed intervals along a sphere
 * Used for boid/swimming navigation
 * 
 * Jeff Stevenson
 * 3.12.25
 */

public class NavigationSphereCaster
{
    public static Vector3[] GetNavigationSphereVectors(int numVectors)
    {
        Vector3[] navigationVectors = new Vector3[numVectors];

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrementValue = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numVectors; i++)
        {
            float t = (float)i / numVectors;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrementValue * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);
            navigationVectors[i] = new Vector3(x, y, z);
        }

        return navigationVectors;
    }
}
