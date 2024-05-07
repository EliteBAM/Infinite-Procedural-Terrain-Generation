using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;



[BurstCompile]
struct ChunkMeshJob<S> : IJob
    where S : struct, IMeshStreams 
{

    public BlockMap blockMap;

    [DeallocateOnJobCompletion]
    public NativeArray<float3> positionTable;
    [DeallocateOnJobCompletion]
    public NativeArray<float3> normalTable;
    [DeallocateOnJobCompletion]
    public NativeArray<(int3, int3)> triangleTable;
    [DeallocateOnJobCompletion]
    public NativeArray<int3> offsetTable;
    [DeallocateOnJobCompletion]
    public NativeArray<int4> texCoordTable;

    public Mesh.MeshData meshData;

    [WriteOnly]
    S stream;

    public void Execute()
    {
        //texcoord unit
        half blockUnit = half(1f / 16f); //establishing half units (can't implicitly convert ints or floats )

        int vertexCount = 0;
        int indexCount = 0;

        NativeList<Vertex> verticies = new NativeList<Vertex>(50, Allocator.Temp);

        NativeList<TriangleUInt16> triangles = new NativeList<TriangleUInt16>(50, Allocator.Temp);


        for (int x = 0; x < WorldSettings.CHUNK_SIZE.x; x++)
        {
            for (int y = 0; y < WorldSettings.CHUNK_SIZE.y; y++)
            {
                for (int z = 0; z < WorldSettings.CHUNK_SIZE.z; z++)
                {

                    if (blockMap[x, y, z] == BlockType.AIR)
                        continue;

                    for(int i = 0; i < offsetTable.Length; i++)
                    {

                        int texIndex = (((int)blockMap[x, y, z] - 1) * 6) + i;

                        if (x + offsetTable[i].x >= 0 && x + offsetTable[i].x < WorldSettings.CHUNK_SIZE.x
                            && y + offsetTable[i].y >= 0 && y + offsetTable[i].y < WorldSettings.CHUNK_SIZE.y
                            && z + offsetTable[i].z >= 0 && z + offsetTable[i].z < WorldSettings.CHUNK_SIZE.z)
                        {
                            if (blockMap[x + offsetTable[i].x, y + offsetTable[i].y, z + offsetTable[i].z] == BlockType.AIR)
                            {

                                float3 positionOffset = float3(x, y, z) * WorldSettings.VOXEL_SIZE;

                                //Add Voxel Mesh Data
                                verticies.Add(new Vertex()
                                {
                                    position = positionTable[triangleTable[i].Item1.x] + positionOffset,
                                    normal = normalTable[i],
                                    texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)), 
                                                      half(blockUnit * half(texCoordTable[texIndex].y)))
                                });

                                verticies.Add(new Vertex()
                                {
                                    position = positionTable[triangleTable[i].Item1.y] + positionOffset,
                                    normal = normalTable[i],
                                    texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)), 
                                                      half(blockUnit * half(texCoordTable[texIndex].w)))
                                });

                                verticies.Add(new Vertex()
                                {
                                    position = positionTable[triangleTable[i].Item1.z] + positionOffset,
                                    normal = normalTable[i],
                                    texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].z)), 
                                                      half(blockUnit * half(texCoordTable[texIndex].y)))
                                });

                                verticies.Add(new Vertex()
                                {
                                    position = positionTable[triangleTable[i].Item2.z] + positionOffset,
                                    normal = normalTable[i],
                                    texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].z)), 
                                                      half(blockUnit * half(texCoordTable[texIndex].w)))
                                });

                                //add face triangle indices (6) (2 triangles, 3 indices each)
                                triangles.Add(int3(triangleTable[0].Item1) + int3(vertexCount));
                                triangles.Add(int3(triangleTable[0].Item2) + int3(vertexCount));

                                vertexCount += 4;
                                indexCount += 6;

                            }
                        }
                        //left edge
                        if(x == 0)
                        {
                            float3 positionOffset = float3(x, y, z) * WorldSettings.VOXEL_SIZE;

                            //Add Voxel Mesh Data
                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[4].Item1.x] + positionOffset,
                                normal = normalTable[4],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].y)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[4].Item1.y] + positionOffset,
                                normal = normalTable[4],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].w)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[4].Item1.z] + positionOffset,
                                normal = normalTable[4],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].z)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].y)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[4].Item2.z] + positionOffset,
                                normal = normalTable[4],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].z)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].w)))
                            });

                            //add face triangle indices (6) (2 triangles, 3 indices each)
                            triangles.Add(int3(triangleTable[0].Item1) + int3(vertexCount));
                            triangles.Add(int3(triangleTable[0].Item2) + int3(vertexCount));

                            vertexCount += 4;
                            indexCount += 6;
                        }
                        //right edge
                        if (x == WorldSettings.CHUNK_SIZE.x - 1)
                        {
                            float3 positionOffset = float3(x, y, z) * WorldSettings.VOXEL_SIZE;

                            //Add Voxel Mesh Data
                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[5].Item1.x] + positionOffset,
                                normal = normalTable[5],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].y)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[5].Item1.y] + positionOffset,
                                normal = normalTable[5],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].w)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[5].Item1.z] + positionOffset,
                                normal = normalTable[5],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].z)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].y)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[5].Item2.z] + positionOffset,
                                normal = normalTable[5],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].z)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].w)))
                            });

                            //add face triangle indices (6) (2 triangles, 3 indices each)
                            triangles.Add(int3(triangleTable[0].Item1) + int3(vertexCount));
                            triangles.Add(int3(triangleTable[0].Item2) + int3(vertexCount));

                            vertexCount += 4;
                            indexCount += 6;
                        }
                        //top edge
                        if (y == WorldSettings.CHUNK_SIZE.y - 1)
                        {
                            float3 positionOffset = float3(x, y, z) * WorldSettings.VOXEL_SIZE;

                            //Add Voxel Mesh Data
                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[0].Item1.x] + positionOffset,
                                normal = normalTable[0],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].y)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[0].Item1.y] + positionOffset,
                                normal = normalTable[0],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].w)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[0].Item1.z] + positionOffset,
                                normal = normalTable[0],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].z)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].y)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[0].Item2.z] + positionOffset,
                                normal = normalTable[0],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].z)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].w)))
                            });

                            //add face triangle indices (6) (2 triangles, 3 indices each)
                            triangles.Add(int3(triangleTable[0].Item1) + int3(vertexCount));
                            triangles.Add(int3(triangleTable[0].Item2) + int3(vertexCount));

                            vertexCount += 4;
                            indexCount += 6;
                        }
                        //bottom edge
                        if (y == 0)
                        {
                            float3 positionOffset = float3(x, y, z) * WorldSettings.VOXEL_SIZE;

                            //Add Voxel Mesh Data
                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[1].Item1.x] + positionOffset,
                                normal = normalTable[1],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].y)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[1].Item1.y] + positionOffset,
                                normal = normalTable[1],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].w)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[1].Item1.z] + positionOffset,
                                normal = normalTable[1],
                                texCoord0 = half2(half(blockUnit * texCoordTable[texIndex].z), 
                                                  half(blockUnit * half(texCoordTable[texIndex].y)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[1].Item2.z] + positionOffset,
                                normal = normalTable[1],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].z)),
                                                  half(blockUnit * half(texCoordTable[texIndex].w)))
                            });

                            //add face triangle indices (6) (2 triangles, 3 indices each)
                            triangles.Add(int3(triangleTable[0].Item1) + int3(vertexCount));
                            triangles.Add(int3(triangleTable[0].Item2) + int3(vertexCount));

                            vertexCount += 4;
                            indexCount += 6;
                        }
                        //front edge
                        if (z == 0)
                        {
                            float3 positionOffset = float3(x, y, z) * WorldSettings.VOXEL_SIZE;

                            //Add Voxel Mesh Data
                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[2].Item1.x] + positionOffset,
                                normal = normalTable[2],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].y)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[2].Item1.y] + positionOffset,
                                normal = normalTable[2],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)),
                                                  half(blockUnit * half(texCoordTable[texIndex].w)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[2].Item1.z] + positionOffset,
                                normal = normalTable[2],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].z)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].y)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[2].Item2.z] + positionOffset,
                                normal = normalTable[2],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].z)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].w)))
                            });

                            //add face triangle indices (6) (2 triangles, 3 indices each)
                            triangles.Add(int3(triangleTable[0].Item1) + int3(vertexCount));
                            triangles.Add(int3(triangleTable[0].Item2) + int3(vertexCount));

                            vertexCount += 4;
                            indexCount += 6;
                        }
                        //back edge
                        if (z == WorldSettings.CHUNK_SIZE.z - 1)
                        {
                            float3 positionOffset = float3(x, y, z) * WorldSettings.VOXEL_SIZE;

                            //Add Voxel Mesh Data
                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[3].Item1.x] + positionOffset,
                                normal = normalTable[3],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].y)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[3].Item1.y] + positionOffset,
                                normal = normalTable[3],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].x)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].w)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[3].Item1.z] + positionOffset,
                                normal = normalTable[3],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].z)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].y)))
                            });

                            verticies.Add(new Vertex()
                            {
                                position = positionTable[triangleTable[3].Item2.z] + positionOffset,
                                normal = normalTable[3],
                                texCoord0 = half2(half(blockUnit * half(texCoordTable[texIndex].z)), 
                                                  half(blockUnit * half(texCoordTable[texIndex].w)))
                            });

                            //add face triangle indices (6) (2 triangles, 3 indices each)
                            triangles.Add(int3(triangleTable[0].Item1) + int3(vertexCount));
                            triangles.Add(int3(triangleTable[0].Item2) + int3(vertexCount));

                            vertexCount += 4;
                            indexCount += 6;
                        }
                    }
                }
            }
        }

        Bounds bounds = new Bounds(WorldSettings.CHUNK_SIZE * WorldSettings.VOXEL_SIZE / 2, WorldSettings.CHUNK_SIZE * WorldSettings.VOXEL_SIZE);
        stream.Setup(meshData, bounds, vertexCount, indexCount);

        for(int i = 0; i < vertexCount; i++)
        {
            stream.SetVertex(i, verticies[i]);
        }

        for(int  i = 0; i < indexCount/3; i++)
        {
            stream.SetTriangle(i, triangles[i]);
        }
       
    }

    public static JobHandle ScheduleJob(ref BlockMap blockMap,
                                        float3[] positionTable,
                                        (int3, int3)[] triangleTable,
                                        float3[] normalTable,
                                        int3[] offsetTable,
                                        int4[] texCoordTable,
                                        Mesh mesh, Mesh.MeshData meshData, JobHandle depenency)
    {
        var job = new ChunkMeshJob<MultiStream>()
        {

            blockMap = blockMap,

            //Copy managed voxel Lookup Tables into unmanaged memory for job access and burst compiler compliance.. so fucked
            positionTable = new NativeArray<float3>(positionTable, Allocator.TempJob),
            normalTable = new NativeArray<float3>(normalTable, Allocator.TempJob),
            triangleTable = new NativeArray<(int3, int3)>(triangleTable, Allocator.TempJob),
            offsetTable = new NativeArray<int3>(offsetTable, Allocator.TempJob),
            texCoordTable = new NativeArray<int4>(texCoordTable, Allocator.TempJob),

            meshData = meshData
        };

        return job.Schedule(depenency);
    }
}