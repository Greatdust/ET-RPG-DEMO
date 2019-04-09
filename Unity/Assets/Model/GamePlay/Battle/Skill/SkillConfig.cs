using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig/SkillConfig")]
public class SkillConfig : ScriptableObject
{
    [LabelText("主动技能最大数量")]
    [SerializeField]
    private int activeSkillNum;
    public int ActiveSkillNum
    {
        get => activeSkillNum;
    }

    [LabelText("被动技能最大数量")]
    [SerializeField]
    private int passiveSkillNum;
    public int PassiveSkillNum
    {
        get => passiveSkillNum;
    }
}

