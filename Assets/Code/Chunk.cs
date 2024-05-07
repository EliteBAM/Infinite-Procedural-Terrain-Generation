using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Jobs;
using Unity.Collections;
using static Unity.Mathematics.math;
using Unity.Mathematics;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{

    private Mesh _mesh;
    private MeshCollider _collider;

    private BlockMap blockMap;

    public Vector2 offset = new Vector2(0f, 0f);

    //mesh gen stuff
    public Mesh.MeshDataArray meshDataArray;
    public Mesh.MeshData meshData;


    private void Start()
    {
        _mesh = new Mesh
        {
            name = "Procedural Mesh"
        };

        _collider = gameObject.AddComponent<MeshCollider>();

        GetComponent<MeshFilter>().mesh = _mesh;

        offset = new Vector2(transform.position.x, transform.position.z);

        ScheduleMesh().Complete();
        FinalizeMesh();


        GetComponent<MeshFilter>().mesh = _mesh;

    }

    public void LoadMesh()
    {
        _mesh.Clear();

        blockMap = PerlinNoise.GenerateBlockMap(PerlinNoise.GenerateNoiseArea(offset));

        meshDataArray = Mesh.AllocateWritableMeshData(1);
        meshData = meshDataArray[0];

        //float time = Time.realtimeSinceStartup;
        //call job
        ChunkMeshJob<MultiStream>.ScheduleJob(ref blockMap,
                                              VData.VOXEL_VERTS,
                                              VData.VOXEL_TRIS,
                                              VData.VOXEL_NORMALS,
                                              VData.VOXEL_NEIGHBOR_OFFSETS,
                                              VData.TEXTURE_COORDS,
                                              _mesh, meshData, default).Complete();
        //Debug.Log("mesh job: " + (Time.realtimeSinceStartup - time) * 1000 + " ms");

        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, _mesh);
        //meshDataArray.Dispose();

        _mesh.RecalculateBounds();

        _collider.sharedMesh = _mesh;
    }

    public JobHandle ScheduleMesh()
    {
        _mesh.Clear();

        blockMap = PerlinNoise.GenerateBlockMap(PerlinNoise.GenerateNoiseArea(offset));

        meshDataArray = Mesh.AllocateWritableMeshData(1);
        meshData = meshDataArray[0];

        //call job
        return ChunkMeshJob<MultiStream>.ScheduleJob(ref blockMap,
                                              VData.VOXEL_VERTS,
                                              VData.VOXEL_TRIS,
                                              VData.VOXEL_NORMALS,
                                              VData.VOXEL_NEIGHBOR_OFFSETS,
                                              VData.TEXTURE_COORDS,
                                              _mesh, meshData, default);
    }

    public void FinalizeMesh()
    {
        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, _mesh);
        //meshDataArray.Dispose();

        _mesh.RecalculateBounds();

        _collider.sharedMesh = _mesh;
    }

}
