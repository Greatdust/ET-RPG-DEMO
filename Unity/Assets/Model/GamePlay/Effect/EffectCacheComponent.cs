using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
[ObjectSystem]
public class EffectCacheComponentAwakeSystem : AwakeSystem<EffectCacheComponent>
{
    public override void Awake(EffectCacheComponent self)
    {
        self.Awake();
    }
}

public class EffectCacheComponent : ETModel.Component
{
    public Dictionary<string, Queue<GameObject>> gameObjPool;
    public Dictionary<string, GameObject> prefabPool;

    private const int InitEffectCount = 3;

    public void Awake()
    {
        gameObjPool = new Dictionary<string, Queue<GameObject>>();
        prefabPool = new Dictionary<string, GameObject>();
    }

    public GameObject Get(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        Queue<GameObject> goQueue;
        gameObjPool.TryGetValue(id, out goQueue);
        if (goQueue == null) return null;
        if (goQueue.Count > 0)
            return goQueue.Dequeue();
        else
        {
            GameObject go = prefabPool[id];
            GameObject result = GetCopy(go);
            return result;
        }
    }

    public bool Contains(string id)
    {
        return prefabPool.ContainsKey(id);
    }

    public GameObject GetCopy(GameObject go)
    {
        if (go != null)
        {
            return GameObject.Instantiate(go);
        }
        return go;
    }

    public void Add(string id, GameObject go)
    {
        go.SetActive(false);
        GameObject newGo = go;
        bool firstAdd = false;
        if (!prefabPool.ContainsKey(id))
        {
            prefabPool[id] = go;
            newGo = GetCopy(go); firstAdd = true;
        }

        if (!gameObjPool.ContainsKey(id))
        {
            gameObjPool[id] = new Queue<GameObject>();
        }
        gameObjPool[id].Enqueue(newGo);
        if (firstAdd)
        {
            for (int i = 0; i < InitEffectCount; i++)
            {
                gameObjPool[id].Enqueue(GetCopy(go));
            }
        }
    }

    public void Recycle(string id, GameObject go)
    {
        ParticleSystem parentSystem = go.GetComponentInChildren<ParticleSystem>();

        parentSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        parentSystem.Simulate(0, true, true);
        
        Add(id, go);
    }

    public void Clear()
    {
        foreach (var v in gameObjPool.Values)
        {
            foreach (var go in v)
            {
                GameObject.Destroy(go);
            }

        }
        gameObjPool.Clear();

    }
}

