using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class SkillActiveConditionAttribute : BaseAttribute
{
    public string BuffType;
    public SkillActiveConditionAttribute(string buffType)
    {
        BuffType = buffType;
    }
}

