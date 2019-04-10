using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BattleReward
{
    //long是对应的Unit
    public Dictionary<long, (int,float,float,bool)> expDatas = new Dictionary<long, (int, float, float,bool)>();
    //装备的itemId
    public List<long> equipDatas = new List<long>();
    //道具的typeId+数量
    public Dictionary<string, int> itemDatas = new Dictionary<string, int>();
}



public static class BattleEventHandler
{


    public static void LoadAssets(DungeonComponent dungeonCom)
    {
        EffectCacheComponent effectCacheComponent = Game.Scene.GetComponent<EffectCacheComponent>();
        AudioCacheComponent audioCacheComponent = Game.Scene.GetComponent<AudioCacheComponent>();
        //加载和缓存特效资源
        foreach (var unit in dungeonCom.playerTeam)
        {
            ActiveSkillComponent skillComponent = unit.GetComponent<ActiveSkillComponent>();
            AddEffectCache(effectCacheComponent, audioCacheComponent, skillComponent);
            //TODO: 添加被动技能的
        }
        foreach (var unit in dungeonCom.enemyTeam)
        {
            ActiveSkillComponent skillComponent = unit.GetComponent<ActiveSkillComponent>();
            AddEffectCache(effectCacheComponent,audioCacheComponent, skillComponent);

        }
    }

    static void AddEffectCache(EffectCacheComponent effectCacheComponent,AudioCacheComponent audioCacheComponent, ActiveSkillComponent skillComponent)
    {
        foreach (var skillData in skillComponent.activeSkillDic.Values)
        {
            Game.Scene.GetComponent<ResourcesComponent>().LoadBundle(skillData.skillAssetsABName.StringToAB());
            GameObject skillAssetsPrefabGo = Game.Scene.GetComponent<ResourcesComponent>().GetAsset(skillData.skillAssetsABName.StringToAB(), skillData.skillAssetsABName) as GameObject;

            if (skillData.AllBuffInSkill != null && skillData.AllBuffInSkill.Count > 0)
            {
                //循环检测每一个Buff,看是否是播放特效的,如果是,那就找到对应的Buff 缓存起来
                foreach (var buff in skillData.AllBuffInSkill.Values)
                {
                    if (!buff.enable) continue;
                    AddEffectCache(effectCacheComponent, buff.buffData, skillAssetsPrefabGo);
                    AddAudioCache(audioCacheComponent, buff.buffData, skillAssetsPrefabGo);
                    if (buff.buffData.GetBuffIdType() == BuffIdType.AddBuff)
                    {
                        Buff_AddBuff buff_AddBuffOnSkillEnd = buff.buffData as Buff_AddBuff;
                        if (buff_AddBuffOnSkillEnd.buffGroup.buffList == null) continue;
                        foreach (var buff_onSkillEnd in buff_AddBuffOnSkillEnd.buffGroup.buffList)
                        {
                            AddEffectCache(effectCacheComponent, buff_onSkillEnd, skillAssetsPrefabGo);
                            AddAudioCache(audioCacheComponent, buff_onSkillEnd, skillAssetsPrefabGo);
                        }
                    }
                }
            }
        }
    }
    static void AddEffectCache(EffectCacheComponent effectCacheComponent, BaseBuffData buff, GameObject skillAssetsPrefabGo)
    {
        if (buff.GetBuffIdType() == BuffIdType.EmitEffectInSkill)
        {
            Buff_EmitEffect buff_EmitEffect = buff as Buff_EmitEffect;
            if (buff_EmitEffect != null)
            {
                if (!string.IsNullOrEmpty(buff_EmitEffect.emitObjId) && !effectCacheComponent.Contains(buff_EmitEffect.emitObjId))
                    effectCacheComponent.Add(buff_EmitEffect.emitObjId, skillAssetsPrefabGo.Get<GameObject>(buff_EmitEffect.emitObjId));
            }
        }
        if (buff.GetBuffIdType() == BuffIdType.HitEffect)
        {
            Buff_HitEffect buff_HitEffect = buff as Buff_HitEffect;
            if (buff_HitEffect != null)
            {
                if (!string.IsNullOrEmpty(buff_HitEffect.hitObjId) && !effectCacheComponent.Contains(buff_HitEffect.hitObjId))
                    effectCacheComponent.Add(buff_HitEffect.hitObjId, skillAssetsPrefabGo.Get<GameObject>(buff_HitEffect.hitObjId));
            }

        }
    }
    static void AddAudioCache(AudioCacheComponent audioCacheComponent, BaseBuffData buff, GameObject skillAssetsPrefabGo)
    {
        if (buff.GetBuffIdType() == BuffIdType.PlaySound)
        {
            Buff_PlaySound buff_PlaySound = buff as Buff_PlaySound;
            if (buff_PlaySound != null)
            {
                if (!string.IsNullOrEmpty(buff_PlaySound.audioClipName) && audioCacheComponent.Get(buff_PlaySound.audioClipName) == null)
                    audioCacheComponent.Add(buff_PlaySound.audioClipName, skillAssetsPrefabGo.Get<AudioClip>(buff_PlaySound.audioClipName));
            }
        }
    }

    static void ReleaseAssets(DungeonComponent dungeonCom)
    {
        EffectCacheComponent effectCacheComponent = Game.Scene.GetComponent<EffectCacheComponent>();
        effectCacheComponent.Clear();
        AudioCacheComponent audioCacheComponent = Game.Scene.GetComponent<AudioCacheComponent>();
        audioCacheComponent.Clear();
        foreach (var v in dungeonCom.playerTeam)
        {
            if (v.Id == UnitComponent.Instance.MyUnit.Id)
            {
                v.GetComponent<BuffMgrComponent>().ClearBuffGroupOnBattleEnd();             
            }
        }
        foreach (var v in dungeonCom.enemyTeam)
        {
            UnitComponent.Instance.Remove(v.Id);
        }
    }

}

