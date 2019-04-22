using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 一组BUFF,用来组成玩家眼中的装备/道具/技能等等附加的持续性效果
/// </summary>
[Serializable]
public struct BuffGroup
{
    private long buffGroupId;

    [HideInEditorMode]
    [NonSerialized]
    public long sourceUnitId;// 添加到一个Unit的BuffMgr上时,这个用以记录这个buffGroup的来源

    public int buffTypeId;
    [InfoBox("-1代表持续到BUFF组被解除,0代表瞬间完成.大于0代表持续一段时间")]
    [LabelText("Buff持续时间")]
    [LabelWidth(150)]
    public float duration ;

    [ListDrawerSettings(ShowItemCount = true)]
    public List<BaseBuffData> buffList;

    public long BuffGroupId
    {
        get
        {
            if (buffGroupId == 0)
                buffGroupId = IdGenerater.GenerateId();
            return buffGroupId;
        }
        set => buffGroupId = value;
    }

  
}

