using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[LabelText("添加BUFF")]
[LabelWidth(100)]
[Serializable]
public class Buff_AddBuff : BaseBuffData
{
    public BuffGroup buffGroup;

    public override string GetBuffIdType()
    {
        return BuffIdType.AddBuff;
    }

}
