using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UnitAudioSourceHelper : UnityEngine.MonoBehaviour
{
    public AudioSource attackAS;//播放单位使用技能等攻击行为产生的音效的音源
    public AudioSource envirAS;//播放单位与环境交互产生的音效的音源
    public AudioSource moveAS;//播放单位移动时产生的音效的音源
    public AudioSource emoteAS;//播放单位说话时,做表情时产生的音源
}

