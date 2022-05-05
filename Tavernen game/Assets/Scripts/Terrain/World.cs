
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class World : MonoBehaviour
{
    public static World instance;

    // good color
    // #A43A00

    private void Awake()
    {
        instance = this;



        chunks.Clear();

        while (0 < transform.childCount)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

 
    public Transform[] loadAround;


    


    public const float voxelSize = 2;


    public const int chunkVoxelWidth = 8;




    [Header("Generation")]

    public int seed = 100;
    public GameObject[] trees;

    public float blend = 30;

    public Gradient ground;
    public Gradient lake;
    public Gradient pathColor;

    public AnimationCurve river;

    public AnimationCurve path;



    public Material material;
    public Material waterMaterial;


    [Header("Preview")]

    public bool autoUpdate;

    public Dictionary<ChunkCoords, TerrainChunk> chunks = new Dictionary<ChunkCoords, TerrainChunk>();

    public ChunkCoords GetChunkCoordsFromWorldPos(float PosX, float PosZ)
    {
        // in c# casting to int does Math.Truncate
        return new ChunkCoords(Mathf.FloorToInt((PosX / voxelSize) / chunkVoxelWidth), Mathf.FloorToInt((PosZ / voxelSize) / chunkVoxelWidth));
    }
    public ChunkCoords GetChunkCoordsFromPos(int PosX,int PosZ) {
        // in c# casting to int does Math.Truncate
        return new ChunkCoords(Mathf.FloorToInt((float)PosX / chunkVoxelWidth), Mathf.FloorToInt((float)PosZ / chunkVoxelWidth));
    }

 

  
    public void GeneratePreview() {
        while (0 < transform.childCount)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        chunks.Clear();

        int dis = 10;
        for (int x = 1 - dis; x < dis; x++)
        {
            for (int z = 1 - dis; z < dis; z++)
            {
                ChunkCoords coord = new ChunkCoords(x, z);
                GetChunkFromChunkCoords(coord);
            }
        }
    }



    public List<ChunkCoords> active = new List<ChunkCoords>();
   

    void updateChunks() {
        List<ChunkCoords> lastActive = new List<ChunkCoords>(active);
        active.Clear();
        foreach (var item in loadAround)
        {
            ChunkCoords coords = GetChunkCoordsFromWorldPos((int)item.position.x, (int)item.position.z);

            if (!active.Contains(coords))
                active.Add(coords);

            int dis = 10;
            for (int x = 1 - dis; x < dis; x++)
            {
                for (int z = 1 - dis; z < dis; z++)
                {
                    ChunkCoords coord = new ChunkCoords(coords.posX+x,coords.posZ+z);
                    if(!active.Contains(coord))
                        active.Add(coord);
                }
            }


        }
 

        foreach (var item in active)
        {

            if (lastActive.Contains(item))
                lastActive.Remove(item);
            else
                GetChunkFromChunkCoords(item);


        }

        foreach (var item in lastActive)
        {
            RemoveChunk(item);
        }
    
    
    }



    TerrainChunk GetChunkFromChunkCoords(ChunkCoords at) {
        if (chunks.ContainsKey(at))
            return chunks[at];

        TerrainChunk chunk = new TerrainChunk(this,at);
        chunks.Add(at, chunk);

        return chunk;

    }

    void RemoveChunk(ChunkCoords at) {
        if (!chunks.ContainsKey(at))
            return;

        chunks[at].Delete();

 
    }
 



   


    private void FixedUpdate()
    {


        updateChunks();


    }





}

public class ChunkCoords{
    public readonly int posX;
    public readonly int posZ;

    public ChunkCoords(int posX,int posZ) {
        this.posX = posX;
        this.posZ = posZ;
    }

    public int getVoxelPosX 
    {
        get { return posX * World.chunkVoxelWidth; }

    }
    public int getVoxelPosZ
    {
        get { return posZ * World.chunkVoxelWidth; }
    }

    public Vector3Int getStartVoxelPos() {
        return new Vector3Int(World.chunkVoxelWidth * posX,0, World.chunkVoxelWidth * posZ);
    }
    public Vector3 getStartPos()
    {
        return new Vector3(World.chunkVoxelWidth * posX * World.voxelSize, 0, World.chunkVoxelWidth * posZ * World.voxelSize);
    }
    public override bool Equals(System.Object obj) {
        if (this.GetType().Equals(obj.GetType()))
        {
            ChunkCoords p = (ChunkCoords)obj;
            return (posX == p.posX) && (posZ == p.posZ);
        }
        return false;
    }
    public override int GetHashCode()
    {
        return posX * 10000000 + posZ;
    }
    public override string ToString()
    {
        return "Chunk("+posX + " " + posZ+")";
    }
}

