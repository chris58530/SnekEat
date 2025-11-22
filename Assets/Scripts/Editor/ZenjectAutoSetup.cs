using UnityEngine;
using UnityEditor;
using Zenject;
using System.IO;
using System.Collections.Generic;

public class ZenjectAutoSetup : EditorWindow
{
    [MenuItem("Tools/SnekRun/Auto Setup Zenject")]
    public static void Setup()
    {
        SetupProjectContext();
        SetupSceneContextAndInstaller();
    }

    static void SetupProjectContext()
    {
        // 1. 確保 Resources 資料夾存在
        string resourcesPath = Application.dataPath + "/Resources";
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
            AssetDatabase.Refresh();
        }

        // 2. 檢查 ProjectContext Prefab 是否存在
        string prefabPath = "Assets/Resources/ProjectContext.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
        {
            GameObject pc = new GameObject("ProjectContext");
            pc.AddComponent<ProjectContext>();

            // 儲存為 Prefab
            PrefabUtility.SaveAsPrefabAsset(pc, prefabPath);
            GameObject.DestroyImmediate(pc);

            Debug.Log($"[ZenjectAutoSetup] Created ProjectContext at {prefabPath}");
        }
        else
        {
            Debug.Log("[ZenjectAutoSetup] ProjectContext already exists.");
        }
    }

    static void SetupSceneContextAndInstaller()
    {
        // 1. 尋找或建立 SceneContext
        SceneContext sceneContext = Object.FindAnyObjectByType<SceneContext>();
        if (sceneContext == null)
        {
            GameObject scGo = new GameObject("SceneContext");
            sceneContext = scGo.AddComponent<SceneContext>();
            Undo.RegisterCreatedObjectUndo(scGo, "Create SceneContext");
            Debug.Log("[ZenjectAutoSetup] Created SceneContext in scene.");
        }

        // 2. 尋找或建立 GameInstaller
        GameInstaller installer = Object.FindAnyObjectByType<GameInstaller>();
        if (installer == null)
        {
            // 嘗試找一個叫做 "GameInstaller" 的物件，如果沒有就建立
            GameObject installerGo = GameObject.Find("GameInstaller");
            if (installerGo == null)
            {
                installerGo = new GameObject("GameInstaller");
                Undo.RegisterCreatedObjectUndo(installerGo, "Create GameInstaller");
            }

            installer = installerGo.AddComponent<GameInstaller>();
            Debug.Log("[ZenjectAutoSetup] Created GameInstaller in scene.");
        }

        // 3. 將 Installer 加入 SceneContext
        // 注意：SceneContext 的 Installers 列表是 [SerializeField] private 的，
        // 但 Zenject 的 SceneContext 有公開的 Installers 屬性 (IEnumerable)，
        // 不過在 Editor 模式下我們要修改的是序列化的資料。

        SerializedObject so = new SerializedObject(sceneContext);
        SerializedProperty installersProp = so.FindProperty("_monoInstallers"); // Zenject 內部欄位名稱通常是 _monoInstallers

        if (installersProp != null)
        {
            // 檢查是否已經在列表中
            bool alreadyExists = false;
            for (int i = 0; i < installersProp.arraySize; i++)
            {
                SerializedProperty element = installersProp.GetArrayElementAtIndex(i);
                if (element.objectReferenceValue == installer)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
            {
                int index = installersProp.arraySize;
                installersProp.InsertArrayElementAtIndex(index);
                installersProp.GetArrayElementAtIndex(index).objectReferenceValue = installer;
                so.ApplyModifiedProperties();
                Debug.Log("[ZenjectAutoSetup] Added GameInstaller to SceneContext.");
            }
        }
        else
        {
            Debug.LogWarning("[ZenjectAutoSetup] Could not find '_monoInstallers' property on SceneContext. Please add GameInstaller manually.");
        }

        Selection.activeGameObject = sceneContext.gameObject;
    }
}
