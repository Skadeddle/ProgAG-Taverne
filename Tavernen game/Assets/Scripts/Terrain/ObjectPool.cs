using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    public Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();


    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }


    public static void Remove(GameObject toRemove)
    {
        if (instance)
            instance.RemoveObject(toRemove);
        else
            DestroyImmediate(toRemove);
    }

    public static GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        if (instance)
            return instance.SpawnObject(prefab,pos,rot,parent);

        GameObject gameObject = Instantiate(prefab,pos,rot);
        gameObject.transform.SetParent(parent);
        return gameObject;
    }





    public void RemoveObject(GameObject toRemove) {
        if (!pool.ContainsKey(toRemove.name))
            pool.Add(toRemove.name, new Queue<GameObject>());

 
        toRemove.transform.SetParent(transform);
        pool[toRemove.name].Enqueue(toRemove);
    }

    public GameObject SpawnObject(GameObject prefab,Vector3 pos,Quaternion rot,Transform parent = null) {
        if (pool.ContainsKey(prefab.name) && pool[prefab.name].Count > 0)
        {
            GameObject go = pool[prefab.name].Dequeue();
            go.transform.position = pos;
            go.transform.rotation = rot;
            go.transform.SetParent(parent);


   

            return go;
        }
        else
        {
            GameObject go = Instantiate(prefab);
            go.transform.position = pos;
            go.transform.rotation = rot;
            go.transform.SetParent(parent);
            go.name = prefab.name;

            return go;
        }
    }
}
