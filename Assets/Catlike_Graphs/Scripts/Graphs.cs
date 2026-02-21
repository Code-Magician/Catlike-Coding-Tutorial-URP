using Unity.VisualScripting;
using UnityEngine;

public class Graphs : MonoBehaviour
{
    public enum TransitionMode { Cycle, Random}

    #region Inspector Fields

    [SerializeField]
    Transform pointPrefab;

    [Space(10), SerializeField, Range(10, 200)]
    int resolution = 10;

    [Space(10), SerializeField]
    FunctionLibrary.FunctionName currentFunctionName;

    [SerializeField]
    TransitionMode transitionMode;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    #endregion


    #region Private Fields

    Transform[] points;
    float duration = 0;
    FunctionLibrary.FunctionName prevFunctionName;
    bool transitioning = false;

    #endregion


    #region Mono Methods

    private void Awake()
    {
        DrawGraph();
    }

    private void Update()
    {
        duration += Time.deltaTime;

        if(transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration = 0;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;

            transitioning = true;

            prevFunctionName = currentFunctionName;
            PickNextFunction();
        }

        if(transitioning)   UpdateGraphTransition();
        else                UpdateGraph();
    }

    #endregion


    #region Graph Drawing Functions

    private void PickNextFunction()
    {
        currentFunctionName = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextByFunctionName(currentFunctionName) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(currentFunctionName);
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

        Debug.Log($"Points: {points.Length}");
    }

    private void UpdateGraph()
    {
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(currentFunctionName);
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

    private void UpdateGraphTransition()
    {
        FunctionLibrary.Function 
            from = FunctionLibrary.GetFunction(prevFunctionName),
            to = FunctionLibrary.GetFunction(currentFunctionName);

        float progress = duration / transitionDuration;

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

            points[i].localPosition = FunctionLibrary.Morph(u, v, time, from, to, progress);
        }
    }

    #endregion
}
