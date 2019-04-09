using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class BuffTypeAttribute : BaseAttribute
{
    public string BuffType;
    public BuffTypeAttribute(string buffType)
    {
        BuffType = buffType;
    }
}

