using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    public static class LoadConfigFromJsonTool
    {
        public static NumericRealtionType GetNumericRealtionType(string str)
        {
            switch (str)
            {
                case ">":
                    return NumericRealtionType.Greater;
                case "<":
                    return NumericRealtionType.Less;
                case ">=":
                    return NumericRealtionType.GreaterEqual;
                case "<=":
                    return NumericRealtionType.LessEqual;
                case "==":
                    return NumericRealtionType.Equal;
                case "!=":
                    return NumericRealtionType.NotEqual;
                default:
                    Log.Error("该字符串无法转换为比较关系");
                    throw new Exception();
            }
        }
    }
}
