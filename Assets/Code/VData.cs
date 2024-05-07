using Unity.Mathematics;
using static Unity.Mathematics.math;

/// <summary>
/// 
/// VOXEL DATA
/// 
/// </summary>
public static class VData
{

    public static readonly int3[] VOXEL_NEIGHBOR_OFFSETS = new int3[6]
    {
        int3( 0,  1,  0), //up
        int3( 0, -1,  0), //down
        int3( 0,  0, -1), //back
        int3( 0,  0,  1), //front
        int3(-1,  0,  0), //left
        int3( 1,  0,  0), //right
    };

    public static readonly int4[] TEXTURE_COORDS = new int4[24]
    {
        //grass block
        int4(0, 15, 1, 16), //up
        int4( 2, 15,  3, 16), //down
        int4( 3,  15, 4, 16), //back
        int4( 3,  15, 4, 16), //front
        int4( 3,  15, 4, 16), //left
        int4( 3,  15, 4, 16), //right
        //dirt block
        int4( 2, 15,  3, 16), //up
        int4( 2, 15,  3, 16), //down
        int4( 2, 15,  3, 16), //back
        int4( 2, 15,  3, 16), //front
        int4( 2, 15,  3, 16), //left
        int4( 2, 15,  3, 16), //right
        //stone block
        int4( 1, 15,  2, 16), //up
        int4( 1, 15,  2, 16), //down
        int4( 1, 15,  2, 16), //back
        int4( 1, 15,  2, 16), //front
        int4( 1, 15,  2, 16), //left
        int4( 1, 15,  2, 16), //right
        //bedrock block
        int4( 1, 14,  2, 15), //up
        int4( 1, 14,  2, 15), //down
        int4( 1, 14,  2, 15), //back
        int4( 1, 14,  2, 15), //front
        int4( 1, 14,  2, 15), //left
        int4( 1, 14,  2, 15), //right
    };

    public static readonly float3[] VOXEL_VERTS = new float3[8]
    {
      /*0*/  float3(0, 1, 0) * WorldSettings.VOXEL_SIZE,
      /*1*/  float3(0, 1, 1) * WorldSettings.VOXEL_SIZE,
      /*2*/  float3(1, 1, 0) * WorldSettings.VOXEL_SIZE,
      /*3*/  float3(1, 1, 1) * WorldSettings.VOXEL_SIZE, //top verts ^
      /*4*/  float3(0, 0, 0),
      /*5*/  float3(0, 0, 1) * WorldSettings.VOXEL_SIZE,
      /*6*/  float3(1, 0, 0) * WorldSettings.VOXEL_SIZE,
      /*7*/  float3(1, 0, 1) * WorldSettings.VOXEL_SIZE //bottom verts ^
    };

    public static readonly (int3, int3)[] VOXEL_TRIS = new (int3, int3)[6]
    {
        new (int3(0, 1, 2), int3(2, 1, 3)) , //top face ^
        new (int3(5, 4, 7), int3(7, 4, 6)) , //bottom face ^
        new (int3(4, 0, 6), int3(6, 0, 2)) , //front face ^
        new (int3(7, 3, 5), int3(5, 3, 1)) , //back face ^
        new (int3(5, 1, 4), int3(4, 1, 0)) , //left face ^
        new (int3(6, 2, 7), int3(7, 2, 3))   //right face ^
    };

    public static readonly float3[] VOXEL_NORMALS = new float3[6]
    {
        up(),
        down(),
        back(),
        forward(),
        left(),
        right()
    };

}
