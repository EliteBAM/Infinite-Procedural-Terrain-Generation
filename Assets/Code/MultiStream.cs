using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst;
using Unity.Mathematics;

public struct MultiStream : IMeshStreams
{

    public static NativeArray<VertexAttributeDescriptor> descriptors;

    [NativeDisableContainerSafetyRestriction]
    NativeArray<float3> stream0, stream1;

    [NativeDisableContainerSafetyRestriction]
    NativeArray<float2> stream2;

    [NativeDisableContainerSafetyRestriction]
    NativeArray<TriangleUInt16> triangles;

    public static void DefineDescriptors()
    {
        descriptors = new NativeArray<VertexAttributeDescriptor>(3, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        //one-time establish the data stream format for all chunk mesh generation, rather than doing it multiple times
        descriptors[0] = new VertexAttributeDescriptor(dimension: 3); //default attribute is position, default stream is zero
        descriptors[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3, stream: 1);
        descriptors[2] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2, stream: 2);
    }

    public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount)
    {
        //descriptor setup

        var descriptors = new NativeArray<VertexAttributeDescriptor>(3, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

        descriptors[0] = new VertexAttributeDescriptor(dimension: 3); //default attribute is position, default stream is zero
        descriptors[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3, stream: 1);
        descriptors[2] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2, stream: 2);

        //descriptor setup

        meshData.SetVertexBufferParams(vertexCount, descriptors);
        descriptors.Dispose();

        meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt16);

        meshData.subMeshCount = 1;

        meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount)
            {
                bounds = bounds,
                vertexCount = vertexCount
            },
            MeshUpdateFlags.DontRecalculateBounds |
            MeshUpdateFlags.DontValidateIndices
        );

        stream0 = meshData.GetVertexData<float3>();
        stream1 = meshData.GetVertexData<float3>(1);
        stream2 = meshData.GetVertexData<float2>(2);

        triangles = meshData.GetIndexData<ushort>().Reinterpret<TriangleUInt16>(2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetVertex(int index, Vertex vertex)
    {
        stream0[index] = vertex.position;
        stream1[index] = vertex.normal;
        stream2[index] = vertex.texCoord0;
    }


    public void SetTriangle(int index, TriangleUInt16 triangle) => triangles[index] = triangle;
}
