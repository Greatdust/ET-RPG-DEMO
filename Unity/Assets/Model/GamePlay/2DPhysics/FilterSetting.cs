using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum GroupIndex
{
    Default = 0,
    Player = -1,
    Monster = -2, // 在PVP中充当另一方    
}

public enum UnitLayer
{

    Default = 1 << 0,
    Water = 1 << 1,
    Obstacle = 1 << 2,

    Character = 1 << 8,
    

    Max = 1 << 15 // 最大只能到15


}

[Flags]
public enum UnitLayerMask
{
    Default = 1 << 0,
    Water = 1 << 1,
    Obstacle = 1 << 2,

    Character = 1 << 8,


    Max = 1 << 15, // 最大只能到15


    ALL = 0xFFFF // 跟全部东西碰撞
}



public enum UnitTag
{
    Static = -1,
    Default = 0,
    Player = 1,
    Monster = 2, // 普通怪物
    Enchanted = 3,// 附带魔法强化的强化怪物
    EliteMonster = 4, // 精英怪物,小BOSS
    Boss = 5,


    Player_Red = 11,//PVP专用
    Player_Blue = 12,

}

public struct UnitData
{
    public UnitLayer unitLayer;
    public UnitLayerMask layerMask;
    public GroupIndex groupIndex;
    public UnitTag unitTag; 
}