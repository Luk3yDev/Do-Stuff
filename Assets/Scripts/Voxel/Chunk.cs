using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mesh;

public class Chunk : MonoBehaviour
{
    const int CHUNK_SIZE = 18; // -2
    private int[,,] blockData = new int[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

    [SerializeField] int atlasSize;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        
        GetSomeData();
        Generate();
    }

    public void GetSomeData()
    {
        for (int x = 0; x < CHUNK_SIZE; x++)
            for (int y = 0; y < CHUNK_SIZE; y++)
                for (int z = 0; z < CHUNK_SIZE; z++) { 
                    blockData[x, y, z] = Random.Range(0, 5);
                }
    }

    public void Generate()
    {
        Mesh mesh = GenerateMesh(blockData);
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    Mesh GenerateMesh(int[,,] blockData)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        for (int x = 1; x < blockData.GetLength(0) - 1; x++)
            for (int y = 1; y < blockData.GetLength(1) - 1; y++)
                for (int z = 1; z < blockData.GetLength(2) - 1; z++)
                {
                    Vector3[] VertPos = new Vector3[8]{
                        new Vector3(-1,  1, -1), new Vector3(-1,  1,  1),
                        new Vector3( 1,  1,  1), new Vector3( 1,  1, -1),
                        new Vector3(-1, -1, -1), new Vector3(-1, -1,  1),
                        new Vector3( 1, -1,  1), new Vector3( 1, -1, -1),
                    };

                    int[,] Faces = new int[6, 7]{
                        {0, 1, 2, 3, 0, 1, 0},    // top
                        {7, 6, 5, 4, 0, -1, 0},   // bottom
                        {2, 1, 5, 6, 0, 0, 1},    // right
                        {0, 3, 7, 4, 0, 0, -1},   // left
                        {3, 2, 6, 7, 1, 0, 0},    // front
                        {1, 0, 4, 5, -1, 0, 0}    // back
                    };

                    if (blockData[x, y, z] != 0) {
                        for (int n = 0; n < 6; n++) {
                            int neighbor = blockData[x + Faces[n, 4], y + Faces[n, 5], z + Faces[n, 6]];
                            if (neighbor == 0)
                                AddQuad(n, vertices.Count);
                        }
                    }
                            
                    void AddQuad(int f, int v)
                    {
                        for (int i = 0; i < 4; i++) vertices.Add(new Vector3(x, y, z) + VertPos[Faces[f, i]] / 2f);
                        triangles.AddRange(new int[]
                            {
                                v, v + 2, v + 1,
                                v, v + 3, v + 2
                            });

                        Block block = Block.BlockFromID(blockData[x, y, z]);

                        Vector2 uvCoord = block.uvCoordinate;
                        Vector2 bottomleft = uvCoord / atlasSize;

                        uvs.AddRange(new List<Vector2>() { bottomleft + new Vector2(0, 1f) / atlasSize, 
                            bottomleft + new Vector2(1f, 1f) / atlasSize, 
                            bottomleft + new Vector2(1f, 0) / atlasSize, 
                            bottomleft });
                    }
                }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.OptimizeReorderVertexBuffer();

        return mesh;
    }
}
