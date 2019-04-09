using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class EditorItemConfigAttribute : BaseAttribute
{
    public Type key;
    public EditorItemConfigAttribute(Type type)
    {
        key = type;
    }
}

