using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[ObjectSystem]
public class ItemConfigComponentAwakeSystem : AwakeSystem<ItemConfigComponent>
{
    public override void Awake(ItemConfigComponent self)
    {
        self.Awake();
    }
}

public class ItemConfigComponent : Component
{
    public ItemCollection itemConfigData;

    public static ItemConfigComponent instance;


    public const string abName = "itemconfig.unity3d";
    public void Awake()
    {
        instance = this;
        Game.Scene.GetComponent<ResourcesComponent>().LoadBundle(abName);
        itemConfigData = Game.Scene.GetComponent<ResourcesComponent>().GetAsset(abName, "ItemCollection") as ItemCollection;
        //Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle(abName);

    }

    public ItemData GetDeepCopy(string typeId)
    {
        ItemData data;
        if (!itemConfigData.itemDic.TryGetValue(typeId, out data))
        {
            Log.Error("typeId为" + typeId + "的Item不存在,请检查!");
            return null;
        }
        ItemData newData = DeepCopyHelper.DeepCopyByBin<ItemData>(data);
        newData.itemId = IdGenerater.GenerateId();
        return newData;
    }
    public int GetMaxNumFromTypeId(string typeId)
    {
        ItemData data;
        if (!itemConfigData.itemDic.TryGetValue(typeId, out data))
        {
            Log.Error("typeId为" + typeId + "的Item不存在,请检查!");
            return 0;
        }
        return data.maxNum;
    }

    public ItemData GetData(string typeId)
    {
        ItemData data;
        if (!itemConfigData.itemDic.TryGetValue(typeId, out data))
        {
            Log.Error("typeId为" + typeId + "的Item不存在,请检查!");
            return null;
        }
        return data;
    }


}

