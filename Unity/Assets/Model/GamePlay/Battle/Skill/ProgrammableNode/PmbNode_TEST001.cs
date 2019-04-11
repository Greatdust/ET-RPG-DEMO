using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//随便测试一下
//角色前方 5m处为圆心,半径12m的区域,落点20个,计算出对应的抛物线的初始方向和初始速度
//根据这些方向和速度,发射20个火球,落地爆炸造成一定伤害

public class PmbNode_TEST001 : IPipeline_PmbNode
{
    public bool emit; // 每次发射火球的时候都判断一下,如果为false ,那就证明被打断了,就不发射了

    public void Excute(SkillData_Var data)
    {
        //TODO: 
        //1. 根据unit来获得技能执行时的中间数据存储在哪
        //2. 计算出20个落点的抛物线,初始方向和初始速度
        //3. 存储计算出来的方向和速度
        //4. 根据上面的结果让角色播放持续施法动作,在2秒内连续发射20个火球.落地带爆炸造成伤害的那种
    }

    public void Break(SkillData_Var data)
    {
        //连续发射火球.如果中间被打断,那就停止发射.
        //
    }
}

