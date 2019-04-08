

using BulletSharp;
using ETModel;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using BulletUnity;

public static class BulletDataLoadTool
{

    public static HeightfieldTerrainShape LoadTerrainShape(string bytesPath,string configPath,out GCHandle pinnedTerrainData)
    {
        TerrainConfig terrainConfig = JsonHelper.FromJson<TerrainConfig>(File.ReadAllText(configPath));

        byte[] terr = File.ReadAllBytes(bytesPath);

        pinnedTerrainData = GCHandle.Alloc(terr, GCHandleType.Pinned);

        HeightfieldTerrainShape hs = new HeightfieldTerrainShape(terrainConfig.width, terrainConfig.length, pinnedTerrainData.AddrOfPinnedObject(), 1f, 0f, (float)terrainConfig.maxHeight, terrainConfig.upIndex, terrainConfig.scalarType, false);
        hs.SetUseDiamondSubdivision(true);
        hs.LocalScaling = new BulletSharp.Math.Vector3((float)terrainConfig.scale_x, 1f, (float)terrainConfig.scale_z);
        //just allocated several hundred float arrays. Garbage collect now since 99% likely we just loaded the scene
        GC.Collect();
        return hs;
    }

    public static Unit[] LoadStaticObjects_BoxShape(string boxObjsConfigPath)
    {
        List<BoxShapeConfig> boxShapeConfigs = JsonHelper.FromJson<List<BoxShapeConfig>>(File.ReadAllText(boxObjsConfigPath));

        Unit[] units = new Unit[boxShapeConfigs.Count];
        for (int i = 0; i < boxShapeConfigs.Count; i++)
        {
            Unit unit = ComponentFactory.CreateWithId<Unit>(IdGenerater.GenerateId());
            unit.AddComponent<UnitStateComponent>(); // 因为是静态物体,所以不加入全局的UnitStateMgrComponent中
            BBoxShape bBoxShape = unit.AddComponent<BBoxShape>();
            unit.Position = new Vector3((float)boxShapeConfigs[i].postion_x, (float)boxShapeConfigs[i].postion_y, (float)boxShapeConfigs[i].postion_z);
            unit.Rotation = Quaternion.identity;
            bBoxShape.Extents = new Vector3((float)boxShapeConfigs[i].extents_x, (float)boxShapeConfigs[i].extents_y, (float)boxShapeConfigs[i].extents_z);
            BCollisionObject bCollisionObject = unit.AddComponent<BCollisionObject,BCollisionShape>(bBoxShape);
            bCollisionObject.collisionFlags = CollisionFlags.StaticObject;
        }

        return units;


    }
}