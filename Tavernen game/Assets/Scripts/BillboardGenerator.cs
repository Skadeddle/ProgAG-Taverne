using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BillboardGenerator : MonoBehaviour
{
    #if UNITY_EDITOR
    public BillboardAsset m_outputFile;
    public Material m_material;

    public float width = 30.35058f;
    public float height = 7.172371f;
    public float bottom = -0.2622106f;

    [ContextMenu("Bake Billboard")]
    void BakeBillboard()
    {
        BillboardAsset billboard = new BillboardAsset();

        billboard.material = m_material;
        Vector4[] texCoords = new Vector4[1];
        ushort[] indices = new ushort[6];
        Vector2[] vertices = new Vector2[4];
        texCoords[0].Set(0, 0, 1, 1);
       

        indices[0] = 2;
        indices[1] = 3;
        indices[2] = 0;
        indices[3] = 1;
        indices[4] = 2;
        indices[5] = 0;
  

        vertices[0].Set(0,0);
        vertices[1].Set(0,1);
        vertices[2].Set(1,1);
        vertices[3].Set(1,0);


        billboard.SetImageTexCoords(texCoords);
        billboard.SetIndices(indices);
        billboard.SetVertices(vertices);

        billboard.width = width;
        billboard.height = height;
        billboard.bottom = bottom;

        if (m_outputFile != null)
        {
            EditorUtility.CopySerialized(billboard, m_outputFile);
        }
        else
        {
            string path;
            path = AssetDatabase.GetAssetPath(m_material) + ".asset";
            AssetDatabase.CreateAsset(billboard, path);
        }
    }
#endif
}
