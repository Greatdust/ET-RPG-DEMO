using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using ETModel;
public static class PassiveSkillListenedEventType
{
    public const string HP改变 = EventIdType.HPChanged; // 处理血量越低,给的属性越多这种情况
    public const string MP改变 = EventIdType.MPChanged;
}
