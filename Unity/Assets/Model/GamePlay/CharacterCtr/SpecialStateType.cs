using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    public enum SpecialStateType
    {
        [LabelText("无法被打断")]
        UnStoppable,//无法被打断的状态
        [LabelText("玩家无法控制")]
        NotInControl,//玩家无法操作的状态
        [LabelText("无敌")]
        Invincible,
        //后续什么沉默或者其他的状态,都可以在这里加. DEMO的话,这几个足够了
    }
}
