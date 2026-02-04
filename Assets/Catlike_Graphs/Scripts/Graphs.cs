using Unity.VisualScripting;
using UnityEngine;

public class Graphs : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;

    [SerializeField, Range(10, 200)]
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName functionName;

    Transform[] points;

    private void Awake()
    {
        DrawGraph();
    }

    private void Update()
    {
        AnimateGraph();
    }


    private void DrawGraph()
    {
        points = new Transform[resolution * resolution];

        var step = 2f / resolution;
        var scale = Vector3.one * step;
        for(int i=0; i<points.Length; i++)
        {
            Transform pointTr = points[i] = Instantiate(pointPrefab, transform);

            pointTr.localScale = scale;
        }
    }

    private void AnimateGraph()
    {
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(functionName);
        float time = Time.time;
        var step = 2f / resolution;
        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z++;
                v = ((z + 0.5f) * step - 1f);
            }

            float u = ((x + 0.5f) * step - 1f);

            points[i].localPosition = f(u, v, time);
        }
    }
}
