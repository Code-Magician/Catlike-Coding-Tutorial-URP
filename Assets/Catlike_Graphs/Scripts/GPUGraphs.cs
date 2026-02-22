using Unity.VisualScripting;
using UnityEngine;

public class GPUGraphs : MonoBehaviour
{
    public enum TransitionMode { Cycle, Random}

    #region Inspector Fields

    [SerializeField]
    ComputeShader computeShader;

    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;

    [Space(10), SerializeField, Range(10, maxResolution)]
    int resolution = 10;

    [Space(10), SerializeField]
    FunctionLibrary.FunctionName currentFunctionName;

    [SerializeField]
    TransitionMode transitionMode;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    #endregion


    #region Private Fields

    const int maxResolution = 1000;

    float duration = 0;
    FunctionLibrary.FunctionName prevFunctionName;
    bool transitioning = false;

    static readonly int positionsID = Shader.PropertyToID("_Positions");
    static readonly int stepID = Shader.PropertyToID("_Step");
    static readonly int timeID = Shader.PropertyToID("_Time");
    static readonly int transitionProgressId = Shader.PropertyToID("_TransitionProgress");
    static readonly int resolutionID = Shader.PropertyToID("_Resolution");

    ComputeBuffer positionsBuffer;

    #endregion


    #region Mono Methods

    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }

    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
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

        UpdateGraphOnGPU();
    }

    #endregion


    #region Graph Drawing Functions

    private void PickNextFunction()
    {
        currentFunctionName = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextByFunctionName(currentFunctionName) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(currentFunctionName);
    }

    private void UpdateGraphOnGPU()
    {
        var kernelIndex = (int)currentFunctionName + 
            (int)(transitioning ? prevFunctionName : currentFunctionName) * 
            FunctionLibrary.FunctionCount;

        float step = 2f / resolution;

        computeShader.SetInt(resolutionID, resolution);
        computeShader.SetFloat(stepID, step);
        computeShader.SetFloat(timeID, Time.time);

        if (transitioning)
        {
            computeShader.SetFloat(
                transitionProgressId,
                Mathf.SmoothStep(0f, 1f, duration / transitionDuration)
            );
        }

        computeShader.SetBuffer(kernelIndex, positionsID, positionsBuffer);

        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);

        material.SetBuffer(positionsID, positionsBuffer);
        material.SetFloat(stepID, step);

        Bounds bounds = new Bounds(Vector3.zero, Vector3.one * (2f + step));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
    }

    #endregion
}
