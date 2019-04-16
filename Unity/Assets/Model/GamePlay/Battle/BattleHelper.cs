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

    public static void LoadAssets(Unit unit)
    {
        EffectCacheComponent effectCacheComponent = Game.Scene.GetComponent<EffectCacheComponent>();
        AudioCacheComponent audioCacheComponent = Game.Scene.GetComponent<AudioCacheComponent>();
        //加载和缓存特效资源

        ActiveSkillComponent skillComponent = unit.GetComponent<ActiveSkillComponent>();
        AddEffectCache(effectCacheComponent, audioCacheComponent, skillComponent);
        //TODO: 添加被动技能的

    }

    static void AddEffectCache(EffectCacheComponent effectCacheComponent,AudioCacheComponent audioCacheComponent, ActiveSkillComponent skillComponent)
    {
        foreach (var skill in skillComponent.skillList)
        {
            ActiveSkillData skillData = Game.Scene.GetComponent<SkillConfigComponent>().GetActiveSkill(skill.Key);
            Game.Scene.GetComponent<ResourcesComponent>().LoadBundle(skillData.skillAssetsABName.StringToAB());
            GameObject skillAssetsPrefabGo = Game.Scene.GetComponent<ResourcesComponent>().GetAsset(skillData.skillAssetsABName.StringToAB(), skillData.skillAssetsABName) as GameObject;

            var buffList = skillData.GetAllBuffs(BuffIdType.EmitObj, BuffIdType.HitEffect, BuffIdType.PlayEffect, BuffIdType.PlaySound,BuffIdType.AddBuff);
            if (buffList.Count > 0)
            {
                //循环检测每一个Buff,看是否是播放特效的,如果是,那就找到对应的Buff 缓存起来
                foreach (var buff in buffList)
                {
                    AddEffectCache(effectCacheComponent, buff, skillAssetsPrefabGo);
                    AddAudioCache(audioCacheComponent, buff, skillAssetsPrefabGo);
                    if (buff.GetBuffIdType() == BuffIdType.AddBuff)
                    {
                        Buff_AddBuff buff_AddBuffOnSkillEnd = (Buff_AddBuff)buff;
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
        if (buff.GetBuffIdType() == BuffIdType.EmitObj)
        {
            Buff_EmitObj buff_EmitEffect = (Buff_EmitObj)buff;
            if (!string.IsNullOrEmpty(buff_EmitEffect.emitObjId) && !effectCacheComponent.Contains(buff_EmitEffect.emitObjId))
                effectCacheComponent.Add(buff_EmitEffect.emitObjId, skillAssetsPrefabGo.Get<GameObject>(buff_EmitEffect.emitObjId));

        }
        if (buff.GetBuffIdType() == BuffIdType.HitEffect)
        {
            Buff_HitEffect buff_HitEffect = (Buff_HitEffect)buff;
            if (!string.IsNullOrEmpty(buff_HitEffect.hitObjId) && !effectCacheComponent.Contains(buff_HitEffect.hitObjId))
                effectCacheComponent.Add(buff_HitEffect.hitObjId, skillAssetsPrefabGo.Get<GameObject>(buff_HitEffect.hitObjId));


        }
        if (buff.GetBuffIdType() == BuffIdType.PlayEffect)
        {
            Buff_PlayEffect buff_PlayEffect = (Buff_PlayEffect)buff;
            if (!string.IsNullOrEmpty(buff_PlayEffect.effectObjId) && !effectCacheComponent.Contains(buff_PlayEffect.effectObjId))
                effectCacheComponent.Add(buff_PlayEffect.effectObjId, skillAssetsPrefabGo.Get<GameObject>(buff_PlayEffect.effectObjId));


        }
    }
    static void AddAudioCache(AudioCacheComponent audioCacheComponent, BaseBuffData buff, GameObject skillAssetsPrefabGo)
    {
        if (buff.GetBuffIdType() == BuffIdType.PlaySound)
        {
            Buff_PlaySound buff_PlaySound = (Buff_PlaySound)buff;
            Log.Debug("添加音效" + buff_PlaySound.audioClipName);
                audioCacheComponent.Add(buff_PlaySound.audioClipName, skillAssetsPrefabGo.Get<AudioClip>(buff_PlaySound.audioClipName));

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

