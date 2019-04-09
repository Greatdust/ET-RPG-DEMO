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
                    return NumericRealtionType.大于;
                case "<":
                    return NumericRealtionType.小于;
                case ">=":
                    return NumericRealtionType.大于等于;
                case "<=":
                    return NumericRealtionType.小于等于;
                case "==":
                    return NumericRealtionType.等于;
                case "!=":
                    return NumericRealtionType.不等于;
                default:
                    Log.Error("该字符串无法转换为比较关系");
                    throw new Exception();
            }
        }
    }
}
