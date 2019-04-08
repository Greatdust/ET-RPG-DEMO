using BulletUnity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Bullet_ExportTool : Editor
{
    private static string ExportTerrainPath = "../../../Config/PhysicWorlds/";

    [MenuItem("Bullet双端工具/导出静态物体(需要选中静态物体的根节点)")]
    static void ExportStaticObjsToBytes()
    {
        Transform trans = Selection.activeGameObject.transform;
        BCollisionObject[] collisionObjs = trans.GetComponentsInChildren<BCollisionObject>();
        if (collisionObjs.Length <= 0)
        {
            Debug.Log("没有检测到BCollisionObject");
            return;
        }

        //导出所有BoxShapeConfig
        List<BoxShapeConfig> staticBoxShapes = new List<BoxShapeConfig>();

        foreach (var v in collisionObjs)
        {
            BBoxShape bBoxShape = v.GetComponent<BBoxShape>();
            if (bBoxShape != null)
            {
                staticBoxShapes.Add(new BoxShapeConfig()
                {
                    postion_x = bBoxShape.transform.position.x,
                    postion_y = bBoxShape.transform.position.y,
                    postion_z = bBoxShape.transform.position.z,
                    extents_x = bBoxShape.Extents.x,
                    extents_y = bBoxShape.Extents.y,
                    extents_z = bBoxShape.Extents.z
                });
            }
        }


        string configPath = Application.dataPath + ExportTerrainPath + UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;

        if (!Directory.Exists(configPath))
        {
            Directory.CreateDirectory(configPath);
        }


        File.WriteAllText(configPath + "/staticObjs_boxs.config", LitJson.JsonMapper.ToJson(staticBoxShapes));

        Debug.Log("成功导出静态物体数据,路径为: " + configPath);

    }

    [MenuItem("Bullet双端工具/导出地形(需要选中TerrainData)")]
    static void ExportTerrainDataToBytes()
    {
        TerrainData td = Selection.activeObject as TerrainData;
        if (td == null)
        {
            Debug.Log("你选中的并不是地形数据!");
            return;
        }

        int width = td.heightmapWidth;
        int length = td.heightmapHeight;
        float maxHeight = td.size.y;


        byte[] terr = new byte[width * length * sizeof(float)];
        System.IO.MemoryStream file = new System.IO.MemoryStream(terr);
        System.IO.BinaryWriter writer = new System.IO.BinaryWriter(file);

        for (int i = 0; i < length; i++)
        {
            float[,] row = td.GetHeights(0, i, width, 1);
            for (int j = 0; j < width; j++)
            {
                writer.Write((float)row[0, j] * maxHeight);
            }
        }

        string path = Application.dataPath + ExportTerrainPath + UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }


        string bytesPath = path + "/terrainData.bytes";
        string configPath = path + "/terrainConfig.config";

        File.WriteAllBytes(bytesPath, terr);

        TerrainConfig terrainConfig = new TerrainConfig();
        terrainConfig.width = td.heightmapWidth;
        terrainConfig.length = td.heightmapHeight;
        terrainConfig.maxHeight = td.size.y;
        terrainConfig.scale_x = td.heightmapScale.x;
        terrainConfig.scale_y = 1.0f;
        terrainConfig.scale_z = td.heightmapScale.z;

        File.WriteAllText(Application.streamingAssetsPath + configPath, LitJson.JsonMapper.ToJson(terrainConfig));

        file.Close();
        file.Dispose();
        writer.Close();
        writer.Dispose();

        Debug.Log("成功导出地形数据,路径为: " + path);

    }
}
