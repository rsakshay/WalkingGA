using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    public DistanceJoint2D spring;

    private float contracted;
    private float relaxed;

    [Range(-1, +1)]
    public float position = +1;

    void Start()
    {
        float distance = spring.distance;
        relaxed = distance * 1.5f;
        contracted = distance / 2f;
    }

    void FixedUpdate()
    {
        spring.distance = linearInterpolation(-1, +1, contracted, relaxed, position);
    }

    public static float linearInterpolation(float x0, float x1, float y0, float y1, float x)
    {
        float d = x1 - x0;
        if (d == 0)
            return (y0 + y1) / 2;
        return y0 + (x - x0) * (y1 - y0) / d;
    }
}
