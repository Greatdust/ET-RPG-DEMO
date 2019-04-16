using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 发射飞行道具
/// </summary>
[LabelText("发射飞行道具")]
[LabelWidth(150)]
[Serializable]
public class Buff_EmitObj : BaseBuffData
{
    [LabelText("发射的飞行道具")]
    [LabelWidth(150)]
    public string emitObjId;//发射时的特效
    [LabelText("锁定追踪")]
    [LabelWidth(150)]
    public bool lockTarget;//是否是锁定目标的,如果是,那么 该物体就会一直追踪目标
    [LabelText("方向反转")]
    [ShowIf("lockTarget")]
    [LabelWidth(150)]
    public bool reverseDir;//下面所有的位置和方向设定,都是基于目标的位置和前方向的,即面对使用者的方向
    [LabelText("发射位置偏移")]
    [LabelWidth(150)]
    public Vector3Serializer startPosOffset;//相对于使用者的位置,等于在使用者的位置和前方向上加上这个V3
    [LabelText("飞行速度")]
    [LabelWidth(150)]
    public float emitSpeed;//发射速度


    [LabelText("道具所属层")]
    [LabelWidth(150)]
    public UnitLayer layer;
    [LabelText("碰撞遮罩")]
    [LabelWidth(150)]
    public UnitLayerMask layerMask;
    [LabelText("寻找同类")]
    [LabelWidth(150)]
    public bool FindFriend;


    [LabelText("最大生命周期")]
    [LabelWidth(150)]
    public float duration;//生命周期
    [InfoBox("碰撞后执行哪个Pipeline_Collision节点的逻辑")]
    [LabelText("碰撞事件")]
    [LabelWidth(150)]
    public string pipelineSignal; // 执行哪个PipelineData下的逻辑
    [LabelText("特效参数")]
    [LabelWidth(150)]
    public List<string> effectParams ;

    public override string GetBuffIdType()
    {
        return BuffIdType.EmitObj;
    }

}
