using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

public static class WorldSettings
{
    //Voxel Units of Measurement
    public static readonly int VOXEL_SIZE = 1;

    public static readonly Vector3Int CHUNK_SIZE = new Vector3Int(16, 64, 16);

    //Chunk Units of Measurement
    public static readonly int CHUNK_WIDTH = VOXEL_SIZE * CHUNK_SIZE.x;

    public static readonly int CHUNK_DEPTH = VOXEL_SIZE * CHUNK_SIZE.z;

    public static readonly int CHUNK_HEIGHT = VOXEL_SIZE * CHUNK_SIZE.y;

    public static readonly int CHUNK_BLOCK_COUNT = CHUNK_SIZE.x * CHUNK_SIZE.y * CHUNK_SIZE.z;


    //Render Settings
    public static readonly int RENDER_DISTANCE = 16;


    //Generation Settings
    public static readonly int SEED = 1230979839;

    public static readonly int MIN_GROUND_HEIGHT = 16;

    public static readonly float PERLIN_AMPLITUDE = 30;

}
