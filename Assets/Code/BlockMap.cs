using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public struct BlockMap
{

    private NativeArray<BlockType> blocks;

    public BlockMap(int length)
    {
        blocks = new NativeArray<BlockType>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
    }

    public BlockMap(NativeArray<BlockType> blocks)
    {
        this.blocks = blocks;
    }

    public void Dispose()
    {
        blocks.Dispose();
    }

    // 3-Dimensional Access
    public BlockType this[int x, int y, int z]
    {
        get => blocks[x + y * WorldSettings.CHUNK_SIZE.x + z * WorldSettings.CHUNK_SIZE.x * WorldSettings.CHUNK_SIZE.y];
        set => blocks[x + y * WorldSettings.CHUNK_SIZE.x + z * WorldSettings.CHUNK_SIZE.x * WorldSettings.CHUNK_SIZE.y] = value;
    }

    // 1-Dimensional Access
    public BlockType this[int i]
    {
        get => blocks[i];
        set => blocks[i] = value;
    }

}
