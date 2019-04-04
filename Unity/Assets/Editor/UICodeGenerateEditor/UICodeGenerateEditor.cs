using System;
using System.Collections.Generic;
using System.IO;
using ETModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using System.Linq;

public class UICodeGeneratetor
{
    /// <summary>
    /// 使用方法:
    /// 把需要生成的UI组件拉到预制体顶层物体的RC里面
    /// 然后根据需要填写namespace和ui继承对象, 输出路径等等 (根据自己需求修改)
    /// </summary>

    private static string nameSpace = "ETHotfix";
    private static string outputPath = "";

    //Component
    private static string scriptComponentName = "";
    private static string uiBaseComponentName = "Component";
    private static string[] usingArrayComponent = { "ETModel", "UnityEngine", "UnityEngine.UI", "System", "TMPro" }; // 根据自己需要添加

    //Adapter
    private static string scriptAdapterName = "";
    private static string uiBaseAdapterName = "Component";
    private static string[] usingArrayAdapter = { "ETModel", "UnityEngine", "UnityEngine.UI", "System", "TMPro" }; // 根据自己需要添加

    //Factory
    private static string scriptFactoryName = "";
    private static string uiBaseFactoryName = "IUIFactory";
    private static string[] usingArrayFactory = { "ETModel", "UnityEngine", "System" }; // 根据自己需要添加


