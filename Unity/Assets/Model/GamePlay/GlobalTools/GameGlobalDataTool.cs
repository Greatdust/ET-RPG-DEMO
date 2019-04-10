using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameGlobalDataTool
{
    #region GlobalTrigger
    [Serializable]
    public class CheckTrigger
    {
        public string triggerName;

    }
    public struct SetTrigger
    {
        public string triggerName;

        public void Trigger()
        {
           // Game.EventSystem.Run(EventIdType.GlobalTrigger, triggerName);
        }
    }
    #endregion

    #region GlobalVar
    [Serializable]
    public class CheckSpecialGlobalVar
    {
        [Serializable]
        public class IntVar
        {
            public int value;
            public NumericRealtionType relationType;
        }
        [Serializable]
        public class FloatVar
        {
            public float value;
            public NumericRealtionType relationType;
        }

        public Dictionary<string, IntVar> intVarDic;
        public Dictionary<string, FloatVar> floatVarDic;
        public Dictionary<string, bool> boolVarDic;
        public Dictionary<string, string> stringVarDic;

        public void SetVar(string varId, int value)
        {
            if (intVarDic == null)
                intVarDic = new Dictionary<string, IntVar>();
            intVarDic[varId] = new IntVar();
            intVarDic[varId].value = value;
        }

        public void SetVar(string varId, float value)
        {
            if (floatVarDic == null)
                floatVarDic = new Dictionary<string, FloatVar>();
            floatVarDic[varId] = new FloatVar();
            floatVarDic[varId].value = value;
        }

        public void SetVar(string varId, bool value)
        {
            if (boolVarDic == null)
                boolVarDic = new Dictionary<string, bool>();
            boolVarDic[varId] = value;
        }
        public void SetVar(string varId, string value)
        {
            if (stringVarDic == null)
                stringVarDic = new Dictionary<string, string>();
            stringVarDic[varId] = value;
        }


        public bool Check()
        {
            GlobalVariableComponent globalVariableComponent = GlobalVariableComponent.Instance;
            if (this.intVarDic != null && this.intVarDic.Count > 0)
            {
                foreach (var v in this.intVarDic)
                {
                    if (!globalVariableComponent.globalVars.intVarDic.ContainsKey(v.Key)) continue;
                    switch (v.Value.relationType)
                    {
                        case NumericRealtionType.Greater:
                            if (globalVariableComponent.globalVars.intVarDic[v.Key] <= v.Value.value)
                            {
                                return false;
                            }
                            break;
                        case NumericRealtionType.Less:
                            if (globalVariableComponent.globalVars.intVarDic[v.Key] >= v.Value.value)
                            {
                                return false;
                            }
                            break;
                        case NumericRealtionType.Equal:
                            if (globalVariableComponent.globalVars.intVarDic[v.Key] != v.Value.value)
                            {
                                return false;
                            }
                            break;
                        case NumericRealtionType.GreaterEqual:
                            if (globalVariableComponent.globalVars.intVarDic[v.Key] < v.Value.value)
                            {
                                return false;
                            }
                            break;
                        case NumericRealtionType.LessEqual:
                            if (globalVariableComponent.globalVars.intVarDic[v.Key] > v.Value.value)
                            {
                                return false;
                            }
                            break;
                    }
                }
            }
            if (this.floatVarDic != null && this.floatVarDic.Count > 0)
            {
                foreach (var v in this.floatVarDic)
                {
                    if (!globalVariableComponent.globalVars.floatVarDic.ContainsKey(v.Key)) continue;
                    switch (v.Value.relationType)
                    {
                        case NumericRealtionType.Greater:
                            if (globalVariableComponent.globalVars.floatVarDic[v.Key] <= v.Value.value)
                            {
                                return false;
                            }
                            break;
                        case NumericRealtionType.Less:
                            if (globalVariableComponent.globalVars.floatVarDic[v.Key] >= v.Value.value)
                            {
                                return false;
                            }
                            break;
                        case NumericRealtionType.Equal:
                            if (globalVariableComponent.globalVars.floatVarDic[v.Key] != v.Value.value)
                            {
                                return false;
                            }
                            break;
                        case NumericRealtionType.GreaterEqual:
                            if (globalVariableComponent.globalVars.floatVarDic[v.Key] < v.Value.value)
                            {
                                return false;
                            }
                            break;
                        case NumericRealtionType.LessEqual:
                            if (globalVariableComponent.globalVars.floatVarDic[v.Key] > v.Value.value)
                            {
                                return false;
                            }
                            break;
                    }
                }
            }
            if (this.boolVarDic != null && this.boolVarDic.Count > 0)
            {
                foreach (var v in this.boolVarDic)
                {
                    if (!globalVariableComponent.globalVars.boolVarDic.ContainsKey(v.Key)) continue;
                    if (globalVariableComponent.globalVars.boolVarDic[v.Key] != v.Value) return false;
                }
            }
            if (this.stringVarDic != null && this.stringVarDic.Count > 0)
            {
                foreach (var v in this.stringVarDic)
                {
                    if (!globalVariableComponent.globalVars.stringVarDic.ContainsKey(v.Key)) continue;
                    if (globalVariableComponent.globalVars.stringVarDic[v.Key] != v.Value) return false;
                }
            }
            return true;
        }
    }
    [Serializable]
    public class SetSpecialGlobalVar
    {
        public Dictionary<string, int> intVarDic;
        public Dictionary<string, float> floatVarDic;
        public Dictionary<string, bool> boolVarDic;
        public Dictionary<string, string> stringVarDic;
        public void SetVar(string varId, int value)
        {
            if (intVarDic == null)
                intVarDic = new Dictionary<string, int>();
            intVarDic[varId] = value;
        }

        public void SetVar(string varId, float value)
        {
            if (floatVarDic == null)
                floatVarDic = new Dictionary<string, float>();
            floatVarDic[varId] = value;
        }

        public void SetVar(string varId, bool value)
        {
            if (boolVarDic == null)
                boolVarDic = new Dictionary<string, bool>();
            boolVarDic[varId] = value;
        }
        public void SetVar(string varId, string value)
        {
            if (stringVarDic == null)
                stringVarDic = new Dictionary<string, string>();
            stringVarDic[varId] = value;
        }

        public void Excute()
        {
            if (intVarDic != null && intVarDic.Count > 0)
            {
                foreach (var v in intVarDic)
                {
                    GlobalVariableComponent globalConfigComponent = GlobalVariableComponent.Instance;
                    globalConfigComponent.globalVars.intVarDic[v.Key] = v.Value;
                }
            }
            if (floatVarDic != null && floatVarDic.Count > 0)
            {
                foreach (var v in floatVarDic)
                {
                    GlobalVariableComponent globalConfigComponent = GlobalVariableComponent.Instance;
                    globalConfigComponent.globalVars.floatVarDic[v.Key] = v.Value;
                }
            }
            if (boolVarDic != null && boolVarDic.Count > 0)
            {
                foreach (var v in boolVarDic)
                {
                    GlobalVariableComponent globalConfigComponent = GlobalVariableComponent.Instance;
                    globalConfigComponent.globalVars.boolVarDic[v.Key] = v.Value;
                }
            }
            if (stringVarDic != null && stringVarDic.Count > 0)
            {
                foreach (var v in stringVarDic)
                {
                    GlobalVariableComponent globalConfigComponent = GlobalVariableComponent.Instance;
                    globalConfigComponent.globalVars.stringVarDic[v.Key] = v.Value;
                }
            }

        }
    }
    #endregion

    #region Item,Equip,Exp,Numeric...etc

    [Serializable]
    public struct AttrData
    {
        [LabelText("属性类型")]
        [LabelWidth(150)]
        public NumericType attrType;
        [LabelText("属性值")]
        [LabelWidth(150)]
        public float value;
    }

    [Serializable]
    public struct ItemData
    {
        public string itemType;
    }
    [Serializable]
    public struct ItemDataForGive
    {
        public string itemType;
        public int num;
    }
    [Serializable]
    public struct ItemDataForCheck
    {
        public ItemDataForGive itemDataForReward;
        public int needNum;
    }
    [Serializable]
    public struct ItemDataForReward
    {
        public string itemType;
        public int minNum;
        public int maxNum;
        public float displayRate;
    }
    [Serializable]
    public struct EquipDataForGive
    {
        public string equipType;
    }
    [Serializable]
    public struct EquipDataForCheck_ItemLevel
    {
        public int needItemLevel;
    }
    [Serializable]
    public struct EquipDataForReward
    {
        public string equipType;
        public float displayRate;
    }
    [Serializable]
    public struct ExpDataForReward
    {
        public int exp;
    }
    [Serializable]
    public struct NumericDataForReward
    {
        public NumericType numericType;
        public float value;
    }

    [Serializable]
    public class HPRecoverData
    {
        public float pct;
        public float value;
        public void Excute(long unitId)
        {
            if (pct > 0)
            {
                Unit unit = UnitComponent.Instance.MyUnit;
                float maxValue = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.HPMax_Final);
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.HP_Final, unitId, maxValue * pct);
            }
            if (value > 0)
            {

                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.HP_Final, unitId, value);
            }
        }
    }

    [Serializable]
    public class MPRecoverData
    {
        public float pct;
        public float value;
        public void Excute(long unitId)
        {
            if (pct > 0)
            {
                Unit unit = UnitComponent.Instance.MyUnit;
                float maxValue = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.HPMax_Final);
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.MP, unitId, maxValue * pct);
            }
            if (value > 0)
            {

                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.MP, unitId, value);
            }
        }
    }

    #endregion
}




