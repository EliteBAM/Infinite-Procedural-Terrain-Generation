using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;
using Unity.Mathematics;
using static Unity.Mathematics.math;

public static class ChunkMeshGenerationHelper
{
	public static void CalculateMeshData(BlockMap map)
    {
        NativeList<float3> positionStream = new NativeList<float3>(Allocator.TempJob);

        NativeList<float3> normalStream = new NativeList<float3>(Allocator.TempJob);

        NativeList<half2> texCoordStream = new NativeList<half2>(Allocator.TempJob);

        NativeList<TriangleUInt16> triangles = new NativeList<TriangleUInt16>(Allocator.TempJob);

        for (int x = 0; x < WorldSettings.CHUNK_SIZE.x; x++)
        {
            for(int y = 0; y < WorldSettings.CHUNK_SIZE.y; y++)
            {
                for(int z = 0; z < WorldSettings.CHUNK_SIZE.z; z++)
                {

                    if (map[x, y, z] == BlockType.AIR)
                        continue;

                    //left face
                    int neighborCoordinate = x - 1;
                    if(neighborCoordinate >= 0 
                        && map[neighborCoordinate, y, z] == BlockType.AIR)
                    {

                        AddVoxelData(float3(x * WorldSettings.VOXEL_SIZE, y * WorldSettings.VOXEL_SIZE, z * WorldSettings.VOXEL_SIZE), 4);
                    }
                    //right face
                    neighborCoordinate = x + 1;
                    if(neighborCoordinate < WorldSettings.CHUNK_SIZE.x 
                        && map[neighborCoordinate, y, z] == BlockType.AIR)
                    {

                        AddVoxelData(float3(x * WorldSettings.VOXEL_SIZE, y * WorldSettings.VOXEL_SIZE, z * WorldSettings.VOXEL_SIZE), 5);
                    }
                    //bottom face
                    neighborCoordinate = y - 1;
                    if (neighborCoordinate >= 0
                        && map[x, neighborCoordinate, z] == BlockType.AIR)
                    {

                        AddVoxelData(float3(x * WorldSettings.VOXEL_SIZE, y * WorldSettings.VOXEL_SIZE, z * WorldSettings.VOXEL_SIZE), 1);
                    }
                    //top face
                    neighborCoordinate = y + 1;
                    if (neighborCoordinate < WorldSettings.CHUNK_SIZE.y
                        && map[x, neighborCoordinate, z] == BlockType.AIR)
                    {

                        AddVoxelData(float3(x * WorldSettings.VOXEL_SIZE, y * WorldSettings.VOXEL_SIZE, z * WorldSettings.VOXEL_SIZE), 0);
                    }
                    //front face
                    neighborCoordinate = z - 1;
                    if (neighborCoordinate >= 0
                        && map[x, y, neighborCoordinate] == BlockType.AIR)
                    {

                        AddVoxelData(float3(x * WorldSettings.VOXEL_SIZE, y * WorldSettings.VOXEL_SIZE, z * WorldSettings.VOXEL_SIZE), 2);
                    }
                    //back face
                    neighborCoordinate = z + 1;
                    if (neighborCoordinate < WorldSettings.CHUNK_SIZE.z
                        && map[x, neighborCoordinate, z] == BlockType.AIR)
                    {

                        AddVoxelData(float3(x * WorldSettings.VOXEL_SIZE, y * WorldSettings.VOXEL_SIZE, z * WorldSettings.VOXEL_SIZE), 3);
                    }
                }
            }
        }

        void AddVoxelData(float3 positionOffset, int index)
        {
            //face verts
            positionStream.Add(VData.VOXEL_VERTS[VData.VOXEL_TRIS[index].Item1.x] + positionOffset);
            positionStream.Add(VData.VOXEL_VERTS[VData.VOXEL_TRIS[index].Item1.y] + positionOffset);
            positionStream.Add(VData.VOXEL_VERTS[VData.VOXEL_TRIS[index].Item1.z] + positionOffset);
            positionStream.Add(VData.VOXEL_VERTS[VData.VOXEL_TRIS[index].Item2.z] + positionOffset);

            //face normals
            normalStream.Add(VData.VOXEL_NORMALS[index]);
            normalStream.Add(VData.VOXEL_NORMALS[index]);
            normalStream.Add(VData.VOXEL_NORMALS[index]);
            normalStream.Add(VData.VOXEL_NORMALS[index]);

            //face Texture Coordinates
            half blockUnit = half(1f / 16f); //establishing half units (can't implicitly convert ints or floats )

            texCoordStream.Add(half2(half(blockUnit * half(0f)), half(blockUnit * half(15f))));
            texCoordStream.Add(half2(half(blockUnit * half(0f)), half(blockUnit * half(16f))));
            texCoordStream.Add(half2(half(blockUnit), half(blockUnit * half(15f))));
            texCoordStream.Add(half2(half(blockUnit), half(blockUnit * half(16f))));

            //add triangle indices
            triangles.Add(int3(VData.VOXEL_TRIS[index].Item1));
            triangles.Add(int3(VData.VOXEL_TRIS[index].Item2));

        }

    }

}
