using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if (UNITY_EDITOR)

[CustomEditor(typeof(World))]
public class WorldEditor : Editor
{

    World world;
    
    public override void OnInspectorGUI()
    {

        using (var check = new EditorGUI.ChangeCheckScope())
        {

            base.OnInspectorGUI();


            



            if (check.changed && world.autoUpdate)
                world.GeneratePreview();

     


            
        }

        


        if (GUILayout.Button("Generate"))
        {
            world.GeneratePreview();           
        }
 


    }


    private void OnEnable()
    {
        world = (World)target;

    }

}
#endif
