using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public interface IMeshStreams
{
    void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount);

    void SetVertex(int index, Vertex vertex);

    void SetTriangle(int index, TriangleUInt16 triangle);
}
