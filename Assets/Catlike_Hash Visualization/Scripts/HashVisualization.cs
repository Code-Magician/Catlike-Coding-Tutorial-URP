using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;


public readonly struct SmallXXHash
{

    const uint primeA = 0b10011110001101110111100110110001;
    const uint primeB = 0b10000101111010111100101001110111;
    const uint primeC = 0b11000010101100101010111000111101;
    const uint primeD = 0b00100111110101001110101100101111;
    const uint primeE = 0b00010110010101100110011110110001;

    readonly uint accumulator;

    public SmallXXHash(uint accumulator)
    {
        this.accumulator = accumulator;
    }

    public static implicit operator SmallXXHash(uint accumulator) =>
        new SmallXXHash(accumulator);

    public static SmallXXHash Seed(int seed) => (uint)seed + primeE;

    static uint RotateLeft(uint data, int steps) =>
        (data << steps) | (data >> 32 - steps);

    public SmallXXHash Eat(int data) =>
        RotateLeft(accumulator + (uint)data * primeC, 17) * primeD;

    public SmallXXHash Eat(byte data) =>
        RotateLeft(accumulator + data * primeE, 11) * primeA;

    public static implicit operator uint(SmallXXHash hash)
    {
        uint avalanche = hash.accumulator;
        avalanche ^= avalanche >> 15;
        avalanche *= primeB;
        avalanche ^= avalanche >> 13;
        avalanche *= primeC;
        avalanche ^= avalanche >> 16;
        return avalanche;
    }
}

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
