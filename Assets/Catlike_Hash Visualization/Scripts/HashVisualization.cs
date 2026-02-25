using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;


public class HashVisualization : MonoBehaviour
{
    static int hashesId = Shader.PropertyToID("_Hashes");
    static int configId = Shader.PropertyToID("_Config");

    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    struct HashJob : IJobFor
    {
        public int resolution;
        public float invResolution;
        public NativeArray<uint> hashes;
        public SmallXXHash hash;

        public void Execute(int i)
        {
            int v = (int)floor(i * invResolution + 0.00001f);
            int u = i - resolution * v;

            hashes[i] = hash.Eat(u).Eat(v);
        }
    }

    [SerializeField, Range(4, 512)]
    int resolution = 32;

    [SerializeField]
    int seed;

    [SerializeField, Range(-2f, 2f)]
    float verticalOffset = 1f;

    [SerializeField]
    Mesh mesh;

    [SerializeField]
    Material material;

    ComputeBuffer hashBuffer;
    NativeArray<uint> hashes;
    MaterialPropertyBlock propertyBlock;


    private void OnEnable()
    {
        int length = resolution * resolution;
        hashes = new NativeArray<uint>(length, Allocator.Persistent);
        hashBuffer = new ComputeBuffer(length, 4);

        new HashJob
        {
            resolution = resolution,
            invResolution = 1f / resolution,
            hashes = hashes,
            hash = SmallXXHash.Seed(seed),
        }.ScheduleParallel(length, resolution, default).Complete();

        hashBuffer.SetData(hashes);

        propertyBlock ??= new MaterialPropertyBlock();
        propertyBlock.SetBuffer(hashesId, hashBuffer);
        propertyBlock.SetVector(configId, new Vector4(resolution, 1f / resolution, verticalOffset / resolution));
    }

    private void OnDisable()
    {
        hashes.Dispose();
        hashBuffer.Release();

        hashBuffer = null;
    }

    private void OnValidate()
    {
        if (hashBuffer != null && enabled)
        {
            OnDisable();
            OnEnable();
        }
    }

    private void Update()
    {
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, new Bounds(Vector3.zero, Vector3.one), hashes.Length, propertyBlock);
    }
}
