using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using static Unity.Mathematics.math;
using Unity.Mathematics;


//OLD, NON-FUNCTIONAL, SHOULD PROBABLY DELETE, BUT MAY CONTAIN FRAGMENTS OF USEFUL THOUGHTS FOR THE FUTURE

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class OldChunk : MonoBehaviour
{

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Mesh _mesh;

    //mesh data
    private int _vertexCount = 4;
    private int _vertexAttributeCount = 3;

    private int _indexCount = 6;


    private void Start()
    {
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        _meshRenderer.material = Resources.Load("TerrainMat", typeof(Material)) as Material;

        InitializeMesh();

        _meshFilter.mesh = _mesh;

    }

    public void InitializeMesh()
    {
        //Allocate memory for mesh data. returns mesh data array in case of mesh batching. 
        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1); //parameter takes in # of meshes. In this case only one mesh.
        Mesh.MeshData meshData = meshDataArray[0];



        //specify vertex count and vertex data layout (in this case, only position and normal data are stored on the vertex)
        var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(_vertexAttributeCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

        vertexAttributes[0] = new VertexAttributeDescriptor(dimension: 3); //defines struct for position data. 3 dimensions: x, y, z; declaration uses 'named arguments' format
        vertexAttributes[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3, stream: 1);
        vertexAttributes[2] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, dimension: 2, stream: 2);

        meshData.SetVertexBufferParams(_vertexCount, vertexAttributes);
        vertexAttributes.Dispose(); //necessary because of the use of NativeArray. Likely deletes memory allocation in unmanaged code.

        //set vertex data for chunk on a per-attribute-stream basis
        NativeArray<float3> vertPositions = meshData.GetVertexData<float3>(); //stream 0: positions
        //top face
        vertPositions[0] = float3(0f, WorldSettings.VOXEL_SIZE, 0f);
        vertPositions[1] = float3(0f, WorldSettings.VOXEL_SIZE, WorldSettings.VOXEL_SIZE);
        vertPositions[2] = float3(WorldSettings.VOXEL_SIZE, WorldSettings.VOXEL_SIZE, 0f);
        vertPositions[3] = float3(WorldSettings.VOXEL_SIZE, WorldSettings.VOXEL_SIZE, WorldSettings.VOXEL_SIZE);


        //using half data type for Float16 formatted attributes
        half h0 = half(0f), h1 = half(1f), blockUnit = half(1f / 16f); //establishing half units (can't implicitly convert ints or floats )

        NativeArray<float3> vertNormals = meshData.GetVertexData<float3>(1); //stream 1: normals
        //top face
        vertNormals[0] = vertNormals[1] = vertNormals[2] = vertNormals[3] = up();


        NativeArray<half2> vertTexCoords = meshData.GetVertexData<half2>(2); //stream 2: texCoords
        //top face
        vertTexCoords[0] = half2(half(blockUnit * half(0f)), half(blockUnit * half(15f)));
        vertTexCoords[1] = half2(half(blockUnit * half(0f)), half(blockUnit * half(16f)));
        vertTexCoords[2] = half2(half(blockUnit), half(blockUnit * half(15f)));
        vertTexCoords[3] = half2(half(blockUnit), half(blockUnit * half(16f)));



        //establish index buffer.. whatever that is
        meshData.SetIndexBufferParams(_indexCount, IndexFormat.UInt16);

        //set index data for chunk
        NativeArray<ushort> indices = meshData.GetIndexData<ushort>();
        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2;
        indices[3] = 2;
        indices[4] = 1;
        indices[5] = 3;

        //Define submesh bounds manually so that Unity doesn't waste resources calculating them automatically
        var bounds = new Bounds(new Vector3(0.5f, 0.5f), new Vector3(1f, 1f));

        meshData.subMeshCount = 1;
        meshData.SetSubMesh(0, new SubMeshDescriptor(0, _indexCount)
        {
            bounds = bounds,
            vertexCount = _vertexCount
        }, MeshUpdateFlags.DontRecalculateBounds);


        //define mesh object
        _mesh = new Mesh { name = "procedural mesh", bounds = bounds };
        //apply meshdata to the new mesh
        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, _mesh);

    }

}
