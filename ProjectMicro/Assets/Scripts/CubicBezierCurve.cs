using UnityEngine;

public class CubicBezierCurve
{
    public Vector3[] controlPoints;

    public CubicBezierCurve(Vector3[] controlPoints)
    {

        this.controlPoints = controlPoints;
    }

    public Vector3 GetPosition(float t)
    {
        float t2 = t * t;
        float t3 = t * t2;
        float m = (1 - t);
        float m2 = m * m;
        float m3 = m * m2;

        Vector3 position = new Vector3
        {
            x = (m3 * controlPoints[0].x) +
                     (3 * m2 * t * controlPoints[1].x) +
                     (3 * m * t2 * controlPoints[2].x) +
                     (t3 * controlPoints[3].x),

            y = (m3 * controlPoints[0].y) +
                     (3 * m2 * t * controlPoints[1].y) +
                     (3 * m * t2 * controlPoints[2].y) +
                     (t3 * controlPoints[3].y),

            z = (m3 * controlPoints[0].z) +
                     (3 * m2 * t * controlPoints[1].z) +
                     (3 * m * t2 * controlPoints[2].z) +
                     (t3 * controlPoints[3].z)
        };

        return position;
    }
}