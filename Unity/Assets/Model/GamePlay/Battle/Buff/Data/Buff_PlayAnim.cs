using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[LabelText("播放角色动作")]
[LabelWidth(150)]
[Serializable]
public class Buff_PlayAnim : BaseBuffData
{
  
    public string anim_boolValue;
    public bool boolValue;
    public string anim_triggerValue;

    public float origin;//原时间
    public float playTime = 0;//动作实际播放时间



    public override string GetBuffIdType()
    {
        return BuffIdType.PlayAnim;
    }

}
