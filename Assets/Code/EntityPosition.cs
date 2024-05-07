using UnityEngine;

[System.Serializable]
public struct EntityPosition
{
    public Vector3Int voxelPosition;
    public Vector3Int chunkPosition;
    public Vector3 rawPosition;

    //Never displayed, used for internal logic
    [System.NonSerialized]
    public Vector3Int previousChunkPosition;

}