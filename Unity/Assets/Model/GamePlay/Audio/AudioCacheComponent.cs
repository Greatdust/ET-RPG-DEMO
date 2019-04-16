using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
[ObjectSystem]
public class AduioCacheComponentAwakeSystem : AwakeSystem<AudioCacheComponent>
{
    public override void Awake(AudioCacheComponent self)
    {
        self.Awake();
    }
}

public class AudioCacheComponent : ETModel.Component
{
    public Dictionary<string, AudioClip> pool;

    public void Awake()
    {
        pool = new Dictionary<string, AudioClip>();
    }

    public AudioClip Get(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        AudioClip clip;
        pool.TryGetValue(id, out clip);
        return clip;
    }

    public void Add(string id, AudioClip clip)
    {
        if (clip == null)
        {
            Log.Error("无法获取音频+"+ id);
        }
        if (string.IsNullOrEmpty(id)) return ;
        pool[id] = clip;
    }

    public void Clear()
    {
        pool.Clear();

    }
}

