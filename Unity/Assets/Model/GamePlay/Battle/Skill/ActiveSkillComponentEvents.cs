using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class ChooseSkillHanlder : AEvent<UnitActionData>
{
    public override void Run(UnitActionData a)
    {
        ActiveSkillComponent skillCom = a.mUnit.GetComponent<ActiveSkillComponent>();
        skillCom.Excute(a).Coroutine();
    }
}


public class RecChoosedSkill : AEvent<long, ActiveSkillData>
{
    public override void Run(long unitID, ActiveSkillData a)
    {
        Unit unit = UnitComponent.Instance.Get(unitID);
        SkillHelper.ChooseSkillDataTask.SetResult(a);
    }
}


public class GotBuffTargetEvent : AEvent<long>
{
    public override void Run(long unitId)
    {
        Debug.Log("已接收到目标" + unitId);

    }
}