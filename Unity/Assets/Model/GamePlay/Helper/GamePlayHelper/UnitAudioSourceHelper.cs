using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UnitAudioSourceHelper : MonoBehaviour
{
    [InfoBox("播放单位使用技能等攻击行为产生的音效的音源")]
    public AudioSource attackAS;//播放单位使用技能等攻击行为产生的音效的音源
    [InfoBox("播放单位与环境交互产生的音效的音源")]
    public AudioSource envirAS;//播放单位与环境交互产生的音效的音源
    [InfoBox("播放单位移动时产生的音效的音源")]
    public AudioSource moveAS;//播放单位移动时产生的音效的音源
    [InfoBox("播放单位说话时,做表情时产生的音源")]
    public AudioSource emoteAS;//播放单位说话时,做表情时产生的音源

    //需要根据脚底下的材质控制播放什么样的音频

        [Serializable]
    public struct MaterialToClip
    {
        public Material material;
        public AudioClip[] audioClips;
    }

    public List<MaterialToClip> materialToClips = new List<MaterialToClip>();


    private Dictionary<Material, AudioClip[]> moveAudioClips = new Dictionary<Material, AudioClip[]>();

    public AudioClip[] defaultMoveClips;

    private bool moving;

    private void Start()
    {
        if (materialToClips == null) return;
        foreach (var v in materialToClips)
        {
            moveAudioClips.Add(v.material, v.audioClips);
        }
    }


    public void PlayMoveSound(float pitch)
    {
        moving = true;
        moveAS.loop = false;
       // moveAS.pitch = pitch;
    }

    public void PauseMoveSound()
    {
        moving = false;
        moveAS.Pause();
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            if (moveAS.isPlaying) return;
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
            Material mat = null;
            if (Physics.Raycast(ray, out hit, 10, LayerMask.GetMask("Map", "Water"), QueryTriggerInteraction.Ignore))
            {
                Renderer groundRenderer = hit.collider.GetComponentInChildren<Renderer>();
                mat = groundRenderer ? groundRenderer.sharedMaterial : null;

            }
            AudioClip clip = defaultMoveClips[RandomHelper.RandomNumber(0, defaultMoveClips.Length)];
            if (mat != null && moveAudioClips.ContainsKey(mat))
            {
                clip = moveAudioClips[mat][RandomHelper.RandomNumber(0, moveAudioClips[mat].Length)];
            }
            moveAS.clip = clip;
            moveAS.Play();
        }
    }

}