    [MenuItem("自动化工具/生成UI模块View层代码")]
    public static void Gen()
    {
        GameObject go = Selection.activeGameObject;
        outputPath = Application.dataPath + "/Hotfix/UI/" + go.name;
        if (go != null)
        {

            Directory.CreateDirectory(outputPath);
            if (string.IsNullOrEmpty(scriptComponentName))
            {
                scriptComponentName = go.name + "Component";
            }
            if (string.IsNullOrEmpty(scriptAdapterName))
            {
                scriptAdapterName = go.name + "AdapterComponent";
            }
            if (string.IsNullOrEmpty(scriptFactoryName))
            {
                scriptFactoryName = go.name + "Factory";
            }

            if (outputPath == "")
            {
                Log.Debug("请填写输出路径!");
                return;
            }

            ReferenceCollector rc = go.GetComponent<ReferenceCollector>();
            Dictionary<string, Object> dict = rc.GetAll();

            Dictionary<string, List<string>> resultDict = new Dictionary<string, List<string>>();

            foreach (var obj in dict)
            {
                GameObject value = obj.Value as GameObject;
                var gra = value.GetComponents<UIBehaviour>();//UIBehaviour是所有UGUI组件的基类
                if (gra == null)
                {
                    Debug.Log("找不到对应的UI组件  " + obj.Key);
                    return;
                }
                List<string> typeList = new List<string>();
                foreach (var v in gra)
                {
                    typeList.Add(v.GetType().ToString().Split('.').Last());
                }
                resultDict.Add(obj.Key, typeList);

            }

            if (resultDict.Count > 0)
            {
                // 覆盖清空文本
                using (new FileStream(Path.Combine(outputPath, scriptComponentName + ".cs"), FileMode.Create))
                {
                }

                using (StreamWriter sw = new StreamWriter(Path.Combine(outputPath, scriptComponentName + ".cs")))
                {
                    for (int i = 0; i < usingArrayComponent.Length; i++)
                    {
                        sw.WriteLine("using " + usingArrayComponent[i] + ";");
                    }

                    sw.WriteLine("// Code Generate By Tool : " + DateTime.Now);
                    sw.WriteLine("\nnamespace " + nameSpace);
                    sw.WriteLine("{");
                    //ObjectSystem
                    sw.WriteLine("\t[ObjectSystem]");
                    sw.WriteLine("\tpublic class " + scriptComponentName + "System : AwakeSystem<" + scriptComponentName + ">");
                    sw.WriteLine("\t{");
                    sw.WriteLine("\t\tpublic override void Awake(" + scriptComponentName + " self)");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\tself.Awake();");
                    sw.WriteLine("\t\t}");
                    sw.WriteLine("\t}");

                    //class
                    sw.WriteLine("\n\tpublic class " + scriptComponentName + " : " + uiBaseComponentName);
                    sw.WriteLine("\t{");

                    sw.WriteLine("\t\t#region 字段声明");
                    // 字段
                    foreach (var result in resultDict)
                    {
                        foreach (var v in result.Value)
                        {
                            sw.WriteLine("\t\tpublic " + v + " " + result.Key + "_" + v + ";");
                        }
                    }
                    sw.WriteLine("\t\t#endregion");

                    // Awake
                    sw.WriteLine("\n\t\tpublic void Awake()");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\tReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();");
                    // 获取组件
                    foreach (var result in resultDict)
                    {
                        // 这里根据自己需要的写法去获得RC身上的组件
                        foreach (var v in result.Value)
                        {
                            sw.WriteLine("\t\t\t" + result.Key + "_" + v + " = rc.Get<GameObject> (" + '"' + result.Key + '"' + ")" + ".GetComponent<" + v + "> ();");
                        }

                    }
                    sw.WriteLine("\t}");

                    // 生成按钮绑定的对应方法
                    foreach (var result in resultDict)
                    {
                        foreach (var v in result.Value)
                        {
                            if (v == "Button")
                            {
                                sw.WriteLine("\n\t\tpublic void On" + result.Key + "ButtonClick(Action action)");
                                sw.WriteLine("\t\t{");
                                sw.WriteLine("\t\t\t" + result.Key + "_" + v + ".onClick.Add(action);");
                                sw.WriteLine("\t\t}");
                            }
                        }
                    }

                    sw.WriteLine("\t}");
                    sw.WriteLine("}");
                }
                string adapterFilePath = Path.Combine(outputPath, scriptAdapterName + ".cs");
                if (!File.Exists(adapterFilePath))
                {
                    using (new FileStream(adapterFilePath, FileMode.Create))
                    {
                    }
                    using (StreamWriter sw = new StreamWriter(adapterFilePath))
                    {
                        for (int i = 0; i < usingArrayAdapter.Length; i++)
                        {
                            sw.WriteLine("using " + usingArrayAdapter[i] + ";");
                        }

                        sw.WriteLine("// Code Generate By Tool : " + DateTime.Now);
                        sw.WriteLine("\nnamespace " + nameSpace);
                        sw.WriteLine("{");

                        sw.WriteLine("\n\tpublic class " + scriptAdapterName + " : " + uiBaseAdapterName);
                        sw.WriteLine("\t{");
                        sw.WriteLine("\n\t\tpublic " + scriptComponentName + " com;");

                        sw.WriteLine("\n\t\tpublic void Init(" + scriptComponentName + " com)");
                        sw.WriteLine("\t\t{"); 
                        sw.WriteLine("\n\t\t\tthis.com = com ;");
                        sw.WriteLine();
                        sw.WriteLine("\t\t}");

                        sw.WriteLine("\n\t\tpublic void Start()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine();
                        sw.WriteLine("\t\t}");

                        sw.WriteLine("\n\t\tpublic ETTask OnEnable()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\treturn ETTask.CompletedTask;");
                        sw.WriteLine("\t\t}");

                        sw.WriteLine("\n\t\tpublic ETTask OnDisable()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\treturn ETTask.CompletedTask;");
                        sw.WriteLine("\t\t}");

                        sw.WriteLine("\t}");
                        sw.WriteLine("}");
                    }
                }

                string factoryFilePath = Path.Combine(outputPath, scriptFactoryName + ".cs");
                if (!File.Exists(factoryFilePath))
                {
                    using (new FileStream(factoryFilePath, FileMode.Create))
                    {
                    }
                    using (StreamWriter sw = new StreamWriter(factoryFilePath))
                    {
                        for (int i = 0; i < usingArrayFactory.Length; i++)
                        {
                            sw.WriteLine("using " + usingArrayFactory[i] + ";");
                        }

                        sw.WriteLine("// Code Generate By Tool : " + DateTime.Now);
                        sw.WriteLine("\nnamespace " + nameSpace);
                        sw.WriteLine("{");
                        sw.WriteLine("\n\t[UIFactory(UIType." + go.name + ")]");
                        sw.WriteLine("\n\tpublic class " + scriptFactoryName + " : " + uiBaseFactoryName);
                        sw.WriteLine("\t{");

                        sw.WriteLine("\t\tprivate " + scriptAdapterName + " adapter;");
                        //Create Func
                        sw.WriteLine("\n\t\tpublic UI Create()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\ttry");
                        sw.WriteLine("\t\t\t{");
                        sw.WriteLine("\t\t\t\tResourcesComponent res = ETModel.Game.Scene.GetComponent<ResourcesComponent>();");
                        sw.WriteLine("\t\t\t\tres.LoadBundle(UIType." + go.name + ".StringToAB());");
                        sw.WriteLine("\t\t\t\tGameObject bundleGameObject = res.GetAsset(UIType." + go.name + ".StringToAB(), UIType." + go.name + ") as GameObject;");
                        sw.WriteLine("\t\t\t\tGameObject go = UnityEngine.Object.Instantiate(bundleGameObject);");
                        sw.WriteLine("\t\t\t\tgo.layer = LayerMask.NameToLayer(LayerNames.UI);");
                        sw.WriteLine("\t\t\t\tUI ui = ComponentFactory.Create<UI, string, GameObject>(UIType." + go.name + ", go ,false);");
                        sw.WriteLine("\t\t\t\tvar com = ui.AddComponent<" + scriptComponentName + ">();");
                        sw.WriteLine("\t\t\t\tadapter = ui.AddComponent<" + scriptAdapterName + ">();");
                        sw.WriteLine("\t\t\t\tadapter.Init(com);");
                        //sw.WriteLine("\t\t\t\tres.UnloadBundle(" + '"' + go.name.ToLower() + ".unity3d" + '"' + ");");
                        sw.WriteLine("\t\t\t\treturn ui;");
                        sw.WriteLine("\t\t\t}");
                        sw.WriteLine("\t\t\tcatch (Exception e)");
                        sw.WriteLine("\t\t\t{");
                        sw.WriteLine("\t\t\t\tLog.Error(e);");
                        sw.WriteLine("\t\t\t\treturn null;");
                        sw.WriteLine("\t\t\t}");
                        sw.WriteLine("\t\t}");

                        sw.WriteLine("\n\t\tpublic void Start()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tadapter.Start();");
                        sw.WriteLine("\t\t}");

                        sw.WriteLine("\n\t\tpublic ETTask OnEnable()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\treturn adapter.OnEnable();");
                        sw.WriteLine("\t\t}");

                        sw.WriteLine("\n\t\tpublic ETTask OnDisable()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\treturn adapter.OnDisable();");
                        sw.WriteLine("\t\t}");

                        sw.WriteLine("\t}");
                        sw.WriteLine("}");
                    }
                }
                Log.Debug("Job Done!");
                AssetDatabase.Refresh(ImportAssetOptions.ForceUncompressedImport);
            }
        }
    }

}
