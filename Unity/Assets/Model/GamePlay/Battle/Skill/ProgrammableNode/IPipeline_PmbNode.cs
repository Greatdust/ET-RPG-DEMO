using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct SkillData_Var
{
    //给予最大自由度 ,
    //想获得Unit的什么属性就获得什么属性,
    //想获得全局的属性就获得全局的属性
    //想获得技能的信息,这里也直接给出

    public string skillId;
    public string pipelineSignal;
    public Unit source; 
}

public interface IPipeline_PmbNode
{
    //适用情景1:  作为中间件.给其他非编程节点传递数据
    //        例如:   发射一片随机的爆炸火球,即位置不重复,在选取的方向的一定范围内,落下数量乘以N的炮弹. 那么这里可以向BufferValue中添加随机出来的方向

    //适用情景2:  直接完整控制整个技能的流程.
    //        这种情况就相当于用脚本写了一个技能

    //其他的能想到的再写
    //如果这都满足不了游戏的需求. 要么策划SB,要么就是这套框架完全不适合你的游戏. 请更换或者调整

    void Excute(SkillData_Var data);

    void Break(SkillData_Var data); // 处理打断的
}


