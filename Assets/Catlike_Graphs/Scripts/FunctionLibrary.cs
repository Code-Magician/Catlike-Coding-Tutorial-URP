using UnityEngine;
using static UnityEngine.Mathf;

public static class FunctionLibrary 
{
    public delegate Vector3 Function(float u, float v, float t);

    public enum FunctionName { Wave, MultiWave, Ripple, Sphere, Torus};

    static Function[] functions = { Wave, MultiWave, Ripple, Sphere, Torus};


    #region Getters

    public static int FunctionCount => functions.Length;

    public static FunctionName GetRandomFunctionNameOtherThan(FunctionName name)
    {
        var choice = (FunctionName)Random.Range(1, functions.Length);
        return (choice == name ? 0 : choice);
    }

    public static FunctionName GetNextByFunctionName(FunctionName name) => (int)name < functions.Length - 1 ? name + 1 : 0;
    public static Function GetFunction(FunctionName name) => functions[(int)name];

    #endregion


    #region Graph Functions

    public static Vector3 Morph(float u, float v, float t, Function from, Function to, float progress)
    {
        return Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), SmoothStep(0, 1, progress));
    }

    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;

        return p;
    }

    public static Vector3 MultiWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += 0.5f * Sin(2 * PI * (v + t));
        p.y += 0.5f * Sin(PI * (u + v + 0.25f * t));
        p.y *= 0.4f;
        p.z = v;

        return p;
    }

    public static Vector3 Ripple(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        float d = Sqrt(u*u + v*v);
        p.y = Sin(PI * (4f * d - t));
        p.y /= (1 + 10 * d);
        p.z = v;

        return p;
    }

    public static Vector3 Sphere(float u, float v, float t)
    {
        Vector3 p;
        float r = 0.9f + 0.1f * Sin(PI * (12 * u + 8 * v + t));      // Bands in both the direction + movement with time
        //float r = 0.9f + 0.1f * Sin(8f * PI * v);                 // Vertical Bands
        //float r = 0.9f + 0.1f * Sin(8f * PI * u);                 // Horizontal Bands
        //float r = 0.5f + 0.5f * Sin(PI * t);
        float s = r * Cos(0.5f * PI * v);
        p.x = s * Sin(PI * (u));
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * (u));

        return p;
    }

    public static Vector3 Torus(float u, float v, float t)
    {
        Vector3 p;
        float r1 = 0.7f + 0.1f * Sin(PI * (8*u + 0.5f*t));
        float r2 = 0.15f + 0.05f * Sin(PI *(16*u + 8*v + 3*t));
        float s = r1 + r2 * Cos(PI * v);
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);

        return p;
    }

    #endregion
}
