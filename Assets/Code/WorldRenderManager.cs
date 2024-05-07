using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class WorldRenderManager : MonoBehaviour
{

    Vector3 renderRegionOrigin; //point to start generating chunk grid from. Not the same as player location.

    public GameObject[,] chunkObjects;
    public Chunk[,] chunkData;
    public NativeList<JobHandle> jobs;

    int backColumnIndex = 0;
    int frontColumnIndex = WorldSettings.RENDER_DISTANCE - 1;
    int backRowIndex = 0;
    int frontRowIndex = WorldSettings.RENDER_DISTANCE - 1;


    private void Start()
    {
        MultiStream.DefineDescriptors();
        LoadChunks();
    }

    private void Update()
    {
        UpdateChunks();
    }

    void UpdateChunks()
    {

        if(Player.player.loadPositiveXChunks)
        {
            jobs = new NativeList<JobHandle>(2, Allocator.Temp);

            for (int i = 0; i < WorldSettings.RENDER_DISTANCE; i++)
            {
                //update Position
                chunkObjects[backColumnIndex, i].transform.position = new Vector3(chunkObjects[backColumnIndex, i].transform.position.x + (WorldSettings.CHUNK_WIDTH * WorldSettings.RENDER_DISTANCE), 0f, chunkObjects[backColumnIndex, i].transform.position.z);
                chunkObjects[backColumnIndex, i].name = "chunk (" + chunkObjects[backColumnIndex, i].transform.position.x / WorldSettings.CHUNK_WIDTH + ", " + chunkObjects[backColumnIndex, i].transform.position.z / WorldSettings.CHUNK_DEPTH + ")";

                //Update Chunk Data
                chunkData[backColumnIndex, i].offset = new Vector2(chunkData[backColumnIndex, i].transform.position.x, chunkData[backColumnIndex, i].transform.position.z);

                float time = Time.realtimeSinceStartup;

                jobs.Add(chunkData[backColumnIndex, i].ScheduleMesh());
                //chunkData[backColumnIndex, i].LoadMesh();

                Debug.Log((Time.realtimeSinceStartup - time) * 1000f + " ms");
            }

            float times = Time.realtimeSinceStartup;
            JobHandle.CompleteAll(jobs);
            for (int i = 0; i < WorldSettings.RENDER_DISTANCE; i++)
            {
                chunkData[backColumnIndex, i].FinalizeMesh();
            }
            Debug.Log("Job completion: " + (Time.realtimeSinceStartup - times) * 1000f + " ms");


            frontColumnIndex = backColumnIndex;
            backColumnIndex++;
            if (backColumnIndex > WorldSettings.RENDER_DISTANCE - 1)
                backColumnIndex = 0;
        }

        if(Player.player.loadNegativeXChunks)
        {
            jobs = new NativeList<JobHandle>(2, Allocator.Temp);

            for (int i = 0; i < WorldSettings.RENDER_DISTANCE; i++)
            {
                //update Position
                chunkObjects[frontColumnIndex, i].transform.position = new Vector3(chunkObjects[frontColumnIndex, i].transform.position.x - (WorldSettings.CHUNK_WIDTH * WorldSettings.RENDER_DISTANCE), 0f, chunkObjects[frontColumnIndex, i].transform.position.z);
                chunkObjects[frontColumnIndex, i].name = "chunk (" + chunkObjects[frontColumnIndex, i].transform.position.x / WorldSettings.CHUNK_WIDTH + ", " + chunkObjects[frontColumnIndex, i].transform.position.z / WorldSettings.CHUNK_DEPTH + ")";

                //Update Chunk Data
                chunkData[frontColumnIndex, i].offset = new Vector2(chunkData[frontColumnIndex, i].transform.position.x, chunkData[frontColumnIndex, i].transform.position.z);

                float time = Time.realtimeSinceStartup;

                jobs.Add(chunkData[frontColumnIndex, i].ScheduleMesh());
                //chunkData[frontColumnIndex, i].LoadMesh();

                Debug.Log((Time.realtimeSinceStartup - time) * 1000f + " ms");
            }

            float times = Time.realtimeSinceStartup;
            JobHandle.CompleteAll(jobs);
            for (int i = 0; i < WorldSettings.RENDER_DISTANCE; i++)
            {
                chunkData[frontColumnIndex, i].FinalizeMesh();
            }
            Debug.Log("Job completion: " + (Time.realtimeSinceStartup - times) * 1000f + " ms");

            backColumnIndex = frontColumnIndex;
            frontColumnIndex--;
            if (frontColumnIndex < 0)
                frontColumnIndex = WorldSettings.RENDER_DISTANCE - 1;
        }

        if(Player.player.loadPositiveZChunks)
        {
            jobs = new NativeList<JobHandle>(2, Allocator.Temp);

            for (int i = 0; i < WorldSettings.RENDER_DISTANCE; i++)
            {
                //update Position
                chunkObjects[i, backRowIndex].transform.position = new Vector3(chunkObjects[i, backRowIndex].transform.position.x, 0f, chunkObjects[i, backRowIndex].transform.position.z + (WorldSettings.CHUNK_DEPTH * WorldSettings.RENDER_DISTANCE));
                chunkObjects[i, backRowIndex].name = "chunk (" + chunkObjects[i, backRowIndex].transform.position.x / WorldSettings.CHUNK_DEPTH + ", " + chunkObjects[i, backRowIndex].transform.position.z / WorldSettings.CHUNK_WIDTH + ")";

                //Update Chunk Data
                chunkData[i, backRowIndex].offset = new Vector2(chunkData[i, backRowIndex].transform.position.x, chunkData[i, backRowIndex].transform.position.z);

                float time = Time.realtimeSinceStartup;

                jobs.Add(chunkData[i, backRowIndex].ScheduleMesh());
                //chunkData[i, backRowIndex].LoadMesh();

                Debug.Log((Time.realtimeSinceStartup - time) * 1000f + " ms");
            }

            float times = Time.realtimeSinceStartup;
            JobHandle.CompleteAll(jobs);
            for (int i = 0; i < WorldSettings.RENDER_DISTANCE; i++)
            {
                chunkData[i, backRowIndex].FinalizeMesh();
            }
            Debug.Log("Job completion: " + (Time.realtimeSinceStartup - times) * 1000f + " ms");

            frontRowIndex = backRowIndex;
            backRowIndex++;
            if (backRowIndex > WorldSettings.RENDER_DISTANCE - 1)
                backRowIndex = 0;
        }

        if(Player.player.loadNegativeZChunks)
        {
            jobs = new NativeList<JobHandle>(2, Allocator.Temp);

            for (int i = 0; i < WorldSettings.RENDER_DISTANCE; i++)
            {
                //update Position
                chunkObjects[i, frontRowIndex].transform.position = new Vector3(chunkObjects[i, frontRowIndex].transform.position.x, 0f, chunkObjects[i, frontRowIndex].transform.position.z - (WorldSettings.CHUNK_DEPTH * WorldSettings.RENDER_DISTANCE));
                chunkObjects[i, frontRowIndex].name = "chunk (" + chunkObjects[i, frontRowIndex].transform.position.x / WorldSettings.CHUNK_WIDTH + ", " + chunkObjects[i, frontRowIndex].transform.position.z / WorldSettings.CHUNK_DEPTH + ")";

                //Update Chunk Data
                chunkData[i, frontRowIndex].offset = new Vector2(chunkData[i, frontRowIndex].transform.position.x, chunkData[i, frontRowIndex].transform.position.z);

                float time = Time.realtimeSinceStartup;

                jobs.Add(chunkData[i, frontRowIndex].ScheduleMesh());
                //chunkData[i, frontRowIndex].LoadMesh();

                Debug.Log((Time.realtimeSinceStartup - time) * 1000f + " ms");
            }

            float times = Time.realtimeSinceStartup;
            JobHandle.CompleteAll(jobs);
            for (int i = 0; i < WorldSettings.RENDER_DISTANCE; i++)
            {
                chunkData[i, frontRowIndex].FinalizeMesh();
            }
            Debug.Log("Job completion: " + (Time.realtimeSinceStartup - times) * 1000f + " ms");

            backRowIndex = frontRowIndex;
            frontRowIndex--;
            if (frontRowIndex < 0)
                frontRowIndex = WorldSettings.RENDER_DISTANCE - 1;
        }

    }

    void LoadChunks()
    {

        chunkObjects = new GameObject[WorldSettings.RENDER_DISTANCE, WorldSettings.RENDER_DISTANCE];
        chunkData = new Chunk[WorldSettings.RENDER_DISTANCE, WorldSettings.RENDER_DISTANCE];
        renderRegionOrigin = new Vector3(Player.player.playerPosition.chunkPosition.x * WorldSettings.CHUNK_WIDTH, 0f, Player.player.playerPosition.chunkPosition.z * WorldSettings.CHUNK_WIDTH) - new Vector3((WorldSettings.RENDER_DISTANCE * WorldSettings.CHUNK_WIDTH / 2), 0f, (WorldSettings.RENDER_DISTANCE * WorldSettings.CHUNK_WIDTH / 2));
        Debug.Log("render origin: " + Player.player.playerPosition.chunkPosition * WorldSettings.CHUNK_WIDTH);

        int offsetX = 0;
        int offsetZ;

        for(int i = 0; i < WorldSettings.RENDER_DISTANCE; i++)
        {
            offsetZ = 0;

            for(int j = 0; j < WorldSettings.RENDER_DISTANCE; j++)
            {
                chunkObjects[i, j] = new GameObject();
                //chunkObjects[i, j] = Instantiate(Resources.Load("ChunkTemp") as GameObject);
                chunkObjects[i, j].transform.position = new Vector3((int)renderRegionOrigin.x + offsetX, 0, (int)renderRegionOrigin.z + offsetZ);
                chunkObjects[i, j].name = "chunk (" + chunkObjects[i, j].transform.position.x/WorldSettings.CHUNK_WIDTH + ", " + chunkObjects[i, j].transform.position.z/WorldSettings.CHUNK_WIDTH + ")";
                chunkData[i, j] = chunkObjects[i, j].AddComponent<Chunk>();
                chunkObjects[i, j].GetComponent<MeshRenderer>().material = Resources.Load("TerrainMat") as Material;

                offsetZ += WorldSettings.CHUNK_DEPTH;
            }

            offsetX += WorldSettings.CHUNK_WIDTH;

        }
    }
}
