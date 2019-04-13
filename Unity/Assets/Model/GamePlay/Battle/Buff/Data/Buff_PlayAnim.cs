using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[Serializable]
public class Buff_PlayAnim : BaseBuffData
{
  
    [LabelText("BOOL变量名")]
    [LabelWidth(150)]
    [HorizontalGroup]
    public string anim_boolValue;
    [HorizontalGroup]
    [LabelText("BOOL值")]
    [LabelWidth(150)]
    public bool boolValue;
    [LabelText("触发器变量名")]
    [LabelWidth(150)]
    public string anim_triggerValue;

    [LabelText("动作初始速度")]
    [LabelWidth(150)]
    //实际上真正影响动作速度的,应该是玩家的一个类似于 施法速度的值. 而不是这个江湖救急的值
    public float playSpeed; // 动作播放速度. 可能需要用到的情况是提供的动画策划觉得速度不对,要微调一下. 交给动画人员重新调整需要时间,不如程序微调一下先看效果



    public override string GetBuffIdType()
    {
        return BuffIdType.PlayAnim;
    }

}
