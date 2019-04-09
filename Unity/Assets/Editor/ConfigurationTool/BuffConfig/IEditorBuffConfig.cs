using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IEditorBuffConfig
{
    void DrawingBuff(BaseBuffData data);
}

public interface IEditorBuff_TimelineConfig 
{
    void DrawTimeLineBuff(BaseBuffData timeLineBuff);
}

public interface IEditorBuff_TriggerConfig
{
    void DrawTrigger(BaseBuffData baseBuffData);
}

public interface IEditorBuffActiveConditionConfig
{
    void DrawCondition(BaseSkillData.IActiveConditionData buffActiveConditionData);
}

