using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
  
    readonly World world;

    GameObject chunkGameObject;

    ChunkCoords chunkCoords;



    List<GameObject> props = new List<GameObject>();

    List<float> heights = new List<float>();
    List<Color> colors = new List<Color>();

   
    public TerrainChunk(World world, ChunkCoords chunkCoords)
    {
        this.world = world;
        this.chunkCoords = chunkCoords;

        CreateGameObject();
        Populate();
        UpdateMesh();

    }



    void CreateGameObject() {

        chunkGameObject = new GameObject();
        chunkGameObject.transform.SetParent(world.transform);
        chunkGameObject.transform.position = chunkCoords.getStartPos();
        chunkGameObject.name = chunkCoords.posX + " " + chunkCoords.posZ;

        chunkGameObject.AddComponent<MeshFilter>();
        chunkGameObject.AddComponent<MeshCollider>();
        chunkGameObject.AddComponent<MeshRenderer>().materials = new Material[] { world.material , world.waterMaterial };
    }

    void Populate()
    {
        int vertexAmount = World.chunkVoxelWidth + 1;
        for (int z = 0; z < vertexAmount; z++)
        {
            for (int x = 0; x < vertexAmount; x++)
            {


                float worldX = (x + chunkCoords.getVoxelPosX) * World.voxelSize;
                float worldZ = (z + chunkCoords.getVoxelPosZ) * World.voxelSize;


                float realTerrainHeight = 3 + WorldGeneration.PerlinNoise2D(worldX, worldZ, 0.2f, world.seed) * 0.3f + WorldGeneration.PerlinNoise2D(worldX, worldZ, 0.02f,world.seed) * 7;
                Color color = world.ground.Evaluate(WorldGeneration.PerlinNoise2D(worldX, worldZ, 0.1f, world.seed));

             

                float distanceToCenter = Vector3.Distance(Vector3.zero, new Vector3(worldX, 0, worldZ));
              


                float dis = 80 + WorldGeneration.PerlinNoise2D(worldX, worldZ, 0.03f, world.seed) * 20;

             

                if (distanceToCenter < dis)
                {
                    realTerrainHeight = Mathf.Lerp(realTerrainHeight, 2.5f + WorldGeneration.PerlinNoise2D(worldX, worldZ, 0.06f, world.seed)*0.3f, (dis - distanceToCenter) / world.blend);
                

                }

                float terrainHeight = realTerrainHeight;


                float rivers = 1 - world.river.Evaluate(1 - WorldGeneration.RidgidNoise2D(worldX, worldZ, 2, 0.001f, world.seed));
                terrainHeight = Mathf.Lerp(terrainHeight, -0.3f, rivers);



                float paths = Math.Max(1 - world.path.Evaluate(1 - WorldGeneration.RidgidNoise2D(worldX, worldZ, 4, 0.001f, world.seed + 166)), 1 - world.path.Evaluate(1 - WorldGeneration.RidgidNoise2D(worldX, worldZ, 2, 0.004f, world.seed + 1326)));
                terrainHeight = Mathf.Lerp(terrainHeight, Mathf.Lerp(realTerrainHeight,2.5f,0.5f), paths);

  

            
                if (paths > 0.5f)
                {
                    color = Color.Lerp(color, world.pathColor.Evaluate(WorldGeneration.PerlinNoise2D(worldX, worldZ, 0.1f, world.seed)), paths);
                }


                if (terrainHeight < 2.5f)              
                    color = Color.Lerp(color, world.lake.Evaluate(WorldGeneration.PerlinNoise2D(worldX, worldZ, 0.1f, world.seed)), Mathf.Clamp01((2.5f - terrainHeight) / 2.5f));

                if (x < vertexAmount - 1 && z < vertexAmount - 1)
                {

                   
                    if (terrainHeight > 2.5f && paths < 0.5f)
                        if (distanceToCenter > dis-world.blend)
                        {
                            if (WorldGeneration.PerlinNoise2D(worldX, worldZ, 1.12f, world.seed) > 0.65f)
                            {
                                GameObject gameObject = ObjectPool.Spawn(world.trees[0], (Vector3)(chunkCoords.getStartVoxelPos()) * World.voxelSize + new Vector3(x * World.voxelSize, terrainHeight * World.voxelSize - 0.1f, z * World.voxelSize), Quaternion.identity, chunkGameObject.transform);

                                float scale = 1+2*WorldGeneration.PerlinNoise2D(worldX, worldZ, 1.32f, world.seed+503);
                                gameObject.transform.localScale = new Vector3(scale, scale, scale);
                                props.Add(gameObject);
                            }

                                

                        }

                }




              
                               

                heights.Add(terrainHeight);
                colors.Add(color);

           



            }
        }

   

    }

    void UpdateMesh()
    {
        int vertexAmount = World.chunkVoxelWidth + 1;

        MeshFilter meshFilter = chunkGameObject.GetComponent<MeshFilter>();
        MeshCollider meshCollider = chunkGameObject.GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();




        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<int> waterTriangles = new List<int>();

        List<Color> cols = new List<Color>();

        int i = 0;
        for (int z = 0; z < vertexAmount; z++)
        {
            for (int x = 0; x < vertexAmount; x++)
            {
               


                if (x != vertexAmount - 1 && z != vertexAmount - 1)
                {
                    vertices.Add(new Vector3((x), heights[(x) + (z) * vertexAmount], (z)) * World.voxelSize);
                    vertices.Add(new Vector3((x+1), heights[(x+1) + (z) * vertexAmount], (z)) * World.voxelSize);
                    vertices.Add(new Vector3((x+1), heights[(x+1) + (z+1) * vertexAmount], (z+1)) * World.voxelSize);
                    vertices.Add(new Vector3((x), heights[(x) + (z+1) * vertexAmount], (z+1)) * World.voxelSize);

                    cols.Add(colors[(x) + (z) * vertexAmount]);
                    cols.Add(colors[(x+1) + (z) * vertexAmount]);
                    cols.Add(colors[(x+1) + (z+1) * vertexAmount]);
                    cols.Add(colors[(x) + (z+1) * vertexAmount]);



           
                    triangles.Add(i);
                    triangles.Add(i + 2);
                    triangles.Add(i + 1);



                    triangles.Add(i + 2);
                    triangles.Add(i);
                    triangles.Add(i + 3);

                    i += 4;

                }

            }
        }



        for (int z = 0; z < World.chunkVoxelWidth; z++)
        {
            for (int x = 0; x < World.chunkVoxelWidth; x++)
            {


                if(heights[x+z*vertexAmount] <= World.voxelSize * 2)
                {
                    vertices.Add((new Vector3(-0.5f,0,-0.5f) + new Vector3((x), 1, (z))) * World.voxelSize);
                    vertices.Add((new Vector3(-0.5f, 0, -0.5f) + new Vector3((x + 1), 1, (z))) * World.voxelSize);
                    vertices.Add((new Vector3(-0.5f, 0, -0.5f) + new Vector3((x + 1), 1, (z + 1))) * World.voxelSize);
                    vertices.Add((new Vector3(-0.5f, 0, -0.5f) + new Vector3((x), 1, (z + 1))) * World.voxelSize);

                    cols.Add(Color.red);
                    cols.Add(Color.red);
                    cols.Add(Color.red);
                    cols.Add(Color.red);

                    waterTriangles.Add(i);
                    waterTriangles.Add(i + 2);
                    waterTriangles.Add(i + 1);


                    waterTriangles.Add(i + 2);
                    waterTriangles.Add(i);
                    waterTriangles.Add(i + 3);

                    i += 4;

                }

            }
        }







        mesh.Clear();
        mesh.vertices = vertices.ToArray();


        mesh.subMeshCount = 2;
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetTriangles(waterTriangles.ToArray(), 1);
   
        mesh.colors = cols.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        Mesh colMesh = new Mesh();

        colMesh.vertices = vertices.ToArray();
        colMesh.triangles = triangles.ToArray();
        colMesh.RecalculateNormals();

        meshCollider.sharedMesh = colMesh;

        
    }

    public void Delete()
    {
        if(world.chunks.ContainsKey(chunkCoords))
            world.chunks.Remove(chunkCoords);
        foreach (var item in props)
        {
            ObjectPool.Remove(item);
        }
        if(chunkGameObject)
            MonoBehaviour.Destroy(chunkGameObject);
    }
}

