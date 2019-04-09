using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[ObjectSystem]
public class EquipConfigComponentAwakeSystem : AwakeSystem<EquipConfigComponent>
{
    public override void Awake(EquipConfigComponent self)
    {
        self.Awake();
    }
}

public class EquipConfigComponent : Component
{
    public EquipCollection itemConfigData;

    public static EquipConfigComponent instance;


    public const string abName = "equipconfig.unity3d";
    public void Awake()
    {
        instance = this;
        Game.Scene.GetComponent<ResourcesComponent>().LoadBundle(abName);
        itemConfigData = Game.Scene.GetComponent<ResourcesComponent>().GetAsset(abName, "EquipCollection") as EquipCollection;
        //Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle(abName);

    }

    public EquipData GetDeepCopy(string typeId)
    {
        EquipData data;
        if (!itemConfigData.equipDic.TryGetValue(typeId, out data))
        {
            Log.Error("typeId为" + typeId + "的Equip不存在,请检查!");
            return null;
        }
        EquipData newData = DeepCopyHelper.DeepCopyByBin<EquipData>(data);
        newData.itemId = IdGenerater.GenerateId();
        return newData;
    }

    public EquipData GetData(string typeId)
    {
        EquipData data;
        if (!itemConfigData.equipDic.TryGetValue(typeId, out data))
        {
            Log.Error("typeId为" + typeId + "的Equip不存在,请检查!");
            return null;
        }
        return data;
    }
}

